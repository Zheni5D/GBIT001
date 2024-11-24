using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : BaseBehavior
{
    public Transform target;
    public float arriveDistance = 3f;
    [Tooltip("200f起步")]
    public float speedForce = 200f;
    public float speed;
    public float turnSpeed;

    private Rigidbody2D rigi;
    private Path path;
    private Seeker seeker;
    public bool reachedEndOfPath = false;
    private int pathIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigi = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f,1f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone() && !reachedEndOfPath)//防止重复更新
            //                                              确保后台加载路径不造成卡顿
            seeker.StartPath(rigi.position,target.position,OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            pathIndex = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null)
            return;
        
        //到达目的地
        if(pathIndex >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            rigi.velocity = Vector3.zero;
            Vector3 targetDir = target.position - transform.position;
            float playerDistance = targetDir.magnitude;
            turn(targetDir);
            if(playerDistance > arriveDistance)
                reachedEndOfPath = false;
            return;
        }else{
            reachedEndOfPath = false;
        }

        Vector2 dir = ((Vector2)path.vectorPath[pathIndex] - rigi.position).normalized;
        //Vector2 force = dir * speedForce * Time.deltaTime;
        time.rigidbody2D.velocity = dir * speed;
        //rigi.AddForce(force);
        //rigi.MovePosition(new Vector2(rigi.transform.position.x,rigi.transform.position.y) + dir * speedForce);
        turn(rigi.velocity.normalized);

        float distance = Vector2.Distance(rigi.position,path.vectorPath[pathIndex]);
        if(distance < arriveDistance)
        {
            pathIndex++;
        }
    }
    void turn(Vector3 targetDir)
    {
        targetDir = Vector3.RotateTowards(transform.up,targetDir,turnSpeed * time.deltaTime,0f);
        targetDir.z = 0;
        float angle = Vector3.SignedAngle(transform.up,targetDir,Vector3.forward);
        Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
        transform.rotation *= quaternion;
    }
}
