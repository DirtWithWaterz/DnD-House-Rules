using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{

    Health h;

    public bool addProf2Init = false;

    public bool barbarian = false;

    public int lvl = 1;

    public int BASE_SPEED = 44;

    public float WEIGHT = 0;

    private string weightString;

    public int STR = 0;
    public int DEX = 0;
    public int CON = 0;
    public int INT = 0;
    public int WIS = 0;
    public int CHA = 0;

    private int DEXWM;
    private int STRWM;
    private int CONWM;

    public int PROF = 0;
    public float SPEED = 0;
    public int INIT = 0;
    public int ARMOR = 0;

    [SerializeField]
    TMP_Text display0;
    [SerializeField]
    TMP_Text display1;
    [SerializeField]
    TMP_Text display2;
    [SerializeField]
    TMP_Text display3;
    [SerializeField]
    TMP_Text display4;
    [SerializeField]
    TMP_Text display5;

    // Start is called before the first frame update
    void Start()
    {
        h = Component.FindObjectOfType<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        PROF = Mathf.CeilToInt(1f + ((1f/4f)*lvl));
        display0.text = $"+{PROF}";

        SPEED = (BASE_SPEED + STR + (DEX/2)) / (1 + (WEIGHT / 1000));
        display4.text = SPEED.ToString();
        SPEED = Mathf.Clamp(SPEED, 0, 99);
        display1.text = $"{Mathf.RoundToInt(SPEED)} ft.";

        INIT = DEX + (addProf2Init ? PROF : 0);
        display2.text = (INIT >= 0 ? "+" : "") + INIT.ToString();

        float sum = 0;
        int count = 0;
        foreach(GameObject g in h.body)
        {
            Bodypart b = g.GetComponent<Bodypart>();
            sum += b.ac;
            count++;
        }
        ARMOR = Mathf.RoundToInt(sum / count);
        display3.text = ARMOR.ToString();

        STRWM = STR*5;
        CONWM = Mathf.FloorToInt((CON*8)/(barbarian ? 2f : 1));
        DEXWM = -(DEX*6);

        WEIGHT = 100+(STRWM+CONWM+DEXWM)+(barbarian ? 50 : 0);

        weightString = 
            WEIGHT > 999 ? "??9":
            WEIGHT > -1 ? $"{WEIGHT:000}":
            "??0";
        
        display5.text = $"{weightString} Lbs.";
    }
}
