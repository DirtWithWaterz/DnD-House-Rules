using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Skill : MonoBehaviour
{


    public string id;

    public int mod;
    public TMP_Text mod_t;

    public mod_type type;

    User user;

    public SkillButton button;

    void Start()
    {

        user = transform.root.GetComponent<User>();
        id = name;
        button.skill = this;
    }

    void Update()
    {
        if (!user.isInitialized.Value)
            return;

        switch (type)
            {

                case mod_type.STR:
                    mod = user.stats.STR.Value;
                    break;
                case mod_type.DEX:
                    mod = user.stats.DEX.Value;
                    break;
                case mod_type.CON:
                    mod = user.stats.CON.Value;
                    break;
                case mod_type.INT:
                    mod = user.stats.INT.Value;
                    break;
                case mod_type.WIS:
                    mod = user.stats.WIS.Value;
                    break;
                case mod_type.CHA:
                    mod = user.stats.CHA.Value;
                    break;
            }

        int profMod = 0;

        switch (button.profVal)
        {
            case proficiency.NONE:
                profMod = 0;
                button.fill.SetActive(false);
                button.blk.SetActive(true);
                button.hlf.SetActive(false);
                break;
            case proficiency.HALF:
                profMod = user.stats.PROF.Value / 2;
                button.fill.SetActive(false);
                button.blk.SetActive(true);
                button.hlf.SetActive(true);
                break;
            case proficiency.FULL:
                profMod = user.stats.PROF.Value;
                button.fill.SetActive(true);
                button.blk.SetActive(true);
                button.hlf.SetActive(false);
                break;
            case proficiency.DOUBLE:
                profMod = user.stats.PROF.Value * 2;
                button.fill.SetActive(true);
                button.blk.SetActive(false);
                button.hlf.SetActive(false);
                break;
        }

        string add = mod >= 0 ? "+" : "-";
        mod_t.text = $"{add}{mod+profMod}";
    }
}

public enum mod_type
{

    STR,
    DEX,
    CON,
    INT,
    WIS,
    CHA
}
public enum proficiency {

    NONE,
    HALF,
    FULL,
    DOUBLE
}
