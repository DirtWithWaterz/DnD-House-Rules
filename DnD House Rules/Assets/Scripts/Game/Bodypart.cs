using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodypart : MonoBehaviour
{

    [SerializeField]
    Description description;

    public SpriteRenderer sr;
    public bool selected = false;

    [SerializeField]
    bool deselect = false;

    public State status = State.Unknown;

    Health h;

    public bool shot = false;

    private bool WasBleeding = false;

    public bool vital = false;

    public int hp;

    public int maxHP;

    public int ac;

    void Awake(){

        sr = GetComponent<SpriteRenderer>();
        h = Component.FindObjectOfType<Health>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        status = 
                hp >= (maxHP*0.5f) && shot ? State.Bleeding : 
                hp >= maxHP ? State.Healthy : 
                hp >= (maxHP*0.75f) ? State.Stable : 
                hp >= (maxHP*0.5f) ? State.Brused : 
                hp >= (maxHP*0.25f) ? State.Critical : 
                hp >= 0 ? State.Emaciated : 
                hp >= -1 ? State.Paralyzed : 
                gameObject.name == "BODY_CHEST" ? State.Concaved : 
                State.Dismembered;
            
        if(h.VitalsNormal()){

            if(status == State.Bleeding){

                h.Bleeding = true;
                WasBleeding = true;
                h.ForceStatus(status);
            }
            else if(WasBleeding && status != State.Bleeding){

                h.Bleeding = false;
                WasBleeding = false;
            }
        }

        if(vital){
            
            switch(status){

                case State.Critical:
                    h.ForceStatus(status);
                    break;
                case State.Emaciated:
                    h.ForceStatus(State.Unconcious);
                    break;
                case State.Paralyzed:
                    h.ForceStatus(status);
                    break;
                case State.Dismembered:
                    h.ForceStatus(State.Deceased);
                    break;
                case State.Concaved:
                    h.ForceStatus(State.Deceased);
                    break;
                case State.Bleeding:
                    h.ForceStatus(status);
                    break;
                default:
                    if(h.VitalsNormal() && !h.Bleeding)
                        h.ForceStatus();
                    break;
            }
        }

        

        if(selected && !deselect){

            description.status = status.ToString();
            description.health = hp;
            description.ac = ac;
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

        description.status = status.ToString();
        description.health = hp;
        description.ac = ac;
        description.condition = this.gameObject.name;

        selected = true;
    }
}
