using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class Weapons : BaseBehavior
{
    [SerializeField]
    protected bool canDamagePlayer=false;
    protected Collider2D other;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        this.other = other;
        if(other.CompareTag("Obstacle"))
        {
            OnHitObstacle();
            return;
        }
        if(other.CompareTag("Enemy") && !canDamagePlayer)
        {
            OnHitEnemy();
            return;
        }
        if(other.CompareTag("Player") && canDamagePlayer)
        {
            OnHitPlayer();
        }
    }

    public void SetCanDamagePlayer(bool b)
    {
        canDamagePlayer = b;
    }

    protected virtual void OnHitObstacle()
    {

    }

    protected virtual void OnHitEnemy()
    {

    }

    protected virtual void OnHitPlayer()
    {

    }

}
