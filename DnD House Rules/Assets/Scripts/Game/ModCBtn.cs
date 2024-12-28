using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Awake(){

        display = this.GetComponentInChildren<TMP_Text>();
        stat = Component.FindObjectOfType<Stats>();
        health = Component.FindObjectOfType<Health>();

        if (mod != Mod.WALK && mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = (val >= 0 ? "+" : "") + val.ToString();
        else if (mod != Mod.INIT && mod != Mod.ARMOR)
            display.text = $"BASE: {44 + val}";

        if (mod == Mod.INIT || mod == Mod.ARMOR)
            display.gameObject.SetActive(false);

        
    }


    void OnMouseOver(){

        if(Input.GetMouseButtonUp(0)){

            if (val <= 98) val++;
            switch (mod)
            {
                case Mod.STR:
                    stat.STR = val;
                    break;
                case Mod.DEX:
                    stat.DEX = val;
                    health.SetInitialValues();
                    health.CalculateValues();
                    break;
                case Mod.CON:
                    stat.CON = val;
                    health.SetInitialValues();
                    health.CalculateValues();
                    health.maxHP = health.currentHP;
                    break;
                case Mod.INT:
                    stat.INT = val;
                    break;
                case Mod.WIS:
                    stat.WIS = val;
                    break;
                case Mod.CHA:
                    stat.CHA = val;
                    break;
                case Mod.WALK:
                    stat.BASE_SPEED = 44 + val;
                    break;
                case Mod.INIT:
                    display.gameObject.SetActive(true);
                    stat.addProf2Init = true;
                    break;
                case Mod.ARMOR:
                    display.gameObject.SetActive(true);
                    stat.barbarian = true;
                    health.SetInitialValues();
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
                    stat.STR = val;
                    break;
                case Mod.DEX:
                    stat.DEX = val;
                    health.SetInitialValues();
                    health.CalculateValues();
                    break;
                case Mod.CON:
                    stat.CON = val;
                    health.SetInitialValues();
                    health.CalculateValues();
                    health.maxHP = health.currentHP;
                    break;
                case Mod.INT:
                    stat.INT = val;
                    break;
                case Mod.WIS:
                    stat.WIS = val;
                    break;
                case Mod.CHA:
                    stat.CHA = val;
                    break;
                case Mod.WALK:
                    stat.BASE_SPEED = 44 + val;
                    break;
                case Mod.INIT:
                    display.gameObject.SetActive(false);
                    stat.addProf2Init = false;
                    break;
                case Mod.ARMOR:
                    display.gameObject.SetActive(false);
                    stat.barbarian = false;
                    health.SetInitialValues();
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
    }
}
