using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.Netcode;

public enum State{

    Unknown = -9,
    Concaved = -8,
    Dismembered = -7,
    Emaciated = -6,
    Paralyzed = -5,
    Sick = -4,
    Poisoned = -3,
    Bleeding = -2,
    Deceased = -1,
    Unconcious = 0,
    Critical = 1,
    Injured = 2,
    Brused = 3,
    Stable = 4,
    Healthy = 5
}

public class Health : NetworkBehaviour
{

    public List<GameObject> body = new List<GameObject>();

    public List<GameObject> vitals = new List<GameObject>();

    Stats s;

    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text levelText;

    public bool forceStatus = false;

    public bool Bleeding = false;

    public int maxHP = 0;
    public int currentHP = 0;

    public int CONM = 0;
    float lvlBuff;
    int lvl = 0;

    string hpString;

    [SerializeField] State status = State.Unknown;
    [SerializeField] State forcedState = State.Unknown;

    // Start is called before the first frame update
    void Start(){
        
        s = transform.root.GetComponentInChildren<Stats>();
        lvl = s.lvl.Value;
        CONM = s.CON.Value;
        
        if(IsOwner)
            SetInitialValuesRpc();

        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();
            maxHP += b.hp.Value;
        }

        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();

            if(b.vital){

                vitals.Add(g);
            }
        }
        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();
            b.maxHP = b.hp;
        }
    }

    // Update is called once per frame
    void Update(){

        if(s.CON.Value != CONM)
        {
            CONM = s.CON.Value;
            SetInitialValuesRpc();
            CalculateValues();
            maxHP = currentHP;
            foreach(GameObject g in body){

                Bodypart b = g.GetComponent<Bodypart>();
                b.maxHP = b.hp;
            }
        }

        if(lvl != s.lvl.Value){

            lvl = s.lvl.Value;
            SetInitialValuesRpc();
            CalculateValues();
            maxHP = currentHP;
            foreach(GameObject g in body){

                Bodypart b = g.GetComponent<Bodypart>();
                b.maxHP = b.hp;
            }
        }
        
        levelText.text = $"LV: {lvl}";

        CalculateValues();

        status = forceStatus ? forcedState : status;

        statusText.text = $"THP: {hpString}  Status: {status.ToString()}";
    }

    [Rpc(SendTo.Owner)]
    public void SetInitialValuesRpc(){

        lvlBuff = 
            s.lvl.Value >= 17 ? 10f : 
            s.lvl.Value >= 12 ? 8.5f : 
            s.lvl.Value >= 8 ? 7f :  
            s.lvl.Value >= 5 ? 6f : 
            s.lvl.Value >= 3 ? 5f :
            s.lvl.Value >= 2 ? 3.5f :
            1.0f;

        body[0].GetComponent<Bodypart>().hp.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[1].GetComponent<Bodypart>().hp.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[2].GetComponent<Bodypart>().hp.Value = 5+Mathf.RoundToInt(CONM < 0 ? (CONM/1.25f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[3].GetComponent<Bodypart>().hp.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[4].GetComponent<Bodypart>().hp.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[5].GetComponent<Bodypart>().hp.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[6].GetComponent<Bodypart>().hp.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[7].GetComponent<Bodypart>().hp.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[8].GetComponent<Bodypart>().hp.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[9].GetComponent<Bodypart>().hp.Value = 5+Mathf.RoundToInt(CONM < 0 ? (CONM/1.25f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[10].GetComponent<Bodypart>().hp.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[11].GetComponent<Bodypart>().hp.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[12].GetComponent<Bodypart>().hp.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[13].GetComponent<Bodypart>().hp.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[14].GetComponent<Bodypart>().hp.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[15].GetComponent<Bodypart>().hp.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[16].GetComponent<Bodypart>().hp.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));


        
    }

    [Rpc(SendTo.Owner)]
    public void SetInitialValuesDexRpc(){

        body[0].GetComponent<Bodypart>().ac.Value = 11 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[1].GetComponent<Bodypart>().ac.Value = 12 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[2].GetComponent<Bodypart>().ac.Value = 8 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[3].GetComponent<Bodypart>().ac.Value = 9 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[4].GetComponent<Bodypart>().ac.Value = 11 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[5].GetComponent<Bodypart>().ac.Value = 12 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[6].GetComponent<Bodypart>().ac.Value = 9 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[7].GetComponent<Bodypart>().ac.Value = 11 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[8].GetComponent<Bodypart>().ac.Value = 12 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[9].GetComponent<Bodypart>().ac.Value = 8 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[10].GetComponent<Bodypart>().ac.Value = 9 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[11].GetComponent<Bodypart>().ac.Value = 9 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[12].GetComponent<Bodypart>().ac.Value = 11 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[13].GetComponent<Bodypart>().ac.Value = 12 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[14].GetComponent<Bodypart>().ac.Value = 9 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[15].GetComponent<Bodypart>().ac.Value = 11 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
        body[16].GetComponent<Bodypart>().ac.Value = 12 + s.DEX.Value + (s.barbarian.Value ? s.CON.Value : 0);
    }

    public void CalculateValues(){

        currentHP = 0;
        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();
            currentHP += b.hp.Value;
        }
        hpString =
            currentHP > 999 ? $"??9":
            currentHP > -1 ? $"{currentHP:000}":
            "??0";

        if(!forceStatus){

            status = 
                currentHP >= maxHP ? State.Healthy : 
                currentHP >= Mathf.RoundToInt(maxHP*0.8f) ? State.Stable : 
                currentHP >= Mathf.RoundToInt(maxHP*0.6f) ? State.Brused : 
                currentHP >= Mathf.RoundToInt(maxHP*0.4f) ? State.Injured :
                currentHP >= Mathf.RoundToInt(maxHP*0.2f) ? State.Critical : 
                currentHP > 0 ? State.Unconcious : 
                State.Deceased;
        }
        else{
            status = forcedState;
        }
    }

    public void ForceStatus(){

        forceStatus = false;
        forcedState = State.Unknown;
        CalculateValues();
    }
    public void ForceStatus(State state){

        forceStatus = true;
        forcedState = state;
    }

    public bool VitalsNormal(){

        foreach(GameObject g in vitals){

            Bodypart v = g.GetComponent<Bodypart>();
            if(v.status.Value == State.Healthy || v.status.Value == State.Stable || v.status.Value == State.Brused)
                continue;
            else
                return false;
        }
        return true;
    }
}
