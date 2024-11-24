using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapons
{
    protected override void OnHitPlayer()
    {
        other.GetComponent<PlayerControl>().GetDamaged(null);
    }
}
