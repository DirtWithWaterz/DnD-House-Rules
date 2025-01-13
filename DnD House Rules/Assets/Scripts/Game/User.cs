using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class User : NetworkBehaviour
{
    public NetworkVariable<bool> isInitialized = new NetworkVariable<bool>(false);
    Camera cam;
    public GameObject screen;

    NetworkVariable<int> clientsReady = new NetworkVariable<int>(0);

    Health health;
    public Stats stats;
    InqueCalendar calendar;

    public List<Bodypart> bodyparts;

    Interpreter interpreter;

    public Backpack backpack;

    [SerializeField] TMP_Text CurrentPlayerLabel1;
    [SerializeField] TMP_Text CurrentPlayerLabel2;

    IEnumerator Start()
    {
        Time.fixedDeltaTime = 10f;
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Game");

        SceneManager.activeSceneChanged += SceneChanged;

        cam = transform.GetChild(0).GetComponent<Camera>();
        screen = transform.GetChild(1).gameObject;

        health = screen.GetComponentInChildren<Health>();
        stats = screen.GetComponentInChildren<Stats>();
        calendar = GameManager.Singleton.inqueCalendar;

        interpreter = FindObjectOfType<Interpreter>();
        backpack = GetComponentInChildren<Backpack>();

        bodyparts = new List<Bodypart>();

        if(IsOwner){
            interpreter.user = this;
            interpreter.CurrentPlayerLabel1 = CurrentPlayerLabel1;
            interpreter.CurrentPlayerLabel2 = CurrentPlayerLabel2;
        }
        
        foreach(GameObject part in health.body){

            bodyparts.Add(part.GetComponent<Bodypart>());
        }
        if(!IsOwner){
            Destroy(cam.gameObject);
            screen.GetComponent<Canvas>().enabled = false;
            yield return false;
        }
        ClientReadyRpc();
        if(IsHost){

            yield return new WaitUntil(() => clientsReady.Value==NetworkManager.Singleton.ConnectedClientsList.Count);
            InitializedRpc();
        }
        yield return new WaitUntil(() => isInitialized.Value);
        UpdateUserDataRpc(NetworkManager.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    void ClientReadyRpc(){

        clientsReady.Value++;
    }
    [Rpc(SendTo.Server)]
    void InitializedRpc(){

        GameManager.Singleton.interpreter.init.Value = true;
        isInitialized.Value = true;
        LoadDataRpc();
    }
    [Rpc(SendTo.Everyone)]
    void LoadDataRpc(){

        StartCoroutine(GameManager.Singleton.LoadData());
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        if(arg1.name == "Game"){
            if(cam != null)
                cam.enabled = true;
            if(screen != null)
                screen.SetActive(true);
        }
    }

    void FixedUpdate(){ // Fixed Timestep = 10 || update will happen once every 10 seconds (much more affordable)
        if(!isInitialized.Value)
            return;
        // if(!screen.activeInHierarchy) return;
        if (GameManager.Singleton == null){
            Debug.LogError("GameManager.Singleton is null!");
            return;
        }
        UpdateUserDataRpc(NetworkManager.LocalClientId);
    }

    // [Rpc(SendTo.ClientsAndHost)]
    // public void DataLoadedRpc(string val){

    //     Debug.Log($"{val} userdata successfully loaded!");
    // }

    public bool ready;

    [Rpc(SendTo.Everyone)]
    public void LoadUserDataRpc(int index){

        if(!IsHost)
            return;

        interpreter.SetLevelRpc(this.name, GameManager.Singleton.userDatas[index].Lvl);
        interpreter.SetModRpc(this.name, "con", GameManager.Singleton.userDatas[index].Con);

        interpreter.SetModRpc(this.name, "str", GameManager.Singleton.userDatas[index].Str);
        interpreter.SetModRpc(this.name, "dex", GameManager.Singleton.userDatas[index].Dex);
        interpreter.SetModRpc(this.name, "int", GameManager.Singleton.userDatas[index].Int);
        interpreter.SetModRpc(this.name, "wis", GameManager.Singleton.userDatas[index].Wis);
        interpreter.SetModRpc(this.name, "cha", GameManager.Singleton.userDatas[index].Cha);

        interpreter.SetBaseSpeedRpc(this.name, GameManager.Singleton.userDatas[index].baseSpeed);
        interpreter.SetBarbarianRpc(this.name, GameManager.Singleton.userDatas[index].barbarian);
        interpreter.SetProf2InitRpc(this.name, GameManager.Singleton.userDatas[index].initProf);

        StartCoroutine(AllocateEnum());

        IEnumerator AllocateEnum(){

            yield return new WaitForSeconds(0.25f);

            interpreter.SetHealthRpc(this.name, 0, GameManager.Singleton.userDatas[index].HP_HEAD);
            interpreter.SetHealthRpc(this.name, 1, GameManager.Singleton.userDatas[index].HP_NECK);
            interpreter.SetHealthRpc(this.name, 2, GameManager.Singleton.userDatas[index].HP_CHEST);
            interpreter.SetHealthRpc(this.name, 3, GameManager.Singleton.userDatas[index].HP_ARM_LEFT);
            interpreter.SetHealthRpc(this.name, 4, GameManager.Singleton.userDatas[index].HP_FOREARM_LEFT);
            interpreter.SetHealthRpc(this.name, 5, GameManager.Singleton.userDatas[index].HP_HAND_LEFT);
            interpreter.SetHealthRpc(this.name, 6, GameManager.Singleton.userDatas[index].HP_ARM_RIGHT);
            interpreter.SetHealthRpc(this.name, 7, GameManager.Singleton.userDatas[index].HP_FOREARM_RIGHT);
            interpreter.SetHealthRpc(this.name, 8, GameManager.Singleton.userDatas[index].HP_HAND_RIGHT);
            interpreter.SetHealthRpc(this.name, 9, GameManager.Singleton.userDatas[index].HP_TORSO);
            interpreter.SetHealthRpc(this.name, 10, GameManager.Singleton.userDatas[index].HP_PELVIS);
            interpreter.SetHealthRpc(this.name, 11, GameManager.Singleton.userDatas[index].HP_THIGH_LEFT);
            interpreter.SetHealthRpc(this.name, 12, GameManager.Singleton.userDatas[index].HP_CRUS_LEFT);
            interpreter.SetHealthRpc(this.name, 13, GameManager.Singleton.userDatas[index].HP_FOOT_LEFT);
            interpreter.SetHealthRpc(this.name, 14, GameManager.Singleton.userDatas[index].HP_THIGH_RIGHT);
            interpreter.SetHealthRpc(this.name, 15, GameManager.Singleton.userDatas[index].HP_CRUS_RIGHT);
            interpreter.SetHealthRpc(this.name, 16, GameManager.Singleton.userDatas[index].HP_FOOT_RIGHT);

            interpreter.SetArmorClassRpc(this.name, 0, GameManager.Singleton.userDatas[index].AC_HEAD);
            interpreter.SetArmorClassRpc(this.name, 1, GameManager.Singleton.userDatas[index].AC_NECK);
            interpreter.SetArmorClassRpc(this.name, 2, GameManager.Singleton.userDatas[index].AC_CHEST);
            interpreter.SetArmorClassRpc(this.name, 3, GameManager.Singleton.userDatas[index].AC_ARM_LEFT);
            interpreter.SetArmorClassRpc(this.name, 4, GameManager.Singleton.userDatas[index].AC_FOREARM_LEFT);
            interpreter.SetArmorClassRpc(this.name, 5, GameManager.Singleton.userDatas[index].AC_HAND_LEFT);
            interpreter.SetArmorClassRpc(this.name, 6, GameManager.Singleton.userDatas[index].AC_ARM_RIGHT);
            interpreter.SetArmorClassRpc(this.name, 7, GameManager.Singleton.userDatas[index].AC_FOREARM_RIGHT);
            interpreter.SetArmorClassRpc(this.name, 8, GameManager.Singleton.userDatas[index].AC_HAND_RIGHT);
            interpreter.SetArmorClassRpc(this.name, 9, GameManager.Singleton.userDatas[index].AC_TORSO);
            interpreter.SetArmorClassRpc(this.name, 10, GameManager.Singleton.userDatas[index].AC_PELVIS);
            interpreter.SetArmorClassRpc(this.name, 11, GameManager.Singleton.userDatas[index].AC_THIGH_LEFT);
            interpreter.SetArmorClassRpc(this.name, 12, GameManager.Singleton.userDatas[index].AC_CRUS_LEFT);
            interpreter.SetArmorClassRpc(this.name, 13, GameManager.Singleton.userDatas[index].AC_FOOT_LEFT);
            interpreter.SetArmorClassRpc(this.name, 14, GameManager.Singleton.userDatas[index].AC_THIGH_RIGHT);
            interpreter.SetArmorClassRpc(this.name, 15, GameManager.Singleton.userDatas[index].AC_CRUS_RIGHT);
            interpreter.SetArmorClassRpc(this.name, 16, GameManager.Singleton.userDatas[index].AC_FOOT_RIGHT);

            interpreter.SetConditionRpc(this.name, 0, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_HEAD.ToString()]);
            interpreter.SetConditionRpc(this.name, 1, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_NECK.ToString()]);
            interpreter.SetConditionRpc(this.name, 2, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_CHEST.ToString()]);
            interpreter.SetConditionRpc(this.name, 3, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_ARM_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 4, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_FOREARM_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 5, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_HAND_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 6, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_ARM_RIGHT.ToString()]);
            interpreter.SetConditionRpc(this.name, 7, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_FOREARM_RIGHT.ToString()]);
            interpreter.SetConditionRpc(this.name, 8, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_HAND_RIGHT.ToString()]);
            interpreter.SetConditionRpc(this.name, 9, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_TORSO.ToString()]);
            interpreter.SetConditionRpc(this.name, 10, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_PELVIS.ToString()]);
            interpreter.SetConditionRpc(this.name, 11, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_THIGH_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 12, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_CRUS_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 13, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_FOOT_LEFT.ToString()]);
            interpreter.SetConditionRpc(this.name, 14, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_THIGH_RIGHT.ToString()]);
            interpreter.SetConditionRpc(this.name, 15, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_CRUS_RIGHT.ToString()]);
            interpreter.SetConditionRpc(this.name, 16, GameManager.Singleton.conditionsValueKey[GameManager.Singleton.userDatas[index].CONDITION_FOOT_RIGHT.ToString()]);

        }

        // DataLoadedRpc(this.name);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateUserDataRpc(ulong id){
        
        if(!IsHost)
            return;
        NetworkObject netObj = null;

        foreach(userData data in GameManager.Singleton.userDatas){
            
            // Debug.Log($"Checking ID: {data.id}");
            if(data.id == id){

                netObj = data.objRef;
            }
        }
        if (netObj == null){
            Debug.LogError($"NetworkObject not found for id {id}");
            return; // Exit the method early to avoid further errors
        }

        User user = netObj.gameObject.GetComponent<User>();

        for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

            if(GameManager.Singleton.userDatas[i].id == id){
                
                userData data = new userData{

                    id = GameManager.Singleton.userDatas[i].id,
                    username = GameManager.Singleton.userDatas[i].username,
                    objRef = GameManager.Singleton.userDatas[i].objRef,

                    Str = user.stats.STR.Value,
                    Dex = user.stats.DEX.Value,
                    Con = user.stats.CON.Value,
                    Int = user.stats.INT.Value,
                    Wis = user.stats.WIS.Value,
                    Cha = user.stats.CHA.Value,

                    Lvl = user.stats.lvl.Value,

                    HP_HEAD = user.bodyparts[0].currentHP.Value,
                    HP_NECK = user.bodyparts[1].currentHP.Value,
                    HP_CHEST = user.bodyparts[2].currentHP.Value,
                    HP_ARM_LEFT = user.bodyparts[3].currentHP.Value,
                    HP_FOREARM_LEFT = user.bodyparts[4].currentHP.Value,
                    HP_HAND_LEFT = user.bodyparts[5].currentHP.Value,
                    HP_ARM_RIGHT = user.bodyparts[6].currentHP.Value,
                    HP_FOREARM_RIGHT = user.bodyparts[7].currentHP.Value,
                    HP_HAND_RIGHT = user.bodyparts[8].currentHP.Value,
                    HP_TORSO = user.bodyparts[9].currentHP.Value,
                    HP_PELVIS = user.bodyparts[10].currentHP.Value,
                    HP_THIGH_LEFT = user.bodyparts[11].currentHP.Value,
                    HP_CRUS_LEFT = user.bodyparts[12].currentHP.Value,
                    HP_FOOT_LEFT = user.bodyparts[13].currentHP.Value,
                    HP_THIGH_RIGHT = user.bodyparts[14].currentHP.Value,
                    HP_CRUS_RIGHT = user.bodyparts[15].currentHP.Value,
                    HP_FOOT_RIGHT = user.bodyparts[16].currentHP.Value,

                    AC_HEAD = user.bodyparts[0].ac.Value,
                    AC_NECK = user.bodyparts[1].ac.Value,
                    AC_CHEST = user.bodyparts[2].ac.Value,
                    AC_ARM_LEFT = user.bodyparts[3].ac.Value,
                    AC_FOREARM_LEFT = user.bodyparts[4].ac.Value,
                    AC_HAND_LEFT = user.bodyparts[5].ac.Value,
                    AC_ARM_RIGHT = user.bodyparts[6].ac.Value,
                    AC_FOREARM_RIGHT = user.bodyparts[7].ac.Value,
                    AC_HAND_RIGHT = user.bodyparts[8].ac.Value,
                    AC_TORSO = user.bodyparts[9].ac.Value,
                    AC_PELVIS = user.bodyparts[10].ac.Value,
                    AC_THIGH_LEFT = user.bodyparts[11].ac.Value,
                    AC_CRUS_LEFT = user.bodyparts[12].ac.Value,
                    AC_FOOT_LEFT = user.bodyparts[13].ac.Value,
                    AC_THIGH_RIGHT = user.bodyparts[14].ac.Value,
                    AC_CRUS_RIGHT = user.bodyparts[15].ac.Value,
                    AC_FOOT_RIGHT = user.bodyparts[16].ac.Value,
                    
                    CONDITION_HEAD = GameManager.Singleton.conditionsKeyValue[user.bodyparts[0].condition.Value.ToString()],
                    CONDITION_NECK = GameManager.Singleton.conditionsKeyValue[user.bodyparts[1].condition.Value.ToString()],
                    CONDITION_CHEST = GameManager.Singleton.conditionsKeyValue[user.bodyparts[2].condition.Value.ToString()],
                    CONDITION_ARM_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[3].condition.Value.ToString()],
                    CONDITION_FOREARM_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[4].condition.Value.ToString()],
                    CONDITION_HAND_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[5].condition.Value.ToString()],
                    CONDITION_ARM_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[6].condition.Value.ToString()],
                    CONDITION_FOREARM_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[7].condition.Value.ToString()],
                    CONDITION_HAND_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[8].condition.Value.ToString()],
                    CONDITION_TORSO = GameManager.Singleton.conditionsKeyValue[user.bodyparts[9].condition.Value.ToString()],
                    CONDITION_PELVIS = GameManager.Singleton.conditionsKeyValue[user.bodyparts[10].condition.Value.ToString()],
                    CONDITION_THIGH_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[11].condition.Value.ToString()],
                    CONDITION_CRUS_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[12].condition.Value.ToString()],
                    CONDITION_FOOT_LEFT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[13].condition.Value.ToString()],
                    CONDITION_THIGH_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[14].condition.Value.ToString()],
                    CONDITION_CRUS_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[15].condition.Value.ToString()],
                    CONDITION_FOOT_RIGHT = GameManager.Singleton.conditionsKeyValue[user.bodyparts[16].condition.Value.ToString()],

                    barbarian = user.stats.barbarian.Value,
                    baseSpeed = user.stats.BASE_SPEED.Value,
                    initProf = user.stats.addProf2Init.Value

                };
                // Debug.LogWarning($"Username: {data.username} : ID: {id} : Chest Condition: {data.CONDITION_CHEST}");
                GameManager.Singleton.userDatas[i] = data;
            }
        }
    }
}
