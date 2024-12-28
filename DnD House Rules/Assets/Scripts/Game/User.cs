using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class User : NetworkBehaviour
{
    NetworkVariable<bool> isInitialized = new NetworkVariable<bool>(false);
    Camera cam;
    public GameObject screen;

    NetworkVariable<int> clientsReady = new NetworkVariable<int>(0);
    NetworkVariable<int> connectedClients = new NetworkVariable<int>(0);

    Health health;
    Stats stats;
    InqueCalendar calendar;

    List<Bodypart> bodyparts;

    Interpreter interpreter;

    IEnumerator Start()
    {
        if(!IsOwner){

            gameObject.SetActive(false);
            yield return false;
        }
        SetConnectedClientsNetworkVariableRpc();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Game");

        SceneManager.activeSceneChanged += SceneChanged;

        cam = transform.GetChild(0).GetComponent<Camera>();
        screen = transform.GetChild(1).gameObject;

        health = screen.GetComponentInChildren<Health>();
        stats = screen.GetComponentInChildren<Stats>();
        calendar = screen.GetComponentInChildren<InqueCalendar>();

        interpreter = FindObjectOfType<Interpreter>();

        bodyparts = new List<Bodypart>();

        interpreter.user = this;
        
        foreach(GameObject part in health.body){

            bodyparts.Add(part.GetComponent<Bodypart>());
        }
        screen.SetActive(false);
        ClientReadyRpc();
        if(IsHost){

            yield return new WaitUntil(() => clientsReady.Value==connectedClients.Value);
            InitializedRpc();
        }
    }
    [Rpc(SendTo.Server)]
    void ClientReadyRpc(){

        clientsReady.Value++;
    }
    [Rpc(SendTo.Server)]
    void InitializedRpc(){

        isInitialized.Value = true;
    }
    [Rpc(SendTo.Server)]
    void SetConnectedClientsNetworkVariableRpc(){

        connectedClients.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        if(arg1.name == "Game"){

            cam.enabled = true;
            screen.SetActive(true);
        }
    }

    void FixedUpdate(){
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
    void UpdateUserDataRpc(ulong id){
        
        if(!IsOwner)
            return;
        NetworkObject netObj = null;

        foreach(userData data in GameManager.Singleton.userDatas){
            
            Debug.Log($"Checking ID: {data.id}");
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

                    Str = user.stats.STR,
                    Dex = user.stats.DEX,
                    Con = user.stats.CON,
                    Int = user.stats.INT,
                    Wis = user.stats.WIS,
                    Cha = user.stats.CHA,

                    Lvl = user.stats.lvl,

                    BODY_HEAD = user.bodyparts[0].hp,
                    BODY_NECK = user.bodyparts[1].hp,
                    BODY_CHEST = user.bodyparts[2].hp,
                    BODY_ARM_LEFT = user.bodyparts[3].hp,
                    BODY_FOREARM_LEFT = user.bodyparts[4].hp,
                    BODY_HAND_LEFT = user.bodyparts[5].hp,
                    BODY_ARM_RIGHT = user.bodyparts[6].hp,
                    BODY_FOREARM_RIGHT = user.bodyparts[7].hp,
                    BODY_HAND_RIGHT = user.bodyparts[8].hp,
                    BODY_TORSO = user.bodyparts[9].hp,
                    BODY_PELVIS = user.bodyparts[10].hp,
                    BODY_THIGH_LEFT = user.bodyparts[11].hp,
                    BODY_CRUS_LEFT = user.bodyparts[12].hp,
                    BODY_FOOT_LEFT = user.bodyparts[13].hp,
                    BODY_THIGH_RIGHT = user.bodyparts[14].hp,
                    BODY_CRUS_RIGHT = user.bodyparts[15].hp,
                    BODY_FOOT_RIGHT = user.bodyparts[16].hp,

                    barbarian = user.stats.barbarian,
                    baseSpeed = user.stats.BASE_SPEED,
                    initProf = user.stats.addProf2Init

                };
                GameManager.Singleton.userDatas[i] = data;
            }
        }
    }
}
