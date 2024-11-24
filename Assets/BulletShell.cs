using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BulletShell : BaseBehavior
{
    public float speed;
    private Vector3 move;
    public float stopTime = .5f;
    public float fadeSpeed = .01f;
    new private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        speed = Random.Range(3, 6);
        float angel = Random.Range(-60f, 60f);
        move = Quaternion.AngleAxis(angel, Vector3.back) * -transform.up;
        StartCoroutine(Rotate());
    }

    private void Update()
    {
        transform.position += speed * time.deltaTime * move;
    }

    IEnumerator Rotate()
    {
        float rotateTime = 0;
        float rotateSpeed = Random.Range(0, 100);
        while (rotateTime < stopTime)
        {
            rotateTime += time.deltaTime;
            transform.Rotate(Vector3.back * time.deltaTime * rotateSpeed);
            yield return null;
        }

        StartCoroutine(Stop());
    }

    IEnumerator Stop()
    {
        move = Vector2.zero;

        while (sprite.color.a > 0)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, sprite.color.a - fadeSpeed);
            yield return new WaitForFixedUpdate();
        }
        
        Destroy(gameObject);
    }
}
