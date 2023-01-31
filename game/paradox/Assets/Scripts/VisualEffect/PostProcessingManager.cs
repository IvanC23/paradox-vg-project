using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance;
    //[SerializeField] private ScreenSize ourScreenSize;
    public GameObject Camera;
    //private GameObject Old_Player_Position;

    private Vignette _vignette;
    private ColorGrading _colorGrading;
    private Vector2 normalizedOldPlayerPos;
    private bool upIntensityDone = false;
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
        /*
        if (GameManager.Instance.IsTutorial())
        {
            if (state == GameState.StartingSecondPart && GameManager.Instance.PreviousGameState == GameState.YoungPlayerTurn)
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
        */

        if (state == GameState.Paradox)
        {
            Camera.GetComponent<CameraShakeScript>().setShakeTrue();
            _colorGrading.active = true;
        }
        else
        {
            Camera.GetComponent<CameraShakeScript>().setShakeFalse();
            _colorGrading.active = false;
        }
    }

    private void Start()
    {
        //Old_Player_Position = GameObject.Find("Old_Position");
        //normalizedOldPlayerPos = NormPosFromZeroToOne(new Vector2(Old_Player_Position.transform.position.x, Old_Player_Position.transform.position.y));
        Camera.GetComponent<PostProcessVolume>().profile.TryGetSettings(out _vignette);
        Camera.GetComponent<PostProcessVolume>().profile.TryGetSettings(out _colorGrading);

        //Wanted to center the camera on the old player but doesn't work in this way
        _vignette.center.value = normalizedOldPlayerPos;
    }



    //COROUTINE TO ADD A TRANSACTION BETWEEN FIRST AND SECOND PART
    /*
    IEnumerator StartDelay()
    {
        isProcessing = true;
        Time.timeScale = 0;

        while (_vignette.intensity.value >= 0f)
        {

            if (_vignette.intensity.value <= 1.5f && !upIntensityDone)
            {
                _vignette.intensity.value += 1.5f * Time.unscaledDeltaTime;
            }
            else
            {
                upIntensityDone = true;
                _vignette.intensity.value -= 1.5f * Time.unscaledDeltaTime;
            }


            yield return 0;
        }

        upIntensityDone = false;
        _vignette.intensity.value = 0f;

        isProcessing = false;
        //Time.timeScale = 1;

        if (GameManager.Instance.IsTutorial())
        {
            GameManager.Instance.UpdateGameState(GameState.SecondPart);
        }
        else
        {
            GameManager.Instance.UpdateGameState(GameState.OldPlayerTurn);
        }



    }
    */
    /*
    public Vector2 NormPosFromZeroToOne(Vector2 input)
    {
        //INFO ON HOW TO CALCULATE DIMENSIONS
        //Place a square on you screen and put it at the boards of the screen

        switch (ourScreenSize)
        {
            case ScreenSize.Small:
                input.x = Mathf.InverseLerp(-9f, 9f, input.x);
                input.y = Mathf.InverseLerp(-5f, 5f, input.y);
                break;
            case ScreenSize.Medium:
                input.x = Mathf.InverseLerp(-12.6f, 12.6f, input.x);
                input.y = Mathf.InverseLerp(-7f, 7f, input.y);
                break;
            case ScreenSize.Large:
                input.x = Mathf.InverseLerp(-17.8f, 17.8f, input.x);
                input.y = Mathf.InverseLerp(-10f, 10f, input.y);
                break;
        }
        return input;
    }

    [System.Serializable]
    public enum ScreenSize
    {
        Small,
        Medium,
        Large
    }
    */
}
