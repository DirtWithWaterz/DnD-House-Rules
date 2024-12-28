using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Description : MonoBehaviour
{

    [SerializeField]
    TMP_Text displayStatus;
    [SerializeField]
    TMP_Text displayHealth;
    [SerializeField]
    TMP_Text displayAc;
    [SerializeField]
    TMP_Text displayCondition;

    public string status;
    public int health;
    public int ac;
    public string condition;


    string oldStatus;
    int oldHealth;
    int oldAc;
    string oldCondition;

    void Start(){

        this.gameObject.SetActive(false);
    }

    void Update(){

        if(status != oldStatus){

            displayStatus.text = $"Status: {status}";

            oldStatus = status;
        }
        if(health != oldHealth){

            displayHealth.text = $"Health: {health}";

            oldHealth = health;
        }
        if(ac != oldAc){

            displayAc.text = $"AC: {ac}";

            oldAc = ac;
        }
        if(condition != oldCondition){

            displayCondition.text = condition;

            oldCondition = condition;
        }
    }
}
