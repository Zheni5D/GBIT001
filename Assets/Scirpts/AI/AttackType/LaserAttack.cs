using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : AttackArea
{
    protected float t = 0.2f;

    public LayerMask layerMask;
    private GameObject laser;
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider2D;
    private int oriBulletNum;
    public int bulletNum;
    private float ShootInterval = 0.01f;

    protected override void Awake()
    {
        base.Awake();
        oriBulletNum = bulletNum;
        MessageCenter.AddListener(OnGameRestart);
        laser = transform.GetChild(0).gameObject;
        lineRenderer = laser.GetComponent<LineRenderer>();
        edgeCollider2D = laser.GetComponent<EdgeCollider2D>();
    }

    private void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public bool TryAttack()
    {
        t += time.deltaTime;
        if (t >= ShootInterval)
        {
            Shoot();
            t -= ShootInterval;
            return true;
        }
        return false;
    }

    public void Shoot()
    {
        Vector3 direction = Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up;
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position + transform.up, direction.normalized, Mathf.Infinity, layerMask);

        lineRenderer.SetPosition(0, transform.position);
        Vector3 hitPoint = hit2D.point;

        lineRenderer.SetPosition(1, hitPoint);
        edgeCollider2D.SetPoints(new List<Vector2>() { Vector3.zero, transform.InverseTransformPoint(hitPoint) });
        laser.SetActive(true);
        SoundManager.PlayAudio("laser");
    }

    public void CloseLaser()
    {
        laser.SetActive(false);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        bulletNum = oriBulletNum;
        CloseLaser();
    }
}
