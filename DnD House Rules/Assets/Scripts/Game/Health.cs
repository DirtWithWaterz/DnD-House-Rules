using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

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

    public static Dictionary<int, string> bodypartDictionary = new Dictionary<int, string>() {
        {0, "BODY_HEAD"},
        {1, "BODY_NECK"},
        {2, "BODY_CHEST"},
        {3, "BODY_ARM_LEFT"},
        {4, "BODY_FOREARM_LEFT"},
        {5, "BODY_HAND_LEFT"},
        {6, "BODY_ARM_RIGHT"},
        {7, "BODY_FOREARM_RIGHT"},
        {8, "BODY_HAND_RIGHT"},
        {9, "BODY_TORSO"},
        {10, "BODY_PELVIS"},
        {11, "BODY_THIGH_LEFT"},
        {12, "BODY_CRUS_LEFT"},
        {13, "BODY_FOOT_LEFT"},
        {14, "BODY_THIGH_RIGHT"},
        {15, "BODY_CRUS_RIGHT"},
        {16, "BODY_FOOT_RIGHT"}
    };

    public List<GameObject> body = new List<GameObject>();

    public List<GameObject> vitals = new List<GameObject>();

    Stats s;

    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text levelText;

    public bool forceStatus = false;

    public bool Bleeding = false;

    public int maxHP = 0;
    public int currHP = 0;

    public int CONM = 0;
    float lvlBuff;
    int lvl = 0;

    string hpString;

    [SerializeField] State status = State.Unknown;
    [SerializeField] State forcedState = State.Unknown;

    User user;

    // Start is called before the first frame update
    IEnumerator Start(){

        
        s = transform.root.GetComponentInChildren<Stats>();
        lvl = s.lvl.Value;
        CONM = s.CON.Value;
        user = transform.root.GetComponent<User>();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Game");
        yield return new WaitUntil(() => user.isInitialized.Value);
        if(IsOwner){
            SetInitialValuesRpc();
            SetInitialValuesDexRpc();
        }

        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();

            if(b.vital){

                vitals.Add(g);
            }
        }
    }

    // Update is called once per frame
    void Update(){
        
        if(SceneManager.GetActiveScene().name != "Game")
            return;
        if(!user.isInitialized.Value)
            return;

        if(s.CON.Value != CONM)
        {
            CONM = s.CON.Value;
            SetInitialValuesRpc();
            SetInitialValuesDexRpc();
            CalculateValues();
            maxHP = currHP;
        }

        if(lvl != s.lvl.Value){

            lvl = s.lvl.Value;
            SetInitialValuesRpc();
            SetInitialValuesDexRpc();
            CalculateValues();
            maxHP = currHP;
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

        body[0].GetComponent<Bodypart>().maximumHP.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[1].GetComponent<Bodypart>().maximumHP.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[2].GetComponent<Bodypart>().maximumHP.Value = 5+Mathf.RoundToInt(CONM < 0 ? (CONM/1.25f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[3].GetComponent<Bodypart>().maximumHP.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[4].GetComponent<Bodypart>().maximumHP.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[5].GetComponent<Bodypart>().maximumHP.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[6].GetComponent<Bodypart>().maximumHP.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[7].GetComponent<Bodypart>().maximumHP.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[8].GetComponent<Bodypart>().maximumHP.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[9].GetComponent<Bodypart>().maximumHP.Value = 5+Mathf.RoundToInt(CONM < 0 ? (CONM/1.25f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[10].GetComponent<Bodypart>().maximumHP.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[11].GetComponent<Bodypart>().maximumHP.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[12].GetComponent<Bodypart>().maximumHP.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[13].GetComponent<Bodypart>().maximumHP.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[14].GetComponent<Bodypart>().maximumHP.Value = 4+Mathf.RoundToInt(CONM < 0 ? (CONM/1.66f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[15].GetComponent<Bodypart>().maximumHP.Value = 3+Mathf.RoundToInt(CONM < 0 ? (CONM/2.5f) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));
        body[16].GetComponent<Bodypart>().maximumHP.Value = 2+Mathf.RoundToInt(CONM < 0 ? (CONM/5) + (s.lvl.Value > 1 ? lvlBuff-2 : 0) : CONM > 0 ? CONM*lvlBuff : (s.lvl.Value > 1 ? lvlBuff-1.5f : 0));

        foreach(Bodypart bodypart in user.bodyparts){

            int addVal = 0;
            for(int i = 0; i < bodypart.slot.Length; i++){

                if(bodypart.slot[i].slotModifierType == SlotModifierType.hp)
                addVal += bodypart.slot[i].item.value;
            }
            bodypart.maximumHP.Value += addVal;
        }

        // foreach(GameObject g in body){

        //     Bodypart b = g.GetComponent<Bodypart>();
        //     b.maximumHP.Value = b.currentHP.Value;
        //     // b.condition.Value = "normal, no notable issues or sustained injuries. ";
        // }
        // if(IsHost)
        //     Debug.Log("host made it to condition setting");
        // if(!IsHost)
        //     return;
        foreach(userData data in GameManager.Singleton.userDatas){

            if(data.id == user.OwnerClientId){
                body[0].GetComponent<Bodypart>().currentHP.Value = data.HP_HEAD;
                body[1].GetComponent<Bodypart>().currentHP.Value = data.HP_NECK;
                body[2].GetComponent<Bodypart>().currentHP.Value = data.HP_CHEST;
                body[3].GetComponent<Bodypart>().currentHP.Value = data.HP_ARM_LEFT;
                body[4].GetComponent<Bodypart>().currentHP.Value = data.HP_FOREARM_LEFT;
                body[5].GetComponent<Bodypart>().currentHP.Value = data.HP_HAND_LEFT;
                body[6].GetComponent<Bodypart>().currentHP.Value = data.HP_ARM_RIGHT;
                body[7].GetComponent<Bodypart>().currentHP.Value = data.HP_FOREARM_RIGHT;
                body[8].GetComponent<Bodypart>().currentHP.Value = data.HP_HAND_RIGHT;
                body[9].GetComponent<Bodypart>().currentHP.Value = data.HP_TORSO;
                body[10].GetComponent<Bodypart>().currentHP.Value = data.HP_PELVIS;
                body[11].GetComponent<Bodypart>().currentHP.Value = data.HP_THIGH_LEFT;
                body[12].GetComponent<Bodypart>().currentHP.Value = data.HP_CRUS_LEFT;
                body[13].GetComponent<Bodypart>().currentHP.Value = data.HP_FOOT_LEFT;
                body[14].GetComponent<Bodypart>().currentHP.Value = data.HP_THIGH_RIGHT;
                body[15].GetComponent<Bodypart>().currentHP.Value = data.HP_CRUS_RIGHT;
                body[16].GetComponent<Bodypart>().currentHP.Value = data.HP_FOOT_RIGHT;
                SetConditionInHealthRpc(data.username.ToString(), data);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    void SetConditionInHealthRpc(string username, userData data){
        if(!IsHost)
            return;
        try {
            GameManager.Singleton.interpreter.SetConditionRpc(username, 0, GameManager.Singleton.conditionsValueKey[data.CONDITION_HEAD.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 1, GameManager.Singleton.conditionsValueKey[data.CONDITION_NECK.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 2, GameManager.Singleton.conditionsValueKey[data.CONDITION_CHEST.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 3, GameManager.Singleton.conditionsValueKey[data.CONDITION_ARM_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 4, GameManager.Singleton.conditionsValueKey[data.CONDITION_FOREARM_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 5, GameManager.Singleton.conditionsValueKey[data.CONDITION_HAND_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 6, GameManager.Singleton.conditionsValueKey[data.CONDITION_ARM_RIGHT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 7, GameManager.Singleton.conditionsValueKey[data.CONDITION_FOREARM_RIGHT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 8, GameManager.Singleton.conditionsValueKey[data.CONDITION_HAND_RIGHT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 9, GameManager.Singleton.conditionsValueKey[data.CONDITION_TORSO.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 10, GameManager.Singleton.conditionsValueKey[data.CONDITION_PELVIS.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 11, GameManager.Singleton.conditionsValueKey[data.CONDITION_THIGH_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 12, GameManager.Singleton.conditionsValueKey[data.CONDITION_CRUS_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 13, GameManager.Singleton.conditionsValueKey[data.CONDITION_FOOT_LEFT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 14, GameManager.Singleton.conditionsValueKey[data.CONDITION_THIGH_RIGHT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 15, GameManager.Singleton.conditionsValueKey[data.CONDITION_CRUS_RIGHT.ToString()]);
            GameManager.Singleton.interpreter.SetConditionRpc(username, 16, GameManager.Singleton.conditionsValueKey[data.CONDITION_FOOT_RIGHT.ToString()]);
        } catch {

            StartCoroutine(GameManager.Singleton.LoadData());
        }
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

        foreach(Bodypart bodypart in user.bodyparts){

            int addVal = 0;
            for(int i = 0; i < bodypart.slot.Length; i++){

                if(bodypart.slot[i].slotModifierType == SlotModifierType.ac)
                addVal += bodypart.slot[i].item.value;
            }
            bodypart.ac.Value += addVal;
        }
    }

    public void CalculateValues(){

        currHP = 0;
        foreach(GameObject g in body){

            Bodypart b = g.GetComponent<Bodypart>();
            currHP += b.currentHP.Value;
        }
        hpString =
            currHP > 999 ? $"??9":
            currHP > -1 ? $"{currHP:000}":
            "??0";

        if(!forceStatus){

            status = 
                currHP >= maxHP ? State.Healthy : 
                currHP >= Mathf.RoundToInt(maxHP*0.8f) ? State.Stable : 
                currHP >= Mathf.RoundToInt(maxHP*0.6f) ? State.Brused : 
                currHP >= Mathf.RoundToInt(maxHP*0.4f) ? State.Injured :
                currHP >= Mathf.RoundToInt(maxHP*0.2f) ? State.Critical : 
                currHP > 0 ? State.Unconcious : 
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
