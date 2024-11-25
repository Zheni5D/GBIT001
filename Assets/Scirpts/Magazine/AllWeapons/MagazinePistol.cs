using UnityEngine;

[CreateAssetMenu(menuName = "Project-Blood/MagazinePistol")]
public class MagazinePistol : MagazineWeapon
{
    public override void Attack(Vector3 shootPos, Quaternion shootRot, bool canDamagePlayer = false)
    {
        GenerateBullet(shootPos,shootRot,canDamagePlayer);   
        Instantiate(bulletShell, shootPos, shootRot);
    }
}
