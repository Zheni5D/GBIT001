using UnityEngine;

[CreateAssetMenu(menuName = "Project-Blood/MagazinShotgun")]
public class MagazinShotgun : MagazineWeapon
{
    [Header("霰弹枪设置")]
    [SerializeField]
    private float maxAngle;
    [SerializeField]
    private int maxBulletCnt;
    public override void Attack(Vector3 shootPos, Quaternion shootRot, bool canDamagePlayer = false)
    {
        GameObject _bullet = Instantiate(bulletPrefab,shootPos,shootRot);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(canDamagePlayer);
        for (int i = 0; i < maxBulletCnt; i++)
        {
            float t = Random.Range(-maxAngle,maxAngle);
            float angle = t + i * maxAngle * 2/5 - maxAngle;
            angle = Mathf.Clamp(angle,-maxAngle,maxAngle);
            Quaternion quaternion = Quaternion.identity;
            quaternion = Quaternion.Euler(0,0,angle);
            quaternion *= shootRot;
            _bullet = Instantiate(bulletPrefab,shootPos,quaternion);
            _bullet.GetComponent<Bullet>().SetCanDamagePlayer(canDamagePlayer);
        }
        Instantiate(bulletShell, shootPos, shootRot);
    }
}
