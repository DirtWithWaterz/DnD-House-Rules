using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


[Serializable]
public struct userData:IEquatable<userData>,INetworkSerializable{

    public ulong id;
    public FixedString32Bytes username;

    public NetworkObjectReference objRef;

    public int Str, Dex, Con, Int, Wis, Cha;

    public int Lvl;

    public int BODY_NECK,
        BODY_HEAD, BODY_CHEST,
        BODY_ARM_LEFT, BODY_FOREARM_LEFT,
        BODY_HAND_LEFT, BODY_ARM_RIGHT,
        BODY_FOREARM_RIGHT, BODY_HAND_RIGHT,
        BODY_TORSO, BODY_PELVIS,
        BODY_THIGH_LEFT, BODY_CRUS_LEFT,
        BODY_FOOT_LEFT, BODY_THIGH_RIGHT,
        BODY_CRUS_RIGHT, BODY_FOOT_RIGHT;               

    public bool barbarian;
    public int baseSpeed;
    public bool initProf;

    public bool Equals(userData other)
    {
        return other.id == id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref objRef);

        serializer.SerializeValue(ref Str);
        serializer.SerializeValue(ref Dex);
        serializer.SerializeValue(ref Con);
        serializer.SerializeValue(ref Int);
        serializer.SerializeValue(ref Wis);
        serializer.SerializeValue(ref Cha);

        serializer.SerializeValue(ref Lvl);

        serializer.SerializeValue(ref BODY_NECK);
        serializer.SerializeValue(ref BODY_HEAD);
        serializer.SerializeValue(ref BODY_CHEST);
        serializer.SerializeValue(ref BODY_ARM_LEFT);
        serializer.SerializeValue(ref BODY_FOREARM_LEFT);
        serializer.SerializeValue(ref BODY_HAND_LEFT);
        serializer.SerializeValue(ref BODY_ARM_RIGHT);
        serializer.SerializeValue(ref BODY_FOREARM_RIGHT);
        serializer.SerializeValue(ref BODY_HAND_RIGHT);
        serializer.SerializeValue(ref BODY_TORSO);
        serializer.SerializeValue(ref BODY_PELVIS);
        serializer.SerializeValue(ref BODY_THIGH_LEFT);
        serializer.SerializeValue(ref BODY_CRUS_LEFT);
        serializer.SerializeValue(ref BODY_FOOT_LEFT);
        serializer.SerializeValue(ref BODY_THIGH_RIGHT);
        serializer.SerializeValue(ref BODY_CRUS_RIGHT);
        serializer.SerializeValue(ref BODY_FOOT_RIGHT);

        serializer.SerializeValue(ref barbarian);
        serializer.SerializeValue(ref baseSpeed);
        serializer.SerializeValue(ref initProf);

    }
}

public class GameManager : NetworkBehaviour
{

    public static GameManager Singleton;

    void Awake(){

        Singleton = Singleton != null && Singleton != this ? null : this;
    }

    public GameObject terminal;

    public NetworkList<userData> userDatas;


    void Start(){

        userDatas = new NetworkList<userData>();
        userDatas.Initialize(this);

        userDatas.OnListChanged += UpdateNames;
    }

    private void UpdateNames(NetworkListEvent<userData> changeEvent)
    {
        foreach(userData data in userDatas){

            if(data.objRef.TryGet(out NetworkObject netObj))
                netObj.name = data.username.ToString();
        }
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerDataRpc(NetworkObjectReference netRef, FixedString32Bytes username, ulong id){

        userData newUser = new userData{

            id = id,
            username = username,
            objRef = netRef
        };
        userDatas.Add(newUser);
    }

    public void OnEnable(){

        DontDestroyOnLoad(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
