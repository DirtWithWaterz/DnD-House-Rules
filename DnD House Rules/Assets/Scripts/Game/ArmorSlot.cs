using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorSlot : MonoBehaviour
{
    public TMP_Text itemName, bonus;
    [SerializeField] RawImage background;

    public Description description;
    public int index;

    void OnMouseOver(){

        background.color = Color.white;
        itemName.color = Color.black;
        bonus.color = Color.black;
    }
    void OnMouseExit(){

        background.color = Color.black;
        itemName.color = Color.white;
        bonus.color = Color.white;
    }
}
