using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private bool isGhost;

    //[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField]
    private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider; // A collider that will be disabled when crouching

    const float k_GroundedRadius = 0.12f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded; // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true; // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")] [Space] public UnityEvent OnLandEvent;

    private bool _isShocked;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;
    private Animator _animator;


    private float _coyoteTimeThreshold = 0.05f;
    private float _timeLeftGrounded;
    private bool _coyoteUsable;

    private float _jumpTimeCounter;
    private float _jumpTime = .17f;
    private float _jumpForce = 7.5f;
    private bool _isJumping;
    private static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsHittingLaser = Animator.StringToHash("IsHittingLaser");

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _isShocked = false;


        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        if (wasGrounded && !m_Grounded)
        {
            _timeLeftGrounded = Time.fixedTime;
            _animator.SetBool(IsGrounded, false);
        }

        if (!wasGrounded && m_Grounded)
        {
            _coyoteUsable = true;
            _animator.SetBool(IsGrounded, true);
        }

        if (!isGhost)
            _animator.SetFloat(VerticalSpeed, m_Rigidbody2D.velocity.y);
    }


    public void Move(float move, bool crouch, bool jump, bool holdJump)
    {
        if (!_isShocked)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch)
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // If crouching
                if (crouch)
                {
                    if (!m_wasCrouching)
                    {
                        m_wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }

                    // Reduce the speed by the crouchSpeed multiplier
                    move *= m_CrouchSpeed;

                    // Disable one of the colliders when crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = false;
                }
                else
                {
                    // Enable the collider when not crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = true;

                    if (m_wasCrouching)
                    {
                        m_wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                if (!isGhost)
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity,
                        m_MovementSmoothing);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            // If the player should jump...
            if (m_Grounded && jump)
            {
                _isJumping = true;
                _jumpTimeCounter = _jumpTime;
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.velocity = (new Vector2(m_Rigidbody2D.velocity.x, _jumpForce));
                //m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                //Play the jump sound-------
                AudioManager a = FindObjectOfType<AudioManager>();
                if (a)
                    a.Play("Jump");
                //---------------------------
                if (!m_Grounded)
                {
                    _animator.SetBool("IsGrounded", false);
                }
            }

            // If the player should jump with COYOTE JUMP...
            if (!m_Grounded && _coyoteUsable && _timeLeftGrounded + _coyoteTimeThreshold > Time.fixedTime && jump)
            {
                _isJumping = true;
                _jumpTimeCounter = _jumpTime;
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.velocity = (new Vector2(m_Rigidbody2D.velocity.x, 0));
                m_Rigidbody2D.velocity = (new Vector2(m_Rigidbody2D.velocity.x, _jumpForce));
                //m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                //Play the jump sound-------
                AudioManager a = FindObjectOfType<AudioManager>();
                if (a)
                    a.Play("Jump");
                //---------------------------
                if (!m_Grounded)
                {
                    _animator.SetBool("IsGrounded", false);
                }
            }

            //If the player keeps pressing the jump key, the strength of the jump increases
            if (holdJump && _isJumping)
            {
                if (_jumpTimeCounter > 0)
                {
                    m_Rigidbody2D.velocity = (new Vector2(m_Rigidbody2D.velocity.x, _jumpForce));
                    _jumpTimeCounter -= Time.fixedDeltaTime;
                }
                else
                {
                    _isJumping = false;
                }
            }

            //If the player releases the jump key while jumping you will stop the boost of the charged jump
            if (!holdJump && _isJumping)
            {
                _isJumping = false;
            }
        }
    }

    public bool GetGrounded()
    {
        return m_Grounded;
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void SetShocked()
    {
        if (!_isShocked)
        {
            _isShocked = true;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.isKinematic = true;
            _animator.SetBool(IsHittingLaser, true);
            StartCoroutine("WaitToShockAnimation");
        }
    }

    IEnumerator WaitToShockAnimation()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.UpdateGameState(GameState.StartingYoungTurn);
    }
}