using UnityEngine;

public class ShootingTest : MagazineBaseShooting
{
    //public MagazinWeaponContainer Weapons;
    //public Transform shootTrans;
    //MagazineWeapon curWeapon;
    //private int curWeaponIndex;
    //private Collider2D[] cacheCols;
    //#region 对确定的一个武器而言(更换之后就要变)
    //private float shootInterval;
    //private float t;
    //#endregion
    //#region 记录重开之前的状态
    //private int lastAmmo;
    //private int lastWeaponIndex;
    //#endregion

    //private void Awake() {
    //    MessageCenter.AddListener(OnGameRestart);
    //    MessageCenter.AddListener(OnMoveNextStage);
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    curWeapon = Weapons.MagazineWeapons[curWeaponIndex];
    //    Ammo = curWeapon.maxAmmo;
    //    lastAmmo = Ammo;
    //    lastWeaponIndex = curWeaponIndex;
    //    shootInterval = 1.0f / curWeapon.rateOfFire;
    //    cacheCols = new Collider2D[16];
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Tab))
    //        ChangeWeapon();
    //    if(curWeapon.isAutomatic)
    //    {
    //        if(t > 0)
    //        {
    //            t -= time.deltaTime;
    //        }else{
    //            if(Input.GetMouseButton(1))
    //            {
    //                Attack();
    //            }
    //        }
    //    }else{
    //        if(t > 0)
    //        {
    //            t -= time.deltaTime;
    //        }else{
    //            if(Input.GetMouseButtonDown(1))
    //            {
    //                Attack();
    //            }
    //        }
    //    }
    //}

    //private void OnDestroy() {
    //    MessageCenter.RemoveListner(OnGameRestart);
    //    MessageCenter.RemoveListner(OnMoveNextStage);
    //}

    //void Attack()
    //{
    //    if(Ammo <= 0)
    //        return;
    //    AmmoAmountChanged(-1);
    //    curWeapon.Attack(shootTrans.position,transform.rotation);
    //    t = shootInterval;
    //    SoundManager.PlayAudio(curWeapon.shootSFXname); 
    //    HearingPoster.PostVoice(transform.position,curWeapon.shootSoundRadius * 3,cacheCols);
    //}

    //void ChangeWeapon()
    //{
    //    curWeaponIndex = getWeaponIndex();
    //    curWeapon = Weapons.MagazineWeapons[curWeaponIndex];
    //    shootInterval = 1.0f / curWeapon.rateOfFire;
    //    Ammo = curWeapon.maxAmmo;//TODO: Pick Up Function
    //    AmmoAmountChanged(0);
    //}

    //void ChangeWeapon(int _index)
    //{
    //    curWeapon = Weapons.MagazineWeapons[_index];
    //    shootInterval = 1.0f / curWeapon.rateOfFire;
    //}

    //int getWeaponIndex()
    //{
    //    return (curWeaponIndex + 1) % Weapons.MagazineWeapons.Length;
    //}
    //void OnMoveNextStage(CommonMessage msg)
    //{
    //    if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
    //    lastAmmo = Ammo;
    //    lastWeaponIndex = curWeaponIndex;
    //}
    //void OnGameRestart(CommonMessage msg)
    //{
    //    if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
    //    AmmoAmountChanged(lastAmmo - Ammo);
    //    ChangeWeapon(lastWeaponIndex);
    //}

    //void OnDrawGizmos()
    //{
    //    if (curWeapon != null)
    //    {
    //        Gizmos.color = Color.white;
    //        Gizmos.DrawWireSphere(transform.position, curWeapon.shootSoundRadius * 3);
    //    }
    //}
}
