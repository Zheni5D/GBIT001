using UnityEngine;

[CreateAssetMenu(menuName = "Project-Blood/MagazinePistol")]
public class MagazinePistol : MagazineWeapon
{
    public override void Attack(Vector3 shootPos, Quaternion shootRot, bool canDamagePlayer = false)
    {
        GameObject _bullet = Instantiate(bulletPrefab,shootPos,shootRot);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(canDamagePlayer);
        Instantiate(bulletShell, shootPos, shootRot);
    }
}
