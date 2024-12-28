using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeUSB : MonoBehaviour
{
    
    [SerializeField] Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeUSBControllers());
    }

    private IEnumerator InitializeUSBControllers()
    {
        yield return new WaitForSeconds(0.3f);
        text.text = text.text + "done.";
    }
}
