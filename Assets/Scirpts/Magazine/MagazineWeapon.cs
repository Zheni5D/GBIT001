using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum MagazineWeaponType{
    pistol,
    shotgun,
    test,
    knives,
    m4a1,
    lasergun
}
public class MagazineWeapon : ScriptableObject {
    public float rateOfFire;
    public int damage;
    [SerializeField]
    protected GameObject bulletPrefab;
    public int Ammo;
    public int maxAmmo;
    public MagazineWeaponType weaponType;
    public bool isAutomatic;//是否自动射击
    [SerializeField]
    protected GameObject shootVFX;
    public float shootSoundRadius;
    public string shootSFXname = "fire";
    public GameObject bulletShell;
    public virtual void Attack(Vector3 shootPos,Quaternion shootRot,bool canDamagePlayer = false){
        Debug.Log(1);
    }
}