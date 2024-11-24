using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseBehavior
{
    public float   maxAngluarSpeed;
    public float   maxRotateAngle;
    new private Rigidbody2D rigidbody;
    private Vector3 oriRight;
    private Vector3 curRight;
    private Vector3 lateRight;
    private Vector3 oriPos;
    private float oriZangle;
    new private Collider2D collider;
    public GameObject CollisionA;
    public GameObject CollisionB;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        Physics2D.maxRotationSpeed = maxAngluarSpeed;
        oriRight = transform.right;
        curRight = oriRight;
        lateRight = curRight;
        oriZangle = transform.rotation.eulerAngles.z;
        oriPos = transform.position;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation =  Quaternion.Euler(0,0,oriZangle);
            transform.position = oriPos;
            rigidbody.freezeRotation = false;
            curRight = oriRight;
            lateRight = curRight;
        }
    }
    
    private void FixedUpdate() {
        curRight = transform.right;
        // float angle = Vector3.SignedAngle(oriRight,curRight,Vector3.forward);
        // if (Mathf.Abs(angle) > maxRotateAngle && !rigidbody.freezeRotation)
        // {
        //     rigidbody.freezeRotation = true;
        //     if(angle > 0)
        //     {
        //         Quaternion quaternion = Quaternion.Euler(0,0,oriZangle + maxRotateAngle);
        //         transform.rotation = quaternion;
        //     }
        //     else
        //     {
        //         Quaternion quaternion = Quaternion.Euler(0,0,oriZangle - maxRotateAngle);
        //         transform.rotation = quaternion;
        //     }
        // }
        if(!rigidbody.freezeRotation)
            GetRotateDirection();
        
        lateRight = curRight;
    }

    public float GetRotateDirection()
    {
        Vector3 cross = Vector3.Cross(lateRight,curRight);
        if(cross.z > 0.1f)
        {
            //Debug.Log("逆时针");
            CollisionA.SetActive(true);
            CollisionB.SetActive(false);
            collider.isTrigger = true;
        }
        else if (cross.z < -0.1f)
        {
           // Debug.Log("顺时针");
            CollisionB.SetActive(true);
            CollisionA.SetActive(false);
            collider.isTrigger = true;
        }
        else
        {
            CollisionB.SetActive(false);
            CollisionA.SetActive(false);
            collider.isTrigger = false;
        }
        return cross.z;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position,new Vector3(1f,1f,1f));
    }
}


