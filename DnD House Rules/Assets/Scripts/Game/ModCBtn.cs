using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;

public enum Mod{

    NUL = 0,
    STR = 1,
    DEX = 2,
    CON = 3,
    INT = 4,
    WIS = 5,
    CHA = 6,
    WALK = 7,
    INIT = 8,
    ARMOR = 9
}

public class ModCBtn : MonoBehaviour
{

    int val = 0;
    TMP_Text display;

    [SerializeField]
    Mod mod = Mod.NUL;
    Stats stat;
    Health health;


    User user;

    void Awake(){

        display = this.GetComponentInChildren<TMP_Text>();
        stat = transform.root.GetComponentInChildren<Stats>();
        health = transform.root.GetComponentInChildren<Health>();

        if (mod != Mod.WALK && mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = (val >= 0 ? "+" : "") + val.ToString();
        else if (mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = $"BASE: {44 + val}";

        if (mod == Mod.INIT || mod == Mod.ARMOR)
            display.gameObject.SetActive(false);

        user = transform.root.GetComponent<User>();

        
    }

    void Update(){

        switch(mod){

            case Mod.STR:
                if(stat.STR.Value != val){

                    val = stat.STR.Value;
                }
                break;
            case Mod.DEX:
                if(stat.DEX.Value != val){

                    val = stat.DEX.Value;
                    health.SetInitialValuesDexRpc();
                    health.CalculateValues();
                }
                break;
            case Mod.CON:
                if(stat.CON.Value != val){

                    val = stat.CON.Value;
                    health.SetInitialValuesRpc();
                    health.CalculateValues();
                    health.maxHP = health.currHP;
                }
                break;
            case Mod.INT:
                if(stat.INT.Value != val){

                    val = stat.INT.Value;
                }
                break;
            case Mod.WIS:
                if(stat.WIS.Value != val){

                    val = stat.WIS.Value;
                }
                break;
            case Mod.CHA:
                if(stat.CHA.Value != val){

                    val = stat.CHA.Value;
                }
                break;
            case Mod.WALK:
                if(stat.BASE_SPEED.Value != 44 + val){

                    val = stat.BASE_SPEED.Value - 44;
                }
                break;
        }
        if (mod != Mod.WALK && mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = (val >= 0 ? "+" : "") + val.ToString();
        else if (mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = $"BASE: {44 + val}";
        if (mod == Mod.INIT){
            if (stat.addProf2Init.Value)
                display.gameObject.SetActive(true);
            else
                display.gameObject.SetActive(false);
        }
        if (mod == Mod.ARMOR){
            if(stat.barbarian.Value){

                display.gameObject.SetActive(true);
                health.SetInitialValuesDexRpc();
            }
            else
                display.gameObject.SetActive(false);
        }
    }


    void OnMouseOver(){
        if(!user.isInitialized.Value)
            return;
        if(Input.GetMouseButtonUp(0)){

            if (val <= 98) val++;
            switch (mod)
            {
                case Mod.STR:
                    stat.STR.Value = val;
                    break;
                case Mod.DEX:
                    stat.DEX.Value = val;
                    health.SetInitialValuesDexRpc();
                    health.CalculateValues();
                    break;
                case Mod.CON:
                    stat.CON.Value = val;
                    health.SetInitialValuesRpc();
                    health.CalculateValues();
                    health.maxHP = health.currHP;
                    break;
                case Mod.INT:
                    stat.INT.Value = val;
                    break;
                case Mod.WIS:
                    stat.WIS.Value = val;
                    break;
                case Mod.CHA:
                    stat.CHA.Value = val;
                    break;
                case Mod.WALK:
                    stat.BASE_SPEED.Value = 44 + val;
                    break;
                case Mod.INIT:
                    display.gameObject.SetActive(true);
                    stat.addProf2Init.Value = true;
                    break;
                case Mod.ARMOR:
                    display.gameObject.SetActive(true);
                    stat.barbarian.Value = true;
                    health.SetInitialValuesDexRpc();
                    health.CalculateValues();
                    break;
                default:
                    break;
            }
            if (mod != Mod.WALK && mod != Mod.INIT && mod != Mod.ARMOR)
                display.text = (val >= 0 ? "+" : "") + val.ToString();
            else if (mod != Mod.INIT && mod != Mod.ARMOR)
                display.text = $"BASE: {44 + val}";

        }
        if (Input.GetMouseButtonUp(1)){

            
            if (val >= -98) val--;

            switch (mod)
            {
                case Mod.STR:
                    stat.STR.Value = val;
                    break;
                case Mod.DEX:
                    stat.DEX.Value = val;
                    health.SetInitialValuesDexRpc();
                    health.CalculateValues();
                    break;
                case Mod.CON:
                    stat.CON.Value = val;
                    health.SetInitialValuesRpc();
                    health.CalculateValues();
                    health.maxHP = health.currHP;
                    break;
                case Mod.INT:
                    stat.INT.Value = val;
                    break;
                case Mod.WIS:
                    stat.WIS.Value = val;
                    break;
                case Mod.CHA:
                    stat.CHA.Value = val;
                    break;
                case Mod.WALK:
                    stat.BASE_SPEED.Value = 44 + val;
                    break;
                case Mod.INIT:
                    display.gameObject.SetActive(false);
                    stat.addProf2Init.Value = false;
                    break;
                case Mod.ARMOR:
                    display.gameObject.SetActive(false);
                    stat.barbarian.Value = false;
                    health.SetInitialValuesDexRpc();
                    health.CalculateValues();
                    break;
                default:
                    break;
            }
            if (mod != Mod.WALK && mod != Mod.INIT && mod != Mod.ARMOR)
                display.text = (val >= 0 ? "+" : "") + val.ToString();
            else if (mod != Mod.INIT && mod != Mod.ARMOR)
                display.text = $"BASE: {44 + val}";
        }
        user.UpdateUserDataRpc(NetworkManager.Singleton.LocalClientId);
    }
}
