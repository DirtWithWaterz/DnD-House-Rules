// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;


// [CreateAssetMenu(fileName = "new moon")]
// public class Moon : ScriptableObject
// {
//     public new string name;
//     public int cycle;
//     public int shift;
//     public Color color;

//     public int currentPhase;
//     public int cyclePos;

//     public static GameManager.JsonMoonData ToJson(Moon moon)
//     {

//         string color_s = $"{moon.color.r}:{moon.color.g}:{moon.color.b}:{moon.color.a}";

//         return new GameManager.JsonMoonData()
//         {

//             name = moon.name,
//             cycle = moon.cycle,
//             shift = moon.shift,
//             color = color_s,
//             currentPhase = moon.currentPhase,
//             cyclePos = moon.cyclePos
//         };
//     }

//     public static Moon FromJson(GameManager.JsonMoonData moon)
//     {

//         string[] color_s = moon.color.Split(':');

//         return new Moon()
//         {

//             name = moon.name,
//             cycle = moon.cycle,
//             shift = moon.shift,
//             color = new Color(int.Parse(color_s[0]), int.Parse(color_s[1]), int.Parse(color_s[2]), int.Parse(color_s[3])),
//             currentPhase = moon.currentPhase,
//             cyclePos = moon.cyclePos
//         };
//     }
// }
