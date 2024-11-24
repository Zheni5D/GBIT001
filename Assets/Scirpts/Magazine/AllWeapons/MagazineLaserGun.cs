using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Project-Blood/MagazineLaserGun")]
public class MagazineLaserGun : MagazineWeapon
{
    public LayerMask layerMask;
    public GameObject laserPrefab;
    public GameObject laser;
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    public override void Attack(Vector3 shootPos, Quaternion shootRot, bool canDamagePlayer = false)
    {
        Vector2 direction = Quaternion.Euler(shootRot.eulerAngles) * Vector2.up;
        RaycastHit2D hit2D = Physics2D.Raycast(shootPos, direction.normalized, Mathf.Infinity, layerMask);

        if (laser == null)
        {
            laser = Instantiate(laserPrefab, shootPos, Quaternion.identity);
            lineRenderer = laser.GetComponent<LineRenderer>();
            edgeCollider = laser.GetComponent<EdgeCollider2D>();
        }
        else
        {
            lineRenderer.SetPosition(0, shootPos);
            lineRenderer.SetPosition(1, hit2D.point);

            Vector3 hitPoint = hit2D.point;
            edgeCollider.SetPoints(new List<Vector2>() { Vector3.zero, hitPoint - laser.transform.position });
        }
    }
}
