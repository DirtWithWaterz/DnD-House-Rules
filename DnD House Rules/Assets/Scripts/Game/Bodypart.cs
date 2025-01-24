using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct itemSlot : IEquatable<item>, INetworkSerializable
{

    // public FixedString64Bytes itemName;
    // public int itemId;

    // public Type itemType;

    public item item;

    public byte slotType;

    public FixedString64Bytes bodypart;

    public bool Empty(){

        return item.id == -1;
    }

    public bool Equals(item other)
    {
        // return other.name == itemName && other.id == itemId;
        return other.name == item.name && other.id == item.id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // serializer.SerializeValue(ref itemName);
        // serializer.SerializeValue(ref itemId);
        // serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref item);
        serializer.SerializeValue(ref slotType);
        serializer.SerializeValue(ref bodypart);
    }
}

public class Bodypart : NetworkBehaviour
{

    [SerializeField]
    public Description description;

    public SpriteRenderer sr;
    public bool selected = false;

    [SerializeField]
    bool deselect = false;

    public NetworkVariable<State> status = new NetworkVariable<State>(State.Unknown, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    Health h;
    User user;

    [SerializeField] itemSlot[] slot;

    public bool shot = false;

    private bool WasBleeding = false;

    public bool vital = false;

    public NetworkVariable<bool> black = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> currentHP = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> maximumHP = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> ac = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<FixedString4096Bytes> condition = new NetworkVariable<FixedString4096Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Awake(){

        sr = GetComponent<SpriteRenderer>();
        h = transform.root.GetComponentInChildren<Health>();

        user = transform.root.GetComponent<User>();
    }

    void Start(){

        slot = new itemSlot[2];
        for(int i = 0; i < slot.Length; i++){

            slot[i] = new itemSlot{

                item = new item{

                    id = -1
                },
                slotType = (byte)i,
                bodypart = gameObject.name
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
            return;
        if(user.isInitialized.Value){
            if(black.Value){

                switch(gameObject.name){

                    case "BODY_ARM_LEFT":
                        user.bodyparts[4].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_FOREARM_LEFT":
                        user.bodyparts[5].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_HAND_LEFT":
                        sr.enabled = false;
                        break;
                    case "BODY_ARM_RIGHT":
                        user.bodyparts[7].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_FOREARM_RIGHT":
                        user.bodyparts[8].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_HAND_RIGHT":
                        sr.enabled = false;
                        break;
                    
                    case "BODY_THIGH_LEFT":
                        user.bodyparts[12].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_CRUS_LEFT":
                        user.bodyparts[13].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_FOOT_LEFT":
                        sr.enabled = false;
                        break;
                    case "BODY_THIGH_RIGHT":
                        user.bodyparts[15].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_CRUS_RIGHT":
                        user.bodyparts[16].black.Value = true;
                        sr.enabled = false;
                        break;
                    case "BODY_FOOT_RIGHT":
                        sr.enabled = false;
                        break;
                }
            }
            if(!black.Value){

                switch(gameObject.name){

                    case "BODY_ARM_LEFT":
                        user.bodyparts[4].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_FOREARM_LEFT":
                        user.bodyparts[5].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_HAND_LEFT":
                        sr.enabled = true;
                        break;
                    case "BODY_ARM_RIGHT":
                        user.bodyparts[7].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_FOREARM_RIGHT":
                        user.bodyparts[8].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_HAND_RIGHT":
                        sr.enabled = true;
                        break;
                    
                    case "BODY_THIGH_LEFT":
                        user.bodyparts[12].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_CRUS_LEFT":
                        user.bodyparts[13].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_FOOT_LEFT":
                        sr.enabled = true;
                        break;
                    case "BODY_THIGH_RIGHT":
                        user.bodyparts[15].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_CRUS_RIGHT":
                        user.bodyparts[16].black.Value = false;
                        sr.enabled = true;
                        break;
                    case "BODY_FOOT_RIGHT":
                        sr.enabled = true;
                        break;
                }
            }
        }
        status.Value = 
                currentHP.Value >= (maximumHP.Value*0.5f) && shot ? State.Bleeding : 
                currentHP.Value >= maximumHP.Value ? State.Healthy : 
                currentHP.Value >= (maximumHP.Value*0.75f) ? State.Stable : 
                currentHP.Value >= (maximumHP.Value*0.5f) ? State.Brused : 
                currentHP.Value >= (maximumHP.Value*0.25f) ? State.Critical : 
                currentHP.Value >= 0 ? State.Emaciated : 
                currentHP.Value >= -1 ? State.Paralyzed : 
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
            description.health = currentHP.Value;
            description.ac = ac.Value;
            description.condition = condition.Value.ToString();
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

        // user.UpdateUserDataRpc(NetworkManager.LocalClientId);
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
        description.health = currentHP.Value;
        description.ac = ac.Value;
        description.condition = condition.Value.ToString();

        selected = true;
    }
}
