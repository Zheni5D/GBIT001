using System;
using System.Collections;
using System.Collections.Generic;
using Chronos;
using UnityEngine;

public class LaserTrap : RoutineTrap
{
    [SerializeField]private LayerMask laserMask;
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider2D;
    private GameObject laser;
    private bool laserOn;
    private void Start()
    {
        laser = transform.GetChild(0).gameObject;
        lineRenderer = laser.GetComponent<LineRenderer>();
        edgeCollider2D = laser.GetComponent<EdgeCollider2D>();
    }

    private void FixedUpdate()
    {
        base.Update();
        if (laserOn)
        {
            Vector3 direction = Quaternion.Euler(transform.rotation.eulerAngles) * Vector2.up;
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position + transform.up, direction.normalized, Mathf.Infinity, laserMask);

            lineRenderer.SetPosition(0, laser.transform.position);
            Vector3 hitPoint = hit2D.point;

            lineRenderer.SetPosition(1, hitPoint);
            edgeCollider2D.SetPoints(new List<Vector2>() { Vector3.zero, transform.InverseTransformPoint(hitPoint) });
            laser.SetActive(true);
            SoundManager.PlayAudio("laser");
        }
        //transform.RotateAround(transform.position, Vector3.forward, 360f * time.fixedDeltaTime);
    }

    protected override void TrapOn()
    {
        laserOn = true;
        
    }

    protected override void TrapOff()
    {
        laserOn = false;
        laser.SetActive(false);
    }
}
