using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlBtn : MonoBehaviour
{

    Stats stat;

    void Awake(){

        stat = Component.FindObjectOfType<Stats>();
    }


    void OnMouseOver(){

        if(Input.GetMouseButtonDown(0) && stat.lvl < 99){

            stat.lvl++;
        }
        if(Input.GetMouseButtonDown(1) && stat.lvl > 1){

            stat.lvl--;
        }
    }
}
