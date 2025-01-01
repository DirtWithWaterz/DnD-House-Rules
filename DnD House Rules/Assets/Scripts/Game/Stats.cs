using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Stats : NetworkBehaviour
{

    Health h;

    public NetworkVariable<bool> addProf2Init = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> barbarian = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> lvl = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> BASE_SPEED = new NetworkVariable<int>(44, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> WEIGHT = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private string weightString;

    public NetworkVariable<int> STR = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> DEX = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> CON = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> INT = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> WIS = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> CHA = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private int DEXWM;
    private int STRWM;
    private int CONWM;

    public NetworkVariable<int> PROF = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float SPEED = 0;
    public NetworkVariable<int> INIT = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> ARMOR = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

    User user;

    // Start is called before the first frame update
    void Start()
    {
        h = transform.root.GetComponentInChildren<Health>();
        user = transform.root.GetComponent<User>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
            return;
        
        PROF.Value = Mathf.CeilToInt(1f + ((1f/4f)*lvl.Value));
        display0.text = $"+{PROF.Value}";

        SPEED = (BASE_SPEED.Value + STR.Value + (DEX.Value/2)) / (1 + (WEIGHT.Value / 1000));
        display4.text = SPEED.ToString();
        SPEED = Mathf.Clamp(SPEED, 0, 99);
        display1.text = $"{Mathf.RoundToInt(SPEED)} ft.";

        INIT.Value = DEX.Value + (addProf2Init.Value ? PROF.Value : 0);
        display2.text = (INIT.Value >= 0 ? "+" : "") + INIT.Value.ToString();

        float sum = 0;
        int count = 0;
        foreach(GameObject g in h.body)
        {
            Bodypart b = g.GetComponent<Bodypart>();
            sum += b.ac.Value;
            count++;
        }
        ARMOR.Value = Mathf.RoundToInt(sum / count);
        display3.text = ARMOR.Value.ToString();

        STRWM = STR.Value*5;
        CONWM = Mathf.FloorToInt((CON.Value*8)/(barbarian.Value ? 2f : 1));
        DEXWM = -(DEX.Value*6);

        WEIGHT.Value = 100+(STRWM+CONWM+DEXWM)+(barbarian.Value ? 50 : 0);

        weightString = 
            WEIGHT.Value > 999 ? "??9":
            WEIGHT.Value > -1 ? $"{WEIGHT.Value:000}":
            "??0";
        
        display5.text = $"{weightString} Lbs.";
    }
}
