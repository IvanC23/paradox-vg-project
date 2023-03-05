using System;
using UnityEngine;
public class MobileButtonManager : MonoBehaviour
{
    private bool _isAndroid=false;
    private void Awake()
    {
        //It is subscribing to the event
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        InputManager.OnChangedInputDevice += InputManagerOnChangedInputDevice;

        for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            _isAndroid = true;
            
           for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        }
    }
    private void InputManagerOnChangedInputDevice(DeviceUsed obj)
    {


        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            _isAndroid = true;
           for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        }
        else
        {
            _isAndroid = false;
            for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
        }



    }


    private void GameManagerOnGameStateChanged(GameState state)
    {
        if ((GameManager.Instance.IsPlayablePhase() || state == GameState.StartingYoungTurn || state == GameState.StartingOldTurn) && _isAndroid)
        {
           for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        }
        else
        {
            for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
        }
    }

    private void OnDestroy()
    {
        //It is unsubscribing to the event
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
        InputManager.OnChangedInputDevice -= InputManagerOnChangedInputDevice;

    }

}
