using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{

     private int _currentWaypointIndex = 0;
     private Vector3 _initPosition;
     private Vector3 _platformPosition;
     private bool _isMoving;
     private bool _isActive;
     [SerializeField]private GameObject _platform;
     [SerializeField]private GameObject _wing;
     [SerializeField] private GameObject[] _waypoints;
     [SerializeField] private MovingFunctioning _function;
     [SerializeField] private float speed = 2.0f;
     [Header("Initial state (Young/Old phase)")]
     [SerializeField] private MovingState _initYoungState;
     [SerializeField] private MovingState _initOldStateOFF;

     [Header("Sprites")]
     [SerializeField] private Sprite _spriteOffDisappearedIfInactive;
     [SerializeField] private Sprite _spriteOnDisappearedIfInactive;
     [SerializeField] private Sprite _spriteStoppedIfInactive;
     private SpriteRenderer _spriteRenderer;
     private SpriteRenderer _wingState;
     private Collider2D _collider2D;
     private Collider2D _colliderSticky;


     //-------------------------------
     private void Awake()
     {
         _spriteRenderer=_platform.GetComponent<SpriteRenderer>();
         _collider2D = _platform.GetComponents<Collider2D>()[0];
         _colliderSticky = _platform.GetComponents<Collider2D>()[1];
         _wingState = _wing.GetComponent<SpriteRenderer>();

         if (_function==MovingFunctioning.DisappearedIfInactive)
         {
             _spriteRenderer.sprite = _spriteOnDisappearedIfInactive;
         }
         else if (_function == MovingFunctioning.StoppedIfInactive)
         {
             _spriteRenderer.sprite = _spriteStoppedIfInactive;
         }

         _isMoving = false;
         _initPosition = _platform.transform.position;
         _platformPosition = _initPosition;
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
         _platformPosition = _initPosition;
         _currentWaypointIndex = 0;
         _platform.transform.position = _initPosition;
         if (_initYoungState == MovingState.Active)
         {
             if (_function==MovingFunctioning.DisappearedIfInactive)
             {
                 SetActive();
             }
             else if (_function==MovingFunctioning.StoppedIfInactive)
             {
                 _spriteRenderer.sprite = _spriteStoppedIfInactive;
             }
             _isMoving = true;
         }
         else if (_initYoungState==MovingState.Inactive)
         {
             if (_function==MovingFunctioning.DisappearedIfInactive)
             {
                 SetInactive();
                 _isMoving = true;
             }
             else if (_function==MovingFunctioning.StoppedIfInactive)
             {
                 _spriteRenderer.sprite = _spriteStoppedIfInactive;
                 _isMoving = false;
             }
         }
     }
     private void InitOld()
     {
         _currentWaypointIndex = 0;
         _platformPosition = _initPosition;
         _platform.transform.position = _initPosition;
         if (_initOldStateOFF == MovingState.Active)
         {
             if (_function==MovingFunctioning.DisappearedIfInactive)
             {
                 SetActive();
             }
             else if (_function==MovingFunctioning.StoppedIfInactive)
             {
                 _spriteRenderer.sprite = _spriteStoppedIfInactive;
             }
             _isMoving = true;
         }
         else if (_initOldStateOFF==MovingState.Inactive)
         {
             if (_function==MovingFunctioning.DisappearedIfInactive)
             {
                 SetInactive();
                 _isMoving = true;
             }
             else if (_function==MovingFunctioning.StoppedIfInactive)
             {
                 _spriteRenderer.sprite = _spriteStoppedIfInactive;
                 _isMoving = false;
             }
         }
     }
     //-------------------------------

     private void FixedUpdate()
     {
          if (Vector2.Distance(_waypoints[_currentWaypointIndex].transform.position, _platformPosition) < .1f)
          {
               _currentWaypointIndex++;
               if (_currentWaypointIndex >= _waypoints.Length)
               {
                    _currentWaypointIndex = 0;
               }
          }

          if (_isMoving&& GameManager.Instance.State!=GameState.StartingYoungTurn&& GameManager.Instance.State!=GameState.StartingOldTurn)
          {
              _platformPosition = Vector2.MoveTowards(_platformPosition,
                    _waypoints[_currentWaypointIndex].transform.position, Time.fixedDeltaTime * speed);
          }
           _platform.transform.position = _platformPosition;
     }



     public void SwitchState()
     {
         if (_function==MovingFunctioning.DisappearedIfInactive)
         {
             if (_isActive)
             {
                 SetInactive();
             }
             else
             {
                 SetActive();
             }
             _isMoving = true;
         }
         else if (_function==MovingFunctioning.StoppedIfInactive)
         {
             _isMoving = !_isMoving;
         }
     }


     private void SetActive()
     {
         _spriteRenderer.sprite = _spriteOnDisappearedIfInactive;
         _collider2D.enabled = true;
         _colliderSticky.enabled = true;
         _isActive = true;
         _wingState.enabled = true;
     }
     private void SetInactive()
     {
         _spriteRenderer.sprite = _spriteOffDisappearedIfInactive;
         _collider2D.enabled = false;
         _colliderSticky.enabled = false;
         _isActive = false;
         _wingState.enabled = false;
     }
}

[System.Serializable]
public enum MovingState
{
    Inactive,
    Active
}
[System.Serializable]
public enum MovingFunctioning
{
    StoppedIfInactive,
    DisappearedIfInactive
}
