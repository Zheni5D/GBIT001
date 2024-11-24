using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class AttackArea : MonoBehaviour
{
    public enum AttackType
    {
        normal,jinzhan
    }
    public float attackArea;
    // public AttackType type;
    private LayerMask targetLayer;
    protected simpleFSM fSM;
    protected Timeline time;

    protected virtual void Awake() {
        fSM = transform.GetComponentInParent<simpleFSM>();
        targetLayer = fSM.paramater.targetLayer;
        time = fSM.time;
    }

    protected void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackArea);
    }

    public bool isInAttackArea()
    {
        if(Physics2D.OverlapCircle(transform.position,attackArea,targetLayer))
        {
            if(fSM.IsPlayerSighted())
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public virtual void Attack()
    {
        
    }

    public virtual void StopAttacking(bool value)
    {
        
    }

    public void SetTatgetLayer(LayerMask _l)
    {
        targetLayer = _l;
    }
}
