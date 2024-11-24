using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunAttack : NormalAttack
{
    public float maxAngle;
    protected override void Shoot()
    {
        _bullet = Instantiate(bulletPrefab,transform.position,transform.rotation);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(true);
        for (int i = 0; i < 6; i++)
        {
            float t = Random.Range(-maxAngle,maxAngle);
            float angle = t + i * maxAngle * 2/5 - maxAngle;
            angle = Mathf.Clamp(angle,-maxAngle,maxAngle);
            Quaternion quaternion = Quaternion.identity;
            quaternion = Quaternion.Euler(0,0,angle);
            quaternion *= transform.rotation;
            _bullet = Instantiate(bulletPrefab,transform.position,quaternion);
            _bullet.GetComponent<Bullet>().SetCanDamagePlayer(true);
        }
        SoundManager.PlayAudio("fire_shotgun");
    }
}
