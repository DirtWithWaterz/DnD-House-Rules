using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LvlBtn : MonoBehaviour
{

    Stats stat;


    User user;

    void Awake(){

        stat = transform.root.GetComponentInChildren<Stats>();

        user = transform.root.GetComponent<User>();
    }


    void OnMouseOver(){
        if(!user.isInitialized.Value)
            return;
        if(Input.GetMouseButtonDown(0) && stat.lvl.Value < 99){

            stat.lvl.Value++;
        }
        if(Input.GetMouseButtonDown(1) && stat.lvl.Value > 1){

            stat.lvl.Value--;
        }
        user.UpdateUserDataRpc(NetworkManager.Singleton.LocalClientId);
    }
}
