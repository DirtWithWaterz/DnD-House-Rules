using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bodypart : NetworkBehaviour
{

    [SerializeField]
    Description description;

    public SpriteRenderer sr;
    public bool selected = false;

    [SerializeField]
    bool deselect = false;

    public NetworkVariable<State> status = new NetworkVariable<State>(State.Unknown, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    Health h;
    User user;

    public bool shot = false;

    private bool WasBleeding = false;

    public bool vital = false;

    public NetworkVariable<int> hp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> maxHP = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> ac = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Awake(){

        sr = GetComponent<SpriteRenderer>();
        h = transform.root.GetComponentInChildren<Health>();

        user = transform.root.GetComponent<User>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
            return;
            
        status.Value = 
                hp.Value >= (maxHP.Value*0.5f) && shot ? State.Bleeding : 
                hp.Value >= maxHP.Value ? State.Healthy : 
                hp.Value >= (maxHP.Value*0.75f) ? State.Stable : 
                hp.Value >= (maxHP.Value*0.5f) ? State.Brused : 
                hp.Value >= (maxHP.Value*0.25f) ? State.Critical : 
                hp.Value >= 0 ? State.Emaciated : 
                hp.Value >= -1 ? State.Paralyzed : 
                gameObject.name == "BODY_CHEST" ? State.Concaved : 
                State.Dismembered;
            
        if(h.VitalsNormal()){

            if(status.Value == State.Bleeding){

                h.Bleeding = true;
                WasBleeding = true;
                h.ForceStatus(status.Value);
            }
            else if(WasBleeding && status.Value != State.Bleeding){

                h.Bleeding = false;
                WasBleeding = false;
            }
        }

        if(vital){
            
            switch(status.Value){

                case State.Critical:
                    h.ForceStatus(status.Value);
                    break;
                case State.Emaciated:
                    h.ForceStatus(State.Unconcious);
                    break;
                case State.Paralyzed:
                    h.ForceStatus(status.Value);
                    break;
                case State.Dismembered:
                    h.ForceStatus(State.Deceased);
                    break;
                case State.Concaved:
                    h.ForceStatus(State.Deceased);
                    break;
                case State.Bleeding:
                    h.ForceStatus(status.Value);
                    break;
                default:
                    if(h.VitalsNormal() && !h.Bleeding)
                        h.ForceStatus();
                    break;
            }
        }

        

        if(selected && !deselect){

            description.status = status.Value.ToString();
            description.health = hp.Value;
            description.ac = ac.Value;
            description.condition = this.gameObject.name;
        }
    }

    void OnMouseOver(){

        if(deselect)
            return;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
    }
    void OnMouseExit(){

        if(deselect)
            return;

        if(!selected)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
    void OnMouseDown(){

        user.UpdateUserDataRpc(NetworkManager.LocalClientId);
        foreach(GameObject g in h.body){

            Bodypart b = g.GetComponent<Bodypart>();
            if(b.selected){
                b.selected = false;
                b.sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
        }

        if(deselect){

            description.gameObject.SetActive(false);
            return;
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.7f);

        description.gameObject.SetActive(true);

        description.status = status.Value.ToString();
        description.health = hp.Value;
        description.ac = ac.Value;
        description.condition = this.gameObject.name;

        selected = true;
    }
}
