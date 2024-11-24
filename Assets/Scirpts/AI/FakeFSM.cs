using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeFSM : actorFSM
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        loot = Instantiate(paramater.lootPrefab, transform.position, Yquaternion); // ��������
        WeaponItem lootItem = loot.GetComponent<WeaponItem>();
        lootItem.haveOwner = true;
        //Debug.Log(1);
        if (paramater.attackArea is NormalAttack rangedAttack)
            lootItem.GetComponent<WeaponItem>().SetAmmoAmount(rangedAttack.bulletNum);
    }

    public override void OnGameRestart(CommonMessage msg)
    {
        base.OnGameRestart(msg);

        if (!loot)
        {
            loot = Instantiate(paramater.lootPrefab, transform.position, Yquaternion); // ��������
            WeaponItem lootItem = loot.GetComponent<WeaponItem>();
            lootItem.haveOwner = true;
            Debug.Log(1);

            if (paramater.attackArea is NormalAttack rangedAttack)
                lootItem.GetComponent<WeaponItem>().SetAmmoAmount(rangedAttack.bulletNum);
        }
    }
}
