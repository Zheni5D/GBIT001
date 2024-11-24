using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : AttackArea
{
    public GameObject circlePrefab;
    public int bombNum = 1;
    public int oriBombNum;
    public GameObject bombCircle;

    protected override void Awake()
    {
        base.Awake();
        oriBombNum = bombNum;
        MessageCenter.AddListener(OnGameRestart);
    }

    public override void Attack()
    {
        if (bombNum > 0)
        {
            bombCircle = Instantiate(circlePrefab, transform.position, Quaternion.identity);
            bombNum--;
        }
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        bombNum = oriBombNum;
    }

    public void StopExplosion()
    {
        if (bombCircle)
            Destroy(bombCircle);
    }
}
