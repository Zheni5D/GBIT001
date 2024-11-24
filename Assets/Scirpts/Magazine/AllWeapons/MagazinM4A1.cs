using UnityEngine;

[CreateAssetMenu(menuName = "Project-Blood/MagazinM4A1")]
public class MagazinM4A1 : MagazineWeapon
{
    [Header("精准度偏差")]
    [SerializeField]
    private float maxAngleOffset;
    public override void Attack(Vector3 shootPos, Quaternion shootRot, bool canDamagePlayer = false)
    {
        /*精准度偏差*/
        float angle = Random.Range(-maxAngleOffset,maxAngleOffset);
        Quaternion quaternion = Quaternion.identity;
        quaternion = Quaternion.Euler(0,0,angle);
        quaternion *= shootRot;
        GameObject _bullet = Instantiate(bulletPrefab,shootPos,shootRot);
        Instantiate(bulletShell, shootPos, shootRot);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(canDamagePlayer);
    }
}