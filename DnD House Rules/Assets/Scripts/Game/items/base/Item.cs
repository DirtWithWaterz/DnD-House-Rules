using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct item:IEquatable<item>,INetworkSerializable{

    public FixedString32Bytes name;
    public int cost;
    public int value;
    public Type type;
    public Size size;
    public int amount;
    public int weight;

    public bool Equals(item other)
    {
        return other.name == name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref cost);
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref size);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref weight);
    }
}

public enum Type{

    other = -1,
    food = 0,
    medical = 1,
    weapon = 2,
    armor = 3,
    capacityMult = 4,
    capacityMultL = 5,
    capacityMultS = 6,
    capacityMultT = 7
}
public enum Size{

    T = 0,
    S = 1,
    L = 2
}