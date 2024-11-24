using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Bullet
{
    bool isFlying = true;

    
    protected override void Update()
    {
        if(isFlying)
        {
            transform.Translate(speed * transform.up * time.deltaTime,Space.World);
            deadtime += time.deltaTime;
            if(deadtime >= deadTime) {
                Destroy(gameObject);
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Obstacle"))
        {
            //播放枪打到墙上的特效
            isFlying = false;
            return;
        }
        if(other.CompareTag("Enemy") && isFlying)
        {
            other.GetComponent<simpleFSM>().GetDamaged(transform.up);
            isFlying = false;
            return;
        }
        if(other.CompareTag("Player") && !isFlying)
        {
            other.GetComponent<MagazineBaseShooting>().AmmoAmountChanged(1);
            Destroy(gameObject);
        }
    }

    

}
