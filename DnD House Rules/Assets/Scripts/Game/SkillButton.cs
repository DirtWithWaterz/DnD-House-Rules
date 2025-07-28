using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    bool hovering;
    User user;

    public GameObject fill;
    public GameObject hlf;
    public GameObject blk;

    public proficiency profVal;

    public Skill skill;

    // Start is called before the first frame update
    void Start()
    {
        user = transform.root.GetComponent<User>();
    }

    IEnumerator OnMouseOver()
    {
        if (!user.isInitialized.Value)
            yield break;

        hovering = true;

        if (Input.GetMouseButtonDown(0))
        {

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            if (hovering)
            {

                switch (profVal)
                {
                    case proficiency.NONE:

                        profVal = proficiency.HALF;
                        break;
                    case proficiency.HALF:

                        profVal = proficiency.FULL;
                        break;
                    case proficiency.FULL:

                        profVal = proficiency.DOUBLE;
                        break;
                    case proficiency.DOUBLE:
                        break;
                }
                user.skills.AddToBucketRpc(new skill
                {
                    id = skill.id,
                    proficiency = profVal
                });
            }
        }
        if(Input.GetMouseButtonDown(1)){

            yield return new WaitUntil(() => Input.GetMouseButtonUp(1));
            if (hovering)
            {

                switch (profVal)
                {
                    case proficiency.NONE:
                        break;
                    case proficiency.HALF:

                        profVal = proficiency.NONE;
                        break;
                    case proficiency.FULL:

                        profVal = proficiency.HALF;
                        break;
                    case proficiency.DOUBLE:

                        profVal = proficiency.FULL;
                        break;
                }
                user.skills.AddToBucketRpc(new skill
                {
                    id = skill.id,
                    proficiency = profVal
                });
            }
        }
    }

    void OnMouseExit(){

        hovering = false;
    }
}
