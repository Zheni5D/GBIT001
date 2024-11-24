using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private GameObject _bloodPrefab;
    private GameObject Player;
    private Collider2D attackBox;
    private LayerMask layer;
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        layer = 1;
        int i = LayerMask.NameToLayer("Obstacle");
        layer = layer << i;
        Player = GameObject.FindWithTag("Player");
        attackBox = GetComponent<Collider2D>();
        attackBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Vector3 dir = other.gameObject.transform.position - Player.transform.position;
            //     float angle = Vector3.SignedAngle(Vector3.right,dir,Vector3.forward);
            //     Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
            //    //生成血迹
            //    _bloodPrefab = Instantiate(BloodSplatPrefab,other.transform.position,quaternion);
            RaycastHit2D hitinfo = Physics2D.Raycast(Player.transform.position, dir, dir.magnitude * distance, layer);
            
            if (hitinfo.transform == null)//没有阻碍
            {
                other.GetComponent<simpleFSM>().GetDamaged(dir);
            }
        }
        if(other.CompareTag("Bullet"))
        {
            Vector3 dir = other.gameObject.transform.position - Player.transform.position;
        //     float angle = Vector3.SignedAngle(Vector3.right,dir,Vector3.forward);
        //     Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
        //    //生成血迹
        //    _bloodPrefab = Instantiate(BloodSplatPrefab,other.transform.position,quaternion);
            Bullet bullet = other.GetComponent<Bullet>();
            bullet.BounceBack();
            other.GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (other.TryGetComponent(out Breakable breakable))
        {
            breakable.GetHurt(transform.position,other.ClosestPoint(transform.position));
        }
    }

    public void StartAttack()
    {
        attackBox.enabled = true;
    }

    public void EndAttack()
    {
        attackBox.enabled = false;
    }
}
