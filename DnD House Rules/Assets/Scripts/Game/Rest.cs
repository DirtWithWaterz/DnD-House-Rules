using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Rest : NetworkBehaviour
{

    public static Dictionary<string, int> bodypartDict0 = new Dictionary<string, int>(){

        {"head", 0},
        {"neck", 1},
        {"chest", 2},
        {"left arm", 3},
        {"left forearm", 4},
        {"left hand", 5},
        {"right arm", 6},
        {"right forearm", 7},
        {"right hand", 8},
        {"torso", 9},
        {"pelvis", 10},
        {"left thigh", 11},
        {"left crus", 12},
        {"left foot", 13},
        {"right thigh", 14},
        {"right crus", 15},
        {"right foot", 16}
    };
    public static Dictionary<int, string> bodypartDict1 = new Dictionary<int, string>(){

        {0, "head"},
        {1, "neck"},
        {2, "chest"},
        {3, "left arm"},
        {4, "left forearm"},
        {5, "left hand"},
        {6, "right arm"},
        {7, "right forearm"},
        {8, "right hand"},
        {9, "torso"},
        {10, "pelvis"},
        {11, "left thigh"},
        {12, "left crus"},
        {13, "left foot"},
        {14, "right thigh"},
        {15, "right crus"},
        {16, "right foot"}
    };

    public Bodypart selectedBodypart;
    public int selectedBodypartIndex;
    TMP_Dropdown dropdown;

    User user;

    void Start(){
        
        dropdown = GetComponentInChildren<TMP_Dropdown>();
        user = transform.root.GetComponent<User>();
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        for(int i = 0; i < user.bodyparts.Count; i++){

            if(!user.bodyparts[i].black.Value){

                optionDatas.Add(new TMP_Dropdown.OptionData(){

                    text = bodypartDict1[i]
                });
            }
        }
        dropdown.options.Clear();
        dropdown.AddOptions(optionDatas);

        ChangeSelectedBodypart();
    }

    public void ChangeSelectedBodypart(){

        selectedBodypart = user.bodyparts[bodypartDict0[dropdown.captionText.text]];
        for(int i = 0; i < user.bodyparts.Count; i++){

            if(selectedBodypart == user.bodyparts[i]){

                selectedBodypartIndex = i;
            }
        }
    }

    public void DoRest(int type){

        if(!IsOwner)

        switch(type){

            case 0:
                if(selectedBodypart.currentHP.Value < selectedBodypart.maximumHP.Value)
                    selectedBodypart.currentHP.Value += 1;
                break;
            case 1:
                break;
        }
        gameObject.SetActive(false);
    }
}
