using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    public bool haveOwner;
    public Vector3 oriPos;
    public Quaternion oriRot;
    public MagazineWeapon template;
    private MagazineWeapon data;
    public bool isRanged;
    public AnimatorOverrideController controller;
    private float shootCd;
    public float timer = 0;
    public Material highlightMat;
    private Material defaultmat;

    private void Awake()
    {
        defaultmat = GetComponent<SpriteRenderer>().material;
        if (isRanged)
        {
            data = Instantiate(template);
            shootCd = 1 / data.rateOfFire;
        }
    }

    private void Start()
    {
        oriPos = transform.position;
        oriRot = transform.rotation;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public bool Attack()
    {
        if (data.Ammo <= 0)
        {
            return false;
        }

        if (timer < 0)
        {
            data.Ammo -= 1;
            SoundManager.PlayAudio(data.shootSFXname);
            data.Attack(transform.position, transform.rotation);
            HearingPoster.PostVoice(transform.position, data.shootSoundRadius);
            timer = shootCd;
            return true;
        }

        return false;
    }

    public bool IsAutomatic()
    {
        return data.isAutomatic;
    }

    public int GetAmmoAmount()
    {
        return data.Ammo;
    }

    public MagazineWeapon GetData()
    {
        return data;
    }

    public void SetAmmoAmount(int amount)
    {
        data.Ammo = amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerWeapon>().pickableWeapon.Enqueue(gameObject);
            GetComponent<SpriteRenderer>().material = highlightMat;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out PlayerWeapon playerWeapon))
            {
                playerWeapon.pickableWeapon.Dequeue();
            }
            GetComponent<SpriteRenderer>().material = defaultmat;
        }
    }

    public void ResetPositionAndRotation()
    {
        transform.position = oriPos;
        transform.rotation = oriRot;
    }
}