using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LegAnim : MonoBehaviour
{
    AIPath ai;
    Animator anim;
    Rigidbody2D rigi;
    // Start is called before the first frame update
    void Start()
    {
        Transform tmp;
        tmp = transform.parent;
        int i = 0;
        while(tmp.parent != null || i > 10)
        {
            tmp = tmp.parent;
            i++;
            if(tmp.TryGetComponent<Rigidbody2D>(out rigi))
            {
                break;
            }
        }
        ai = tmp.GetComponent<AIPath>();
        anim = GetComponent<Animator>();
        rigi = tmp.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ai != null)
        {
            float speed = ai.velocity.magnitude;
            anim.SetFloat("speed",speed);
        }
        else
        {
            float speed = rigi.velocity.magnitude;
            anim.SetFloat("speed",speed);
        }
    }
}
