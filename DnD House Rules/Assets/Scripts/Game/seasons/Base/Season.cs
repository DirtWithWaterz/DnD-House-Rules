using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "new season")]
public class Season : ScriptableObject
{
    public new string name;

    [Header("Duration:")]
    public int duration;

    [Header("Sunrise:")]
    public int rise_hour;
    public int rise_minute;

    [Header("Sunset:")]
    public int set_hour;
    public int set_minute;
}
