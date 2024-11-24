using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MagazineBaseShooting
{
    private Animator animator;
    public AnimatorOverrideController meleeController;
    private AnimatorOverrideController rangedController;
    private bool meleeAttack;

    private ReplaySystem replaySystem;
    public GameObject meleeWeapon, rangedWeapon;

    private RaycastHit2D hit;
    private simpleFSM target;

    public Queue<GameObject> pickableWeapon = new Queue<GameObject>();
    private bool pickedThisFrame;

    private bool lastMelee;
    private int lastAmmo = 0;
    private Weapon weapon;

    [SerializeField] private GameObject lastMeleeWeapon, lastRangedWeapon;

    private void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnMoveNextStage);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = meleeController;
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnMoveNextStage);
    }

    private void Start()
    {
        weapon = transform.GetChild(1).GetComponent<Weapon>();
        meleeAttack = true;
        meleeWeapon = transform.GetChild(0).gameObject;
        //weaponPos = meleeWeapon.transform.localPosition;
        lastMeleeWeapon = meleeWeapon;
        lastMelee = true;

        replaySystem = FindObjectOfType<ReplaySystem>();
    }

    private void Update()
    {
        if (!replaySystem.IsReplaying())
        {
            if (pickedThisFrame)
            {
                pickedThisFrame = false;
            }

            if (rangedWeapon)
            {
                rangedWeapon.GetComponent<WeaponItem>().timer -= Time.deltaTime; // 因为rangedWeapon被SetActive(false)了，Update不运行，不能正常计算开火CD,所以在这里计算
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (pickableWeapon.Count > 0)
                {
                    PickWeapon(pickableWeapon.Peek());
                    EndAttack();
                }
                else
                {
                    SwitchWeapon();
                    EndAttack();
                }
            }

            if (meleeAttack && Input.GetMouseButtonDown(0))
            {
                SoundManager.PlayAudio(string.Concat("slash", Random.Range(1, 4).ToString()));
                animator.SetTrigger("Attack");
            }
            else if (!meleeAttack)
            {
                if (rangedWeapon && rangedWeapon.TryGetComponent(out WeaponItem weaponItem))
                {
                    if (weaponItem.IsAutomatic() && Input.GetMouseButton(0))
                    {
                        if (weaponItem.Attack())
                        {
                            AmmoAmountChanged(-1);
                            ShakeController.Instance.Shake(-transform.up);
                            animator.SetTrigger("Attack");
                        }
                        else if (weaponItem.GetData() is MagazineLaserGun laserGun && weaponItem.GetData().Ammo == 0)
                        {
                            Destroy(laserGun.laser);
                        }
                    }
                    else if (!weaponItem.IsAutomatic() && Input.GetMouseButtonDown(0))
                    {
                        if (weaponItem.Attack())
                        {
                            AmmoAmountChanged(-1);
                            ShakeController.Instance.Shake(-transform.up);
                            animator.SetTrigger("Attack");
                        }
                    }
                    else if (weaponItem.GetData() is MagazineLaserGun laserGun)
                    {
                        Destroy(laserGun.laser);
                    }
                }
            }

            //AimTarget();
        }
    }

    private void FixedUpdate()
    {
        if (rangedWeapon && rangedWeapon.activeInHierarchy)
        {
            hit = Raycast(transform.position, transform.up, 15f, LayerMask.GetMask("Enemy"));
        }
    }

    private void AimTarget()
    {
        if (hit)
        {
            if (target == null)
            {
                target = hit.collider.GetComponent<simpleFSM>();
            }
            else if (target != hit.collider.GetComponent<simpleFSM>())
            {
                target.paramater.targeted = false;
                target = hit.collider.GetComponent<simpleFSM>();
            }
            target.paramater.targeted = true;
        }
        else
        {
            if (target)
            {
                target.paramater.targeted = false;
                target = null;
            }
        }
    }

    public void SwitchWeapon()
    {
        SoundManager.PlayAudio("changeWeapon");
        EndAttack();

        if (meleeAttack && rangedController)
        {
            animator.runtimeAnimatorController = rangedController;
            meleeAttack = !meleeAttack;
        }
        else if (!meleeAttack)
        {
            if (rangedWeapon && rangedWeapon.GetComponent<WeaponItem>().GetData() is MagazineLaserGun laser)
            {
                Destroy(laser.laser);
            }
            animator.runtimeAnimatorController = meleeController;
            meleeAttack = !meleeAttack;
        }
    }

    public void PickWeapon(GameObject newWeapon)
    {
        if (rangedWeapon && rangedWeapon.GetComponent<WeaponItem>().GetData() is MagazineLaserGun laser)
        {
            Destroy(laser.laser);
        }

        if (!pickedThisFrame)
        {
            SoundManager.PlayAudio("pickUpWeapon");
            if (newWeapon.TryGetComponent(out WeaponItem item)) // 捡到远程武器
            {
                if (item.isRanged)
                {
                    pickedThisFrame = true;

                    if (rangedWeapon != null) // 丢掉原远程武器
                    {
                        rangedWeapon.SetActive(true);
                        rangedWeapon.transform.SetParent(null);
                    }

                    rangedController = item.controller;
                    animator.runtimeAnimatorController = rangedController;

                    rangedWeapon = newWeapon;

                    rangedWeapon.SetActive(false);
                    rangedWeapon.transform.SetParent(transform);
                    rangedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));

                    meleeAttack = false;

                    Ammo = rangedWeapon.GetComponent<WeaponItem>().GetAmmoAmount();
                    AmmoAmountChanged(0);
                }
                else // 捡到近战武器
                {
                    pickedThisFrame = true;

                    if (meleeWeapon != null) // 丢掉原近战武器
                    {
                        meleeWeapon.SetActive(true);
                        meleeWeapon.transform.SetParent(null);
                    }

                    meleeController = item.controller;
                    animator.runtimeAnimatorController = meleeController;

                    meleeWeapon = newWeapon;
                    meleeWeapon.SetActive(false);
                    meleeWeapon.transform.SetParent(transform);

                    meleeAttack = true;
                }
            }
        }

    }

    void OnMoveNextStage(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;

        lastMeleeWeapon = meleeWeapon;
        lastRangedWeapon = rangedWeapon;
        lastAmmo = Ammo;
        lastMelee = meleeAttack;
    }

    void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;

        meleeAttack = lastMelee;
        meleeWeapon = lastMeleeWeapon;
        rangedWeapon = lastRangedWeapon;
            
        meleeController = meleeWeapon.GetComponent<WeaponItem>().controller;

        if (rangedWeapon == null)
        {
            rangedController = null;
        }
        Ammo = lastAmmo;
        AmmoAmountChanged(0);
        if (rangedWeapon && rangedWeapon.TryGetComponent(out WeaponItem weaponItem))
        {
            rangedController = weaponItem.controller;

            if (rangedWeapon.transform.parent != transform) // 进入Stage的武器被丢了，Restart时捡回来
            {
                rangedWeapon.SetActive(false);
                rangedWeapon.transform.SetParent(transform);
                rangedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            }
            weaponItem.SetAmmoAmount(Ammo);
        }

        if (meleeAttack)
        {
            animator.runtimeAnimatorController = meleeController;
        }
        else if (rangedWeapon)
        {
            animator.runtimeAnimatorController = rangedController;
        }
    }

    RaycastHit2D Raycast(Vector2 pos, Vector2 rayDirection, float length, LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, rayDirection, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos, rayDirection * length, color);
        return hit;
    }

    public void StartAttack()
    {
        weapon.StartAttack();
    }

    public void EndAttack()
    {
        weapon.EndAttack();
    }

    private void OnDrawGizmos()
    {
        if (rangedWeapon && rangedWeapon.TryGetComponent(out WeaponItem weapon))
        {
            Gizmos.DrawWireSphere(transform.position, weapon.template.shootSoundRadius);
        }
    }
}
