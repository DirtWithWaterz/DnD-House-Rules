using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConditionsUI : MonoBehaviour
{
    public List<NetworkObject> conditionDisplays;

    public List<condition> conditionsList = new List<condition>();

    public TMP_Text iPanel_conditionInfo;
    [SerializeField] GameObject Panel;

    [Rpc(SendTo.Everyone)]
    public void ListConditionRpc(condition condition, string usernameI){

        if(usernameI != GameManager.Singleton.interpreter.GetUsername)
            return;

        if(condition.name.ToString() == "normal"){

            for(int i = 0; i < conditionsList.Count; i++){
                
                if(conditionsList[i].bodypart == condition.bodypart){

                    conditionsList.RemoveAt(i);
                    i = 0;
                }
            }
        }

        for(int i = 0; i < conditionsList.Count; i++){
            
            if(conditionsList[i].Equals(condition))
                return;
        }
        conditionsList.Add(condition);
        RefreshConditionBoxRpc(usernameI);
    }

    [Rpc(SendTo.Everyone)]
    public void UnlistConditionRpc(condition condition, string usernameI){

        if(usernameI != GameManager.Singleton.interpreter.GetUsername)
            return;
        for(int i = 0; i < conditionsList.Count; i++){

            if(conditionsList[i].Equals(condition))
                conditionsList.RemoveAt(i);
        }
        RefreshConditionBoxRpc(usernameI);
    }

    [Rpc(SendTo.Everyone)]
    public void RefreshConditionBoxRpc(string usernameI){

        if(usernameI != GameManager.Singleton.interpreter.GetUsername)
            return;
        
        if(conditionDisplays.Count > 0){

            foreach(NetworkObject itemDis in conditionDisplays){

                if(itemDis != null)
                    Destroy(itemDis.gameObject);
            }
            conditionDisplays.Clear();
        }
        foreach(condition condition in conditionsList){

            if(condition.name == "normal")
                continue;

            NetworkObject conditionDisplayBox = Instantiate(GameManager.Singleton.conditionDisplayBox, Panel.transform);
            ConditionDisplay conditionDisplay = conditionDisplayBox.GetComponent<ConditionDisplay>();
            conditionDisplay.bodypartNameText.text = condition.bodypart.ToString();
            conditionDisplay.conditionNameText.text = condition.name.ToString();
            conditionDisplay.thisCondition = condition;
            conditionDisplay.conditionsUI = this;
            conditionDisplays.Add(conditionDisplayBox);
        }
    }

}
