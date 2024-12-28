using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryTest : MonoBehaviour
{
    [SerializeField] Text text;

    int currentNum = 0;
    int startNum = 0;
    int targetNum = 128420;

    float timeElapsed = 0;
    float duration = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Memory Test:   0K";
    }

    // Update is called once per frame
    void Update()
    {
        if(timeElapsed < duration){

            float t = timeElapsed / duration;
            currentNum = Mathf.FloorToInt(Mathf.Lerp(startNum, targetNum, t));
            timeElapsed += Time.deltaTime;

        }
        else{

            currentNum = targetNum;
        }

        text.text = currentNum == targetNum ? $"Memory Test:   {targetNum}K OK" : $"Memory Test:   {currentNum}K";
    }
}
