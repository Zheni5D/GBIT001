using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseBehavior
{
    public float speed = 10f;
    public float deadTime = 3f;protected float deadtime=0f;
    [SerializeField]private bool canBounceBack;
    [SerializeField]private LayerMask layer;
    [SerializeField][Range(1,10)]private int iteration;
    protected bool canDamagePlayer=false;
    private int _damage = 1;

    protected virtual void OnEnable() {
        MessageCenter.AddListener(OnGameRestart);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        Vector3 oriPos = transform.position;//记录原来的位置
        transform.Translate(speed * transform.up * time.deltaTime,Space.World);
        float length = (transform.position - oriPos).magnitude;//射线的长度
		Vector3 direction = transform.position - oriPos;//方向
        float deltaLength = length / iteration;
        RaycastHit2D hitinfo;
        for (int i = 0; i < iteration; i++)
        {

            oriPos = oriPos + deltaLength * direction.normalized * i;
            hitinfo = Physics2D.Raycast(oriPos, direction, deltaLength, layer);//在两个位置之间发起一条射线，然后通过这条射线去检测有没有发生碰撞
            //check
            if (hitinfo)
            {
                Collider2D other = hitinfo.collider;
                if (other.CompareTag("Breakable"))//子弹会穿透
                {
                    if (other.TryGetComponent(out Breakable breakableObject))
                    {
                        GameObject g = Instantiate(TheShitOfReference.AllShitOfReference.PrefabReference1, hitinfo.point, Quaternion.identity);
                        ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                        m.stopAction = ParticleSystemStopAction.Callback;
                        breakableObject.GetHurt(oriPos, hitinfo.point);
                        //Destroy(gameObject);
                    }
                    return;
                }
                if (other.CompareTag("Obstacle"))//子弹不会穿透 阻挡视野
                {
                    GameObject g = Instantiate(TheShitOfReference.AllShitOfReference.PrefabReference1, hitinfo.point, Quaternion.identity);
                    ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                    m.stopAction = ParticleSystemStopAction.Callback;
                    Destroy(gameObject);
                    return;
                }
                
                if (other.CompareTag("Shield") && !canDamagePlayer)//子弹不会穿透 不阻挡视野
                {
                    GameObject g = Instantiate(TheShitOfReference.AllShitOfReference.PrefabReference1, hitinfo.point, Quaternion.identity);
                    ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                    m.stopAction = ParticleSystemStopAction.Callback;
                    if (other.TryGetComponent(out Breakable breakableObject))
                    {
                        breakableObject.GetHurt(oriPos, hitinfo.point);
                    }
                    Destroy(gameObject);
                    return;
                }

                if (other.CompareTag("Enemy") && !canDamagePlayer)//子弹不会穿透 不阻挡视野
                {
                    other.GetComponent<simpleFSM>().GetDamaged(transform.up,_damage);
                    Destroy(gameObject);
                    return;
                }

                if (other.CompareTag("Player") && canDamagePlayer)//子弹不会穿透
                {
                    other.GetComponent<PlayerControl>().GetDamaged(null);
                    Destroy(gameObject);
                    return;
                }
            }

        }
        deadtime += time.deltaTime;
        if(deadtime >= deadTime) {
            Destroy(gameObject);
            deadtime -= deadTime;
        }        
    }

    protected virtual void OnDisable() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public void SetCanDamagePlayer(bool b)
    {
        canDamagePlayer = b;
    }

    public void BounceBack()
    {
        if(canBounceBack)
        {
            transform.up = -transform.up;
            canDamagePlayer = false;
        }
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        Destroy(gameObject);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

}
