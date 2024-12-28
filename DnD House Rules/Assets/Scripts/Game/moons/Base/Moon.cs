using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new moon")]
public class Moon : ScriptableObject
{
    public new string name;
    public int cycle;
    public int shift;
    public Color color;

    public int currentPhase;
    public int cyclePos;
}
