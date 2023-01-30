using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    private GameObject _pauseMenu;
    private GameObject _statisticsMenuR;
    private GameObject _statisticsMenuL;
    private GameObject _nextButton;
    private GameObject _stars;
    private GameObject _resumeButton;
    private TMP_Text _paradoxText;
    private TMP_Text _retryText;
    private TMP_Text _overallTimeText;
    private PlayerInputactions _actions;


    //public static bool isPaused; //it can be used in other scripts to stop key functioning.
    private void Awake()
    {
        _actions = new PlayerInputactions();

        _pauseMenu = GameObject.Find("Canvases").transform.GetChild(1).GetChild(0).gameObject;
        _statisticsMenuL = GameObject.Find("Door").transform.GetChild(0).GetChild(0).gameObject;
        _statisticsMenuR = GameObject.Find("Door").transform.GetChild(1).GetChild(0).gameObject;

        _stars = _statisticsMenuL.transform.GetChild(1).gameObject;
        _paradoxText = _statisticsMenuL.transform.GetChild(3).gameObject.GetComponent<TMP_Text>();
        _retryText = _statisticsMenuL.transform.GetChild(4).gameObject.GetComponent<TMP_Text>();
        _overallTimeText = _statisticsMenuL.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
        _nextButton = _statisticsMenuR.transform.GetChild(1).GetChild(0).gameObject;
        _resumeButton = _pauseMenu.transform.GetChild(1).GetChild(0).gameObject;

        //It is subscribing to the event
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        _pauseMenu.SetActive(false);
        _statisticsMenuL.SetActive(false);
        _statisticsMenuR.SetActive(false);

    }

    private void OnEnable()
    {
        _actions.UI.Enable();
        _actions.YoungPlayer.Enable();
        _actions.OldPlayer.Enable();
        _actions.UI.Pause.performed += PausePerformed;
        _actions.YoungPlayer.Restart.performed += YoungRestartPerformed;
        _actions.OldPlayer.Restart.performed += OldRestartPerformed;
    }

    private void OldRestartPerformed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsPlayablePhase())
        {
            GameManager.Instance.UpdateGameState(GameState.StartingYoungTurn);
        }
    }

    private void YoungRestartPerformed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsPlayablePhase())
        {
            GameManager.Instance.UpdateGameState(GameState.StartingYoungTurn);
        }
    }

    private void PausePerformed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsPlayablePhase() || GameManager.Instance.State==GameState.PauseMenu||GameManager.Instance.State==GameState.StartingYoungTurn||GameManager.Instance.State==GameState.StartingOldTurn)
        {
            GameManager.Instance.TriggerMenu();
        }
    }


    private void OnDisable()
    {
        _actions.UI.Enable();
        _actions.YoungPlayer.Disable();
        _actions.OldPlayer.Disable();
        _actions.UI.Pause.performed -= PausePerformed;
        _actions.YoungPlayer.Restart.performed -= YoungRestartPerformed;
        _actions.OldPlayer.Restart.performed -= OldRestartPerformed;
    }

    private void OnDestroy()
    {
        //It is unsubscribing to the event
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;

    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.PauseMenu)
        {
            //Clear
            EventSystem.current.SetSelectedGameObject(null);
            //Reassign
            EventSystem.current.SetSelectedGameObject(_resumeButton);

            _pauseMenu.SetActive(true);
            _statisticsMenuL.SetActive(false);
            _statisticsMenuR.SetActive(false);
        }
        else if (state == GameState.LevelCompleted)
        {
            //Clear
            EventSystem.current.SetSelectedGameObject(null);
            //Reassign
            EventSystem.current.SetSelectedGameObject(_nextButton);

            _pauseMenu.SetActive(false);
            _statisticsMenuL.SetActive(true);
            _statisticsMenuR.SetActive(true);
        }
        else if (state == GameState.StatisticsMenu)
        {
            //Clear
            EventSystem.current.SetSelectedGameObject(null);
            //Reassign
            EventSystem.current.SetSelectedGameObject(_nextButton);
            _pauseMenu.SetActive(false);
            _statisticsMenuL.SetActive(true);
            _statisticsMenuR.SetActive(true);
        }
        else
        {
            _pauseMenu.SetActive(false);
            _statisticsMenuL.SetActive(false);
            _statisticsMenuR.SetActive(false);
        }
    }

    /*
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            GameManager.Instance.TriggerMenu();
        }
        else if (Input.GetButtonDown("Restart"))
        {
            GameManager.Instance.UpdateGameState(GameState.StartingYoungTurn);
        }
    }
    */

    public void ResumeGame()
    {
        GameManager.Instance.TriggerMenu();
    }

    public void RestartLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.StartingYoungTurn);
    }

    public void GoToMainMenu()
    {
        LevelManager l = LevelManager.Instance;
        if (l)
        {
            l.PlayMainMenu();
        }
        else
        {
            Debug.LogWarning("LevelManager not found!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartDoor()
    {
        LevelManager.Instance.RestartLevel();
    }
    public void NextLevelDoor()
    {
        GameManager.Instance.UpdateGameState(GameState.NextLevel);
    }

    public void setStatistics(int _counterRetry, int _counterParadoxOverall, TimeSpan _overallTime, int _numStars)
    {
        if (_numStars > 0)
        {
            GameObject activeStar = _stars.transform.GetChild(_numStars).gameObject;
            activeStar.SetActive(true);
        }

        _paradoxText.text = "" + _counterParadoxOverall;
        _retryText.text = "" + _counterRetry;
        _overallTimeText.text = "" + _overallTime.ToString(@"mm\:ss\:ff");

    }

}
