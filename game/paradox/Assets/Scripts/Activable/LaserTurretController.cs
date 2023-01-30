using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurretController : MonoBehaviour
{
    [SerializeField] private GameObject _laserHead;
    public LineRenderer lineRenderer;
    [SerializeField] private GameObject _laserRay;
    private Vector3 _direction;
    private bool _isActive;
    //private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _laserOutput;
    [SerializeField] private LaserDecoration _laserDecoration;
    [SerializeField] private LaserType _laserType;
    [SerializeField] private LaserFunctioning _functionYoung;
    [SerializeField] private LaserFunctioning _functionOldActive;
    [SerializeField] private float _intermittentPeriod = 2.0f;
    [Header("Moving laser configuration")] 
    [SerializeField] private GameObject[] _waypoints;
    [SerializeField] private float speed = 2.0f;
    private int _currentWaypointIndex = 0;
    private Vector3 _initPosition;
    private Vector3 _laserPosition;
    private bool _isMoving;
    [Header("Rotating laser configuration")] 
    [SerializeField] private float _angularSpeed = 1f;
    [SerializeField] private float _rotationArc = 90f;
    [SerializeField] private bool _continuousRotation=false;
    private bool _clockwiseRotation=false;
    
    private bool _isRotating;
    private Vector3 _initDirection;
    private Vector3 _laserDirection;

    [Header("Initial state (Young/Old phase)")] 
    [SerializeField] private LaserState _initYoungState;
    [SerializeField] private LaserState _initOldStateOFF;
    
    
    [Header("Laser component")]
    [SerializeField] private GameObject _laserFoot;
    [SerializeField] private GameObject _laserWing;
    [Header("Laser sprites")]
    [SerializeField] private Sprite _spriteOff;
    [SerializeField] private Sprite _spriteOn1;
    [SerializeField] private Sprite _spriteOn2;
    private int _sprintOn;
    private SpriteRenderer _spriteRenderer;
    private float _fpsCount=0;
    private float _spriteSpeed=0.1f;

    private bool _isFirstHitYoung = true;
    private bool _isFirstHitOld = true;
    private Vector3 _initHitPointYoung;
    private Vector3 _initHitPointOld;

    private RaycastHit2D _raycastHit2D;
    //-------------------------------
    private void Awake()
    {
        _spriteRenderer = _laserHead.GetComponentInChildren<SpriteRenderer>();
        _direction = (_laserOutput.position - _laserHead.transform.position).normalized;
        _initDirection = _laserHead.transform.rotation.eulerAngles;
        //Moving LASER
        _isMoving = false; 
        _initPosition = _laserHead.transform.position;
        _laserPosition = _initPosition;
        SetLaserOff();
        switch (_laserDecoration)
        {
            case LaserDecoration.Foot:
                _laserFoot.SetActive(true);
                _laserWing.SetActive(false);
                break;
            case LaserDecoration.Wing:
                _laserFoot.SetActive(false);
                _laserWing.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        //------------------------------------
        //It is subscribing to the event
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    private void OnDestroy()
    {
        //It is unsubscribing to the event
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.StartingYoungTurn)
        {
            InitYoung();
        }

        if (state == GameState.StartingOldTurn)
        {
            InitOld();
        }
    }

    private void InitYoung()
    {
        _laserPosition = _initPosition;
        _laserHead.transform.position = _laserPosition;

        _laserHead.transform.rotation = Quaternion.Euler(_initDirection);
        _clockwiseRotation = false;
        _currentWaypointIndex = 0;
        CancelInvoke();
        if (_laserType==LaserType.Moving||_laserType==LaserType.MovingRotating)
        {
            _isMoving = true;
        }
        if (_laserType==LaserType.Rotating||_laserType==LaserType.MovingRotating)
        {
            _isRotating = true;
        }
        if (_initYoungState == LaserState.Active)
        {
            _isActive = true;
            SetLaserOn();
            if (_functionYoung == LaserFunctioning.Intermittent)
            {
                InvokeRepeating("Intermittent", _intermittentPeriod/2, _intermittentPeriod/2);
            }
        }
        else if (_initYoungState == LaserState.Inactive)
        {
            _isActive = false;
            SetLaserOff();
        }
        if (_isActive)
        {
            _direction = (_laserOutput.position - _laserHead.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_laserOutput.position, _direction);
            lineRenderer.SetPosition(0, _laserOutput.position);
            if (_isFirstHitYoung)
            {
                _isFirstHitYoung = false;
                if (hit)
                {
                    lineRenderer.SetPosition(1, hit.point);
                    _initHitPointYoung = hit.point;
                }
                else
                {
                    lineRenderer.SetPosition(1, _direction * 100);
                }
            }
            else
            {
                lineRenderer.SetPosition(1, _initHitPointYoung);
            }
            
        }
    }

    private void InitOld()
    {
        _laserHead.transform.rotation = Quaternion.Euler(_initDirection);
        _clockwiseRotation = false;
        _laserPosition = _initPosition;
        _laserHead.transform.position = _laserPosition;

        _currentWaypointIndex = 0;
        CancelInvoke();
        if (_laserType==LaserType.Moving)
        {
            _isMoving = true;
        }
        if (_laserType==LaserType.Rotating)
        {
            _isRotating = true;
        }
        if (_initOldStateOFF == LaserState.Active)
        {
            _isActive = true;
            SetLaserOn();
            if (_functionOldActive == LaserFunctioning.Intermittent)
            {
                InvokeRepeating("Intermittent", _intermittentPeriod/2, _intermittentPeriod / 2);
            }
            
        }
        else if (_initOldStateOFF == LaserState.Inactive)
        {
            _isActive = false;
            SetLaserOff();
        }
        if (_isActive)
        {
            _direction = (_laserOutput.position - _laserHead.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(_laserOutput.position, _direction);
            lineRenderer.SetPosition(0, _laserOutput.position);
            if (_isFirstHitOld)
            {
                _isFirstHitOld = false;
                if (hit)
                {
                    lineRenderer.SetPosition(1, hit.point);
                    _initHitPointOld = hit.point;
                }
                else
                {
                    lineRenderer.SetPosition(1, _direction * 100);
                }
            }
            else
            {
                lineRenderer.SetPosition(1, _initHitPointOld);
            }
            
        }
    }
    //-------------------------------


    void Update()
    {
       
        if (_isActive)
        {
            //Animation
            _fpsCount += Time.deltaTime;
            if (_fpsCount > _spriteSpeed)
            {
                _fpsCount = 0;
                if (_sprintOn==1)
                {
                    _sprintOn = 2;
                    _spriteRenderer.sprite = _spriteOn2;
                }
                else if (_sprintOn==2)
                {
                    _sprintOn = 1;
                    _spriteRenderer.sprite = _spriteOn1;
                }
            }
            //---------
            
            
            
           
            
        }
       
    }
    private void FixedUpdate()
    {
        _laserHead.transform.position = _laserPosition;
        
        if (Vector2.Distance(_waypoints[_currentWaypointIndex].transform.position, _laserPosition) < .1f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }
        }

        if (_isMoving&& GameManager.Instance.State!=GameState.StartingYoungTurn&& GameManager.Instance.State!=GameState.StartingOldTurn)
        {
            _laserPosition = Vector2.MoveTowards(_laserPosition,
                _waypoints[_currentWaypointIndex].transform.position, Time.fixedDeltaTime * speed);
        }

        if (!_continuousRotation)
        {
            if (Math.Abs(_initDirection.z+_rotationArc/2)-(_laserHead.transform.rotation.eulerAngles.z) <.1f||
                        Math.Abs(_initDirection.z-_rotationArc/2)-(_laserHead.transform.rotation.eulerAngles.z) >.1f
                       )
            { 
                _clockwiseRotation = !_clockwiseRotation; 
            }
        }
        
        if (_isRotating&& GameManager.Instance.State!=GameState.StartingYoungTurn&& GameManager.Instance.State!=GameState.StartingOldTurn)
        {
            if(_clockwiseRotation)
                _laserHead.transform.Rotate(new Vector3(0,0,Time.fixedDeltaTime - _angularSpeed));
            else
                _laserHead.transform.Rotate(new Vector3(0,0,Time.fixedDeltaTime + _angularSpeed));
        }
        _direction = (_laserOutput.position - _laserHead.transform.position).normalized;
        _raycastHit2D = Physics2D.Raycast(_laserOutput.position, _direction);
        if (_isActive)
        {
            lineRenderer.SetPosition(0, _laserOutput.position);
            if (_raycastHit2D)
            {
                lineRenderer.SetPosition(1, _raycastHit2D.point);
                CheckCollision(_raycastHit2D.collider);
            }
            else
            {
                lineRenderer.SetPosition(1, _direction * 100);
            }
        }
        
    }
    

    // Check if the laser ray is hitting the player/ghost
    private void CheckCollision(Collider2D col)
    {
        if (_laserRay.activeSelf && GameManager.Instance.State!=GameState.Paradox&&GameManager.Instance.State!=GameState.Paradox&&GameManager.Instance.State!=GameState.StartingOldTurn&&GameManager.Instance.State!=GameState.StartingYoungTurn)
        {
            if (col.gameObject.CompareTag("Young"))
            {
                col.gameObject.GetComponent<CharacterController2D>().SetShocked();
            }
            else if (col.gameObject.CompareTag("Old"))
            {
                col.gameObject.GetComponent<OldController2D>().SetShocked();

            }
            else if ( col.gameObject.CompareTag("Ghost"))
            {
                GameManager.Instance.UpdateGameState(GameState.Paradox);
            }
        }
    }

    
    
    private void Intermittent()
    {
        if (GameManager.Instance.State is not GameState.StartingYoungTurn or GameState.StartingSecondPart or GameState.StartingThirdPart or GameState.StartingOldTurn)
        {
            _isActive = !_isActive;
            if (_isActive)
            {
                SetLaserOn();
            }
            else
            {
                SetLaserOff();
            }
            
        }
        
    }
    public void SwitchState()
    {
        if (_functionOldActive==LaserFunctioning.Fixed)
        {
            if (_isActive)
            {
                _isActive = false;
                SetLaserOff();
            }
            else
            {
                _isActive = true;
                SetLaserOn();
            }
            
        }
        else if (_functionOldActive==LaserFunctioning.Intermittent)
        {
            if (_isActive)
            {
                _isActive = false;
                CancelInvoke();
                SetLaserOff();
            }
            else
            {
                _isActive = true;
                SetLaserOn();
                InvokeRepeating("Intermittent", _intermittentPeriod/2, _intermittentPeriod / 2);
            }
        }
    }


    
    private void SetLaserOn()
    {
        _spriteRenderer.sprite = _spriteOn1;
        _sprintOn = 1;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, _laserOutput.position);
        lineRenderer.SetPosition(1, _raycastHit2D.point);


    }

    private void SetLaserOff()
    {
        _spriteRenderer.sprite = _spriteOff;
        lineRenderer.enabled = false;

    }
    
}

[System.Serializable]
public enum LaserState
{
    Inactive,
    Active
}

[System.Serializable]
public enum LaserFunctioning
{
    Intermittent,
    Fixed
}

public enum LaserDecoration
{
    Foot,
    Wing
}

[System.Serializable]
public enum LaserType
{
    Static,
    Rotating,
    Moving,
    MovingRotating
}

