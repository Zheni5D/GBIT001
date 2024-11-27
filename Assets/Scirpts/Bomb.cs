using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BaseBehavior
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision.transform.TryGetComponent(out PlayerControl playerControl))
        {
            playerControl.GetDamaged(null);
        }
        else if (collision.transform.CompareTag("Enemy") && collision.transform.TryGetComponent(out simpleFSM fsm))
        {
            fsm.GetDamaged(collision.transform.position - transform.position);
        }else if (collision.transform.CompareTag("Breakable") && collision.transform.TryGetComponent(out BombBox bomb))
        {
            bomb.ExplodeImmidiately();
        }
    }

    public void Destroy()
    {
        SoundManager.PlayAudio("bomb");
        Destroy(gameObject);
    }
}
