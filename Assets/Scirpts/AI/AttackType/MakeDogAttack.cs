using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDogAttack : AttackArea
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        //makeTimer += Time.deltaTime;
    }

    public override void Attack()
    {
        //if (makeTimer > makeCD)
        //{
        //    Instantiate(dogPrefab, makeTransform.position, transform.rotation);
        //    makeTimer = 0;
        //}
    }
}
