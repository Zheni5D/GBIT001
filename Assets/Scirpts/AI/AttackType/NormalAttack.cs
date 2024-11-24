using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : AttackArea
{
    protected float t = 0.2f;
    [SerializeField]
    [Tooltip("最低为0.2")]
    protected float ShootInterval = 0.5f;
    private int oriBulletNum;
    public int bulletNum;
    public GameObject bulletPrefab;
    protected GameObject _bullet;
    [SerializeField] private string sfxName;

    protected override void Awake()
    {
        base.Awake();
        oriBulletNum = bulletNum;
        MessageCenter.AddListener(OnGameRestart);
    }

    private void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public bool TryAttack()
    {
        t += time.deltaTime;
        if(t >= ShootInterval) {
            Shoot();
            t -= ShootInterval;
            bulletNum -= 1;
            return true;
        }
        return false;
    }
    protected virtual void Shoot()
    {
        _bullet = Instantiate(bulletPrefab,transform.position,transform.rotation);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(true);
        SoundManager.PlayAudio(sfxName);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        bulletNum = oriBulletNum;
    }
}
