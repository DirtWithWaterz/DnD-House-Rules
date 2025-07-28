using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Skills : NetworkBehaviour
{
    public NetworkList<skill> bucket = new NetworkList<skill>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    User user;

    public override void OnNetworkSpawn()
    {
        // Initialize bucket on the server (ensuring proper setup)
        if (IsServer)
        {
            bucket.Initialize(this);
        }

        user = transform.root.GetComponent<User>();
    }

    [Rpc(SendTo.Server)]
    public void AddToBucketRpc(skill skill)
    {
        int idx = 0;
        bool safe = true;
        foreach (skill s in bucket)
        {
            if (s.Equals(skill))
            {
                idx = bucket.IndexOf(s);
                safe = false;
                break;
            }
        }
        if (!safe)
            bucket[idx] = skill;
        else
            bucket.Add(skill);

        idx = bucket.IndexOf(skill);

        SetProfValRpc(idx, skill.proficiency);
    }

    [Rpc(SendTo.Everyone)]
    void SetProfValRpc(int idx, proficiency proficiency)
    {
        transform.GetChild(idx).GetComponent<Skill>().button.profVal = proficiency;
    }

    [Rpc(SendTo.Server)]
    public void RemoveFromBucketRpc(skill skill)
    {
        int idx = 0;
        bool safe = true;
        foreach (skill s in bucket)
        {
            if (s.Equals(skill))
            {
                idx = bucket.IndexOf(s);
                safe = false;
                break;
            }
        }
        if (!safe)
            bucket.RemoveAt(idx);
    }
}

[Serializable]
public struct skill:IEquatable<skill>,INetworkSerializable{

    public FixedString64Bytes id;
    public proficiency proficiency;

    public bool Equals(skill other)
    {
        return other.id == id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref proficiency);
    }
}
