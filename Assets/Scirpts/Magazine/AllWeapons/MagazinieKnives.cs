using UnityEngine;
[CreateAssetMenu(menuName = "Project-Blood/MagazinieKnives")]
public class MagazinieKnives : MagazineWeapon
{
    public override void Attack(Vector3 shootPos,Quaternion shootRot,bool canDamagePlayer = false)
    {
        GameObject _bullet = Instantiate(bulletPrefab,shootPos,shootRot);
        _bullet.GetComponent<Bullet>().SetCanDamagePlayer(canDamagePlayer);
        
        //弹药,音效以及听觉系统由玩家的手实现
    }
}
