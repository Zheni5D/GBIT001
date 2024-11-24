using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    public Transform ATrans;
    public Transform BTrans;
    public Vector3 size;
    public LayerMask mask;
    public float force;
    new private Rigidbody2D rigidbody;
    private Quaternion oriRot;
    private Vector3 oriRight;
    private Collider2D[] collider2Ds;
    private Vector3 curRight;
    private Vector3 lateRight;
    private int isInvClock=0;//1=逆时针 -1=顺时针 0=不动
    private void OnEnable() {
        MessageCenter.AddListener(OnGameRestart);
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        oriRot = transform.rotation;
        oriRight = transform.right;
        curRight = oriRight;
        lateRight = curRight;
        collider2Ds = new Collider2D[8];
        //size = new Vector3(transform.localScale.x * size.x,size.y * transform.localScale.y);
    }

    private void FixedUpdate() {
        curRight = transform.right;
        float z = GetRotateDirection();
        if(z > 0.1f)
        {
            isInvClock = 1;
        }
        else if (z < -0.1f)
        {
            isInvClock = -1;
        }
        else
        {
            isInvClock = 0;
        }
        float angle = Vector3.SignedAngle(oriRight,transform.right,Vector3.forward);
        if(0 != Physics2D.OverlapBoxNonAlloc(ATrans.position,size,angle,collider2Ds,mask))
        {
            rigidbody.AddForce(-transform.up * force);
        }
        if(0 != Physics2D.OverlapBoxNonAlloc(BTrans.position,size,angle,collider2Ds,mask))
        {
            rigidbody.AddForce(transform.up * force);
        }
        lateRight = curRight;
        
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Enemy"))
        {
            Vector3 dir = transform.position - other.gameObject.transform.position;
            float dot = Vector3.Dot(transform.up,dir);
            //dot < 0: other物体在this物体A面 (transform.up箭头所指那一面)
            //dot > 0: other物体在this物体B面 (transform.up箭头所指那反面)
            if(isInvClock == -1 && dot > 0 || isInvClock == 1 && dot < 0)
            {
                other.gameObject.GetComponent<simpleFSM>().BeatDown(Vector3.zero);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(ATrans.position,size);
        Gizmos.DrawWireCube(BTrans.position,size);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        transform.rotation = oriRot;
    }

    public float GetRotateDirection()
    {
        Vector3 cross = Vector3.Cross(lateRight,curRight);
        // if(cross.z > 0.1f)
        // {
        //     Debug.Log("逆时针");
        // }
        // else if (cross.z < -0.1f)
        // {
        //     Debug.Log("顺时针");
        // }
        return cross.z;
    }
}
