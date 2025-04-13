using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConditionsUI : MonoBehaviour
{
    public List<NetworkObject> conditionDisplays;

    public List<condition> conditionsList = new List<condition>();

    public ConditionDisplay exhaustionDisplay;

    public TMP_Text iPanel_conditionInfo;
    [SerializeField] GameObject Panel;

    [Rpc(SendTo.Everyone)]
    public void ListConditionRpc(condition condition, string usernameI){

        Debug.Log($"{usernameI} == {GameManager.Singleton.interpreter.GetUsername} ?");
        if(usernameI != GameManager.Singleton.interpreter.GetUsername)
            return;
        Debug.Log("true >>>");

        if(condition.name.ToString() == "normal"){

            for(int i = 0; i < conditionsList.Count; i++){
                
                if(conditionsList[i].bodypart == condition.bodypart){
                    Debug.Log($"removing {conditionsList[i].name} from list because the status of {condition.bodypart} is now normal");
                    conditionsList.RemoveAt(i);
                    i = 0;
                }
            }
        }

        for(int i = 0; i < conditionsList.Count; i++){
            
            if(conditionsList[i].Equals(condition)){

                Debug.Log($"{condition.name} already added to list for the {condition.bodypart}");
                return;
            }
        }
        Debug.Log($"adding {condition.name} to the list of conditions for the {condition.bodypart}");
        conditionsList.Add(condition);
        Debug.Log("refreshing display box >>>");
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
        
        Debug.Log("<<< commencing refresh");

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
            Debug.Log($"adding display for {condition.name} on the {condition.bodypart} >>>");
            NetworkObject conditionDisplayBox = Instantiate(GameManager.Singleton.conditionDisplayBox, Panel.transform);
            ConditionDisplay conditionDisplay = conditionDisplayBox.GetComponent<ConditionDisplay>();
            conditionDisplay.bodypartNameText.text = condition.bodypart.ToString();
            conditionDisplay.conditionNameText.text = condition.name.ToString();
            conditionDisplay.thisCondition = condition;
            conditionDisplay.conditionsUI = this;
            conditionDisplays.Add(conditionDisplayBox);
        }

        Debug.Log("finished >>>");
    }

}
