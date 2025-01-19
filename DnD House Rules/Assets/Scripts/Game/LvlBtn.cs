using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LvlBtn : MonoBehaviour
{

    Stats stat;

    bool hovering;
    User user;

    void Awake(){

        stat = transform.root.GetComponentInChildren<Stats>();

        user = transform.root.GetComponent<User>();
    }


    IEnumerator OnMouseOver(){
        if(!user.isInitialized.Value)
            yield break;
        hovering = true;
        
        if(Input.GetMouseButtonDown(0) && stat.lvl.Value < 99){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            if(hovering)
                stat.lvl.Value++;
        }
        if(Input.GetMouseButtonDown(1) && stat.lvl.Value > 1){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
            if(hovering)
                stat.lvl.Value--;
        }
        // user.UpdateUserDataRpc(NetworkManager.Singleton.LocalClientId);
    }

    void OnMouseExit(){

        hovering = false;
    }
}
