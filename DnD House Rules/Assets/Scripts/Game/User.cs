using System.Collections;
using System.Collections.Generic;
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

        bodyparts = new List<Bodypart>();

        if(IsOwner)
            interpreter.user = this;
        
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

        isInitialized.Value = true;
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

    [Rpc(SendTo.Server)]
    public void UpdateUserDataRpc(ulong id){
        
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

                    HP_HEAD = user.bodyparts[0].hp.Value,
                    HP_NECK = user.bodyparts[1].hp.Value,
                    HP_CHEST = user.bodyparts[2].hp.Value,
                    HP_ARM_LEFT = user.bodyparts[3].hp.Value,
                    HP_FOREARM_LEFT = user.bodyparts[4].hp.Value,
                    HP_HAND_LEFT = user.bodyparts[5].hp.Value,
                    HP_ARM_RIGHT = user.bodyparts[6].hp.Value,
                    HP_FOREARM_RIGHT = user.bodyparts[7].hp.Value,
                    HP_HAND_RIGHT = user.bodyparts[8].hp.Value,
                    HP_TORSO = user.bodyparts[9].hp.Value,
                    HP_PELVIS = user.bodyparts[10].hp.Value,
                    HP_THIGH_LEFT = user.bodyparts[11].hp.Value,
                    HP_CRUS_LEFT = user.bodyparts[12].hp.Value,
                    HP_FOOT_LEFT = user.bodyparts[13].hp.Value,
                    HP_THIGH_RIGHT = user.bodyparts[14].hp.Value,
                    HP_CRUS_RIGHT = user.bodyparts[15].hp.Value,
                    HP_FOOT_RIGHT = user.bodyparts[16].hp.Value,

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

                    barbarian = user.stats.barbarian.Value,
                    baseSpeed = user.stats.BASE_SPEED.Value,
                    initProf = user.stats.addProf2Init.Value

                };
                // Debug.LogWarning($"Username: {data.username} : ID: {id} : Level: {data.Lvl} : Chest Health: {data.HP_CHEST}");
                GameManager.Singleton.userDatas[i] = data;
            }
        }
    }
}
