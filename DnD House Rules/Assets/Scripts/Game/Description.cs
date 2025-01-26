using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Collections;
using Unity.Netcode;

public class Description : NetworkBehaviour
{

    [SerializeField]
    TMP_Text displayStatus;
    [SerializeField]
    TMP_Text displayHealth;
    [SerializeField]
    TMP_Text displayAc;
    [SerializeField]
    TMP_Text displayCondition;

    public string status;
    public int health;
    public int ac;
    public string condition;

    public Bodypart bodypart;

    public static Dictionary<string, byte> slotNum = new Dictionary<string, byte>(){

        {"BODY_HEAD", 1},
        {"BODY_NECK", 0},
        {"BODY_CHEST", 3},
        {"BODY_ARM_LEFT", 1},
        {"BODY_FOREARM_LEFT", 1},
        {"BODY_HAND_LEFT", 1},
        {"BODY_ARM_RIGHT", 1},
        {"BODY_FOREARM_RIGHT", 1},
        {"BODY_HAND_RIGHT", 1},
        {"BODY_TORSO", 2},
        {"BODY_PELVIS", 2},
        {"BODY_THIGH_LEFT", 1},
        {"BODY_CRUS_LEFT", 1},
        {"BODY_FOOT_LEFT", 1},
        {"BODY_THIGH_RIGHT", 1},
        {"BODY_CRUS_RIGHT", 1},
        {"BODY_FOOT_RIGHT", 1}
    };

    string oldStatus;
    int oldHealth;
    int oldAc;
    string oldCondition;

    public ArmorSlot[] armorSlots;
    int accessibleASS; // accessible armor slots lmfao

    void Start(){

        armorSlots = new ArmorSlot[3];
        for(int i = 0; i < armorSlots.Length; i++){

            NetworkObject slot = Instantiate(GameManager.Singleton.armorSlotDisplayObject, transform.GetChild(3));
            armorSlots[i] = slot.GetComponent<ArmorSlot>();
            slot.transform.localPosition += Vector3.down*75*i;
            slot.name = $"Slot {i+1}";

            armorSlots[i].description = this;
            armorSlots[i].index = i;

            // slot.Spawn();
        }

        this.gameObject.SetActive(false);
    }

    void Update(){

        if(status != oldStatus){

            displayStatus.text = $"Status: {status}";

            oldStatus = status;
        }
        if(health != oldHealth){

            displayHealth.text = $"Health: {health}";

            oldHealth = health;
        }
        if(ac != oldAc){

            displayAc.text = $"AC: {ac}";

            oldAc = ac;
        }
        if(condition != oldCondition){

            displayCondition.text = condition;

            oldCondition = condition;
        }

        if(bodypart != null)
            accessibleASS = slotNum[bodypart.name];
        else
            accessibleASS = 0;
        
        foreach(ArmorSlot armorSlot in armorSlots){

            armorSlot.gameObject.SetActive(false);
        }
        for(int i = 0; i < accessibleASS; i++){

            armorSlots[i].gameObject.SetActive(true);
        }

    }
}
