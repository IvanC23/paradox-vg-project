using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OldController2D : MonoBehaviour
{
    [SerializeField] private GameObject uiCanvas;
   
    [Range(0, 1)] [SerializeField]
    private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider; // A collider that will be disabled when crouching

    const float k_GroundedRadius = 0.24f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded; // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true; // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private AudioManager _audioManager;

    [Header("Events")] [Space] public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private bool _wasJetpack = false;
    //---Dash--------------------------
    [Header("Dash settings")] 
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTimeDuration = 0.2f;
    [SerializeField] private Image cooldownImageDash;
    [SerializeField] private float dashCooldownTimeDuration;
    private float _dashCooldownTime;
    private bool _isChargingDash;
    private float _dashTime;
    private bool _isDashing;
    //---------------------------------
    
    //---Jump--------------------------
    [Header("Jump settings")] 
    [SerializeField] private float jumpForceOnGround = 400f; 
    [SerializeField] private float jumpForceOnAir = 400f;
    [SerializeField] private float jetForce = 0.05f;
    private bool _canUseJetpackBoost;
    private bool _canUseJetpack;
    private bool _isUsingJet;
    
    //---------------------------------
    
    //---Animator----------------------
    private Animator _animator;
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int HorSpeed = Animator.StringToHash("HorSpeed");
    private static readonly int IsRight = Animator.StringToHash("IsRight");
    private static readonly int IsUsingJet = Animator.StringToHash("IsUsingJet");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsHittingLaser = Animator.StringToHash("IsHittingLaser");

    //---------------------------------
    private bool _isShocked;
    private static readonly int Paradox = Animator.StringToHash("Paradox");

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        _isShocked = false;
        _canUseJetpackBoost = true;
        _animator = gameObject.GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        cooldownImageDash.fillAmount = 0;
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }
    
    private void OnDestroy()
    {
        //It is unsubscribing to the event
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
 
    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.Paradox ) 
        { 
            _animator.SetBool(Paradox, true);
        }
    }

    public void Move(float move, bool crouch, bool jump, bool dash)
    {
        if (!_isShocked)
        {
            var wasGrounded = m_Grounded;
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    if (!jump)
                        _canUseJetpackBoost = true;
                    if (!wasGrounded)
                        OnLandEvent.Invoke();
                }
            }

            switch (wasGrounded)
            {
                case true when !m_Grounded:
                    _animator.SetBool(IsFalling, true);
                    break;
                case false when m_Grounded:
                    _animator.SetBool(IsFalling, false);
                    break;
            }

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
                var velocity = m_Rigidbody2D.velocity;
                Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity,
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
            //--------------------------------

            //---Jump-------------------------
            //If the player is jumping off the ground for the first time
            if (m_Grounded && jump && !_isUsingJet && _canUseJetpackBoost)
            {
                _isUsingJet = true;
                _canUseJetpack = true;
                _canUseJetpackBoost = false;
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, jumpForceOnGround));
                //Play the jump sound-------
                _audioManager.Play("Jump");
                //---------------------------
                _audioManager.Play("Jetpack");
                //---------------------------
                _animator.SetBool(IsUsingJet, true);
                _animator.SetBool(IsFalling, true);
            }

            //If the player is jumping into the air for the first time
            if (!m_Grounded && jump && !_isUsingJet && _canUseJetpackBoost)
            {
                _isUsingJet = true;
                _canUseJetpack = true;
                _canUseJetpackBoost = false;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, jumpForceOnAir));
                //Play the jump sound-------
                _audioManager.Play("Jump");
                //---------------------------
                _audioManager.Play("Jetpack");
                //---------------------------
                _animator.SetBool(IsUsingJet, true);
            }

            //If the player is using the jetpack
            if (jump && _isUsingJet)
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, jetForce));
            }

            //If the player was using the jetpack, but is still in the air and can use it
            if (jump && !_isUsingJet && _canUseJetpack)
            {
                _isUsingJet = true;
                m_Rigidbody2D.AddForce(new Vector2(0f, jetForce));
                //---------------------------
                _audioManager.Play("Jetpack");
                //---------------------------
                _animator.SetBool(IsUsingJet, true);
            }

            //If the player stops using the jetpack in the air
            if (!jump && _isUsingJet && !m_Grounded)
            {
                _isUsingJet = false;
                _audioManager.Stop("Jetpack");
                _animator.SetBool(IsUsingJet, false);
            }

            //If the player hit the ground while using the jetpack
            if (_isUsingJet && m_Grounded)
            {
                _isUsingJet = false;
                _canUseJetpack = false;
                _audioManager.Stop("Jetpack");
                _animator.SetBool(IsUsingJet, false);
            }
            //--------------------------------

            //---Dash-------------------------
            if (dash && !_isDashing && !_isChargingDash)
            {
                _isDashing = true;
                StopCoroutine(nameof(DisableCooldownUI));
                cooldownImageDash.color = new Color(0, 1f, 0, 0.5f);
                cooldownImageDash.fillAmount = 1;

                _dashTime = dashTimeDuration;
                _animator.SetBool(IsDashing, true);
                //Play the dash sound-------
                _audioManager.Play("Dash");
                //---------------------------
            }

            if (dash && _isDashing)
            {
                if (_dashTime > 0)
                {
                    var dir = m_FacingRight ? 1 : -1;
                    var velocity = m_Rigidbody2D.velocity;
                    velocity = (new Vector2(dir * dashSpeed + velocity.x, velocity.y));
                    m_Rigidbody2D.velocity = velocity;
                    _dashTime -= Time.fixedDeltaTime;

                    cooldownImageDash.fillAmount = _dashTime / dashTimeDuration;
                    cooldownImageDash.color = new Color(1, _dashTime / dashTimeDuration, 0, 0.7f);

                }
                else
                {
                    _animator.SetBool(IsDashing, false);
                    _isDashing = false;
                    _isChargingDash = true;
                    _dashCooldownTime = 0;
                }
            }

            //If the player releases the dash key while dashing you will stop the boost
            if (!dash && _isDashing)
            {
                _animator.SetBool(IsDashing, false);
                _isDashing = false;
                _isChargingDash = true;
                _dashCooldownTime = (_dashTime / dashTimeDuration) * dashCooldownTimeDuration;
            }

            if (_isChargingDash)
            {
                if (_dashCooldownTime < dashCooldownTimeDuration)
                {
                    _dashCooldownTime += Time.fixedDeltaTime;
                    cooldownImageDash.fillAmount = _dashCooldownTime / dashCooldownTimeDuration;
                    cooldownImageDash.color = new Color(1, _dashCooldownTime / dashCooldownTimeDuration, 0, 0.7f);
                }
                else
                {
                    _isChargingDash = false;
                    cooldownImageDash.color = new Color(0, 1, 0, 0.7f);
                    StartCoroutine(nameof(DisableCooldownUI));
                }
            }
            //--------------------------------




            _animator.SetFloat(HorSpeed, Math.Abs(m_Rigidbody2D.velocity.x));
        }
    }

    private IEnumerator DisableCooldownUI()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        cooldownImageDash.color = new Color(0,1f,0,0f);
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
        GameManager.Instance.UpdateGameState(GameState.Paradox);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        _animator.SetBool(IsRight, m_FacingRight);

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        var localScale = uiCanvas.transform.localScale;
        localScale = new Vector3(-1*localScale.x, localScale.y, localScale.z);
        uiCanvas.transform.localScale = localScale;
        transform.localScale = theScale;
    }
    
    
}