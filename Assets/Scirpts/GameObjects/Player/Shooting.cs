using UnityEngine;

public class Shooting : MagazineBaseShooting
{
    public GameObject BulletPrefab;
    public float ShootInterval;
    public Transform shootTrans;
    private GameObject _bulletPrefab;
    private float t = 0;
    private int maxAmmo = 10;
    // Start is called before the first frame update
    void Awake()
    {
        MessageCenter.AddListener(OnGameRestart);
    }

    // Update is called once per frame
    void Update()
    {
        if(t >= 0)
        {
            t -= time.deltaTime;
        }
        else
        {
            if(Input.GetMouseButtonDown(1))
            {
                Shoot();
                t = ShootInterval;
            }
        }
    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    void Shoot()
    {
        if(Ammo <= 0)
            return;
        _bulletPrefab = Instantiate(BulletPrefab,shootTrans.position,transform.rotation);
        _bulletPrefab.GetComponent<Bullet>().SetCanDamagePlayer(false);
        SoundManager.PlayAudio("fire");
        AmmoAmountChanged(-1);
        HearingPoster.PostVoice(transform.position,1.5f);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        AmmoAmountChanged(maxAmmo - Ammo);
    }
}
