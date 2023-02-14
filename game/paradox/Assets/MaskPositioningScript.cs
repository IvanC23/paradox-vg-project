using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPositioningScript : MonoBehaviour
{
    private GameObject OldPlayerPos;
    void Awake(){
        OldPlayerPos = GameObject.Find("Old_Position");
        //transform.localPosition = OldPlayerPos.transform.localPosition;
        transform.position = OldPlayerPos.transform.position;
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
