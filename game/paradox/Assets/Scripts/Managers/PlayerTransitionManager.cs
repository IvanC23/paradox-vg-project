using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransitionManager : MonoBehaviour
{
    public static PlayerTransitionManager Instance;

    [SerializeField] private GameObject _playerTransition;
    public bool isProcessing = false;

    private void Awake()
    {
        Instance = this;
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
        if (GameManager.Instance.IsTutorial())
        {
            if (state == GameState.StartingSecondPart && GameManager.Instance.PreviousGameState == GameState.ThirdPart)
            {
                StartCoroutine("StartDelay");
            }

        }
        else
        {
            if (state == GameState.StartingOldTurn && GameManager.Instance.PreviousGameState == GameState.YoungPlayerTurn)
            {
                StartCoroutine("StartDelay");
            }

        }
        
    }
    IEnumerator StartDelay()
    {
        isProcessing = true;
        Time.timeScale = 0;
        _playerTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(2.4f);
        _playerTransition.SetActive(false);
        isProcessing = false;
        //Time.timeScale = 1;

        if (GameManager.Instance.IsTutorial())
        {
            GameManager.Instance.UpdateGameState(GameState.SecondPart);
        }
        else
        {
            //GameManager.Instance.UpdateGameState(GameState.OldPlayerTurn);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
