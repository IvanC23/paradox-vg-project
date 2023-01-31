using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Transform))]
public class CameraShakeScript : MonoBehaviour
{
    
    public Transform cameraTransform = default;
    private Vector3 _orignalPosOfCam = default;
    private float shakeFrequency = 0.25f;
    private bool shake = false;

    // Start is called before the first frame update
    void Start()
    { 
        //When the game starts make sure to assign the origianl possition of the camera, to its current
        //position, supposedly it is where you want the camera to return after shaking.
        _orignalPosOfCam = cameraTransform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        if(shake)
            CameraShake();
        else
            StopShake();
        
    }

    private void CameraShake()
    {
        cameraTransform.position = _orignalPosOfCam + Random.insideUnitSphere * shakeFrequency;
    }

    private void StopShake()
    {
        //Return the camera to it's original position.
        cameraTransform.position = _orignalPosOfCam;
    }
    
    public void setShakeTrue(){
        shake = true;
    }
    public void setShakeFalse(){
        shake = false;
    }

}
