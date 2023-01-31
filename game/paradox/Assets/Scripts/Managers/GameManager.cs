using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public GameState PreviousGameState;


    public static event Action<GameState> OnGameStateChanged;

    private bool _isTutorial;
    private bool _isLoaded;


    private void Awake()
    {
        Instance = this;
        var l = LevelManager.Instance;
        _isTutorial = l && l.IsTutorialLevel();
    }

    private void Start()
    {
        StartCoroutine(nameof(WaitToStart));
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    /*
     * Call this function with:
     * GameManager.Instance.UpdateGameState(GameState.YoungPlayerTurn);
     */
    public void UpdateGameState(GameState newState)
    {
        if (!_isLoaded&&newState!=GameState.StartingYoungTurn)
        {
            return;
        }

        _isLoaded = true;
        if (State == GameState.Paradox)
        {
            if (newState!=GameState.StartingOldTurn)
            {
                return;
            }
        }
        if (State == GameState.StartingOldTurn)
        {
            if (newState==GameState.Paradox)
            {
                return;
            }
        }
        if (State == GameState.LevelCompleted)
        {
            if (newState!=GameState.StatisticsMenu)
            {
                return;
            }
        }
        if (State == GameState.StatisticsMenu)
        {
            if (newState!=GameState.NextLevel)
            {
                return;
            }
        }
        if (State == GameState.NextLevel)
        {
            return;
        }
        Debug.Log("Current State: " + newState + " ----- IsTutorial: " + _isTutorial);

        PreviousGameState = State;
        State = newState;

        if (_isTutorial)
        {
            switch (newState)
            {
                case GameState.StartingYoungTurn:
                    Time.timeScale = 0f;
                    break;
                case GameState.YoungPlayerTurn:
                    Time.timeScale = 1f;
                    break;
                case GameState.StartingSecondPart:
                    if (PreviousGameState != GameState.ThirdPart)
                    {
                        UpdateGameState(GameState.SecondPart);
                    }
                    break;
                case GameState.SecondPart:
                    Time.timeScale = 1f;
                    break;
                case GameState.StartingThirdPart:
                    GameObject.Find("DisappearingPlatform 0").GetComponent<ActivableController>().SwitchState();
                    UpdateGameState(GameState.ThirdPart);
                    break;
                case GameState.ThirdPart:
                    break;
                case GameState.StartingOldTurn:
                    //Time.timeScale = 0f;
                    break;
                case GameState.OldPlayerTurn:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paradox:
                    break;
                case GameState.PauseMenu:
                    break;
                case GameState.GameOverMenu:
                    break;
                case GameState.LevelCompleted:
                    Time.timeScale = 0f;
                    GameObject.Find("Door").GetComponent<Animator>().SetTrigger("Close");
                    StartCoroutine("WaitToCloseDoor");
                    break;
                case GameState.StatisticsMenu:
                    Time.timeScale = 0f;
                    break;
                case GameState.NextLevel:
                    Time.timeScale = 0f;
                    LevelManager.Instance.PlayNextLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
        else
        {
            switch (newState)
            {
                case GameState.StartingYoungTurn:

                    Time.timeScale = 0f;
                    break;
                case GameState.YoungPlayerTurn:
                    Time.timeScale = 1f;
                    break;
                case GameState.StartingOldTurn:
                    break;
                case GameState.OldPlayerTurn:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paradox:
                    break;
                case GameState.PauseMenu:
                    break;
                case GameState.GameOverMenu:
                    break;
                case GameState.LevelCompleted:
                    Time.timeScale = 0f;
                    GameObject.Find("Door").GetComponent<Animator>().SetTrigger("Close");
                    StartCoroutine("WaitToCloseDoor");
                    break;
                case GameState.StatisticsMenu:
                    Time.timeScale = 0f;
                    break;
                case GameState.NextLevel:
                    Time.timeScale = 0f;
                    LevelManager.Instance.PlayNextLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void TriggerMenu()
    {
        if (State == GameState.PauseMenu)
        {
            UpdateGameState(PreviousGameState);
            Time.timeScale = 1f;
        }
        else if ((State == GameState.StartingYoungTurn||State == GameState.YoungPlayerTurn|| State == GameState.StartingOldTurn|| State == GameState.OldPlayerTurn || State == GameState.SecondPart || State == GameState.ThirdPart)&&!PlayerTransitionManager.Instance.isProcessing)
        {
            UpdateGameState(GameState.PauseMenu);
            Time.timeScale = 0f;
        }
    }


    public bool IsTutorial()
    {
        return _isTutorial;
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        UpdateGameState(GameState.StartingYoungTurn);
        LevelManager.Instance.GetStats(this.GetComponent<Statistics>());
    }
    IEnumerator WaitToCloseDoor()
    {
        yield return new WaitForSecondsRealtime(1f);
        UpdateGameState(GameState.StatisticsMenu);
    }
    
    public bool IsPlayablePhase()
    {
        if (State is GameState.YoungPlayerTurn or GameState.SecondPart or GameState.ThirdPart or GameState.OldPlayerTurn)
        {
            return true;
        }
        return false;
    }



}


public enum GameState
{
    
    StartingYoungTurn,
    YoungPlayerTurn,

    StartingSecondPart,
    SecondPart,

    StartingThirdPart,
    ThirdPart,

    StartingOldTurn,
    OldPlayerTurn,

    Paradox,

    PauseMenu,
    GameOverMenu,
    LevelCompleted,
    StatisticsMenu,
    NextLevel
}



