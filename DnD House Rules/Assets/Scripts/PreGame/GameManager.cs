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

    public int HP_NECK,
        HP_HEAD, HP_CHEST,
        HP_ARM_LEFT, HP_FOREARM_LEFT,
        HP_HAND_LEFT, HP_ARM_RIGHT,
        HP_FOREARM_RIGHT, HP_HAND_RIGHT,
        HP_TORSO, HP_PELVIS,
        HP_THIGH_LEFT, HP_CRUS_LEFT,
        HP_FOOT_LEFT, HP_THIGH_RIGHT,
        HP_CRUS_RIGHT, HP_FOOT_RIGHT;
    
    public int AC_NECK,
        AC_HEAD, AC_CHEST,
        AC_ARM_LEFT, AC_FOREARM_LEFT,
        AC_HAND_LEFT, AC_ARM_RIGHT,
        AC_FOREARM_RIGHT, AC_HAND_RIGHT,
        AC_TORSO, AC_PELVIS,
        AC_THIGH_LEFT, AC_CRUS_LEFT,
        AC_FOOT_LEFT, AC_THIGH_RIGHT,
        AC_CRUS_RIGHT, AC_FOOT_RIGHT;

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

        serializer.SerializeValue(ref HP_NECK);
        serializer.SerializeValue(ref HP_HEAD);
        serializer.SerializeValue(ref HP_CHEST);
        serializer.SerializeValue(ref HP_ARM_LEFT);
        serializer.SerializeValue(ref HP_FOREARM_LEFT);
        serializer.SerializeValue(ref HP_HAND_LEFT);
        serializer.SerializeValue(ref HP_ARM_RIGHT);
        serializer.SerializeValue(ref HP_FOREARM_RIGHT);
        serializer.SerializeValue(ref HP_HAND_RIGHT);
        serializer.SerializeValue(ref HP_TORSO);
        serializer.SerializeValue(ref HP_PELVIS);
        serializer.SerializeValue(ref HP_THIGH_LEFT);
        serializer.SerializeValue(ref HP_CRUS_LEFT);
        serializer.SerializeValue(ref HP_FOOT_LEFT);
        serializer.SerializeValue(ref HP_THIGH_RIGHT);
        serializer.SerializeValue(ref HP_CRUS_RIGHT);
        serializer.SerializeValue(ref HP_FOOT_RIGHT);

        serializer.SerializeValue(ref AC_NECK);
        serializer.SerializeValue(ref AC_HEAD);
        serializer.SerializeValue(ref AC_CHEST);
        serializer.SerializeValue(ref AC_ARM_LEFT);
        serializer.SerializeValue(ref AC_FOREARM_LEFT);
        serializer.SerializeValue(ref AC_HAND_LEFT);
        serializer.SerializeValue(ref AC_ARM_RIGHT);
        serializer.SerializeValue(ref AC_FOREARM_RIGHT);
        serializer.SerializeValue(ref AC_HAND_RIGHT);
        serializer.SerializeValue(ref AC_TORSO);
        serializer.SerializeValue(ref AC_PELVIS);
        serializer.SerializeValue(ref AC_THIGH_LEFT);
        serializer.SerializeValue(ref AC_CRUS_LEFT);
        serializer.SerializeValue(ref AC_FOOT_LEFT);
        serializer.SerializeValue(ref AC_THIGH_RIGHT);
        serializer.SerializeValue(ref AC_CRUS_RIGHT);
        serializer.SerializeValue(ref AC_FOOT_RIGHT);

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

    public InqueCalendar inqueCalendar;


    void Start(){

        userDatas = new NetworkList<userData>();
        userDatas.Initialize(this);

        userDatas.OnListChanged += UpdateNames;

        inqueCalendar = GetComponent<InqueCalendar>();
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
