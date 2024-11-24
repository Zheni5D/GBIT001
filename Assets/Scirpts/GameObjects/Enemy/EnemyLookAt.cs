using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyLookAt : MonoBehaviour
{
    public float turnSpeed;
    GameObject player;
    AIPath ai;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ai = GetComponent<AIPath>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        turn();
        //float speed = ai.velocity.magnitude;
        //anim.SetFloat("speed",speed);
    }

    void turn()
    {
        //计算角度
        Vector3 dir = player.transform.position - transform.position;
        dir = Vector3.RotateTowards(transform.up,dir,turnSpeed * Time.deltaTime,0f);
        dir.z = 0;
        float angle = Vector3.SignedAngle(transform.up,dir,Vector3.forward);
        Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
        transform.rotation *= quaternion;
    }
}
