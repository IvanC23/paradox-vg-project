using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.instance.GetColorBlindMode())
        {
            gameObject.GetComponent<SpriteRenderer>().enabled=true;
        }
    }

}
