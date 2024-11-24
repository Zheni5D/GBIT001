using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BaseBehavior
{
    public bool canHurtPlayer;
    public EdgeCollider2D EdgeCollider2D;
    private LineRenderer lineRenderer;
    private int layerMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.TryGetComponent(out simpleFSM fsm) && !canHurtPlayer)
        {
            fsm.GetDamaged(collision.transform.position - transform.position);
        }

        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerControl player) && canHurtPlayer)
        {
            player.GetDamaged(null);
        }

        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Breakable breakable))
        {
            if (lineRenderer == null) 
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            RaycastHit2D hit = Physics2D.Raycast(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0), Mathf.Infinity, LayerMask.GetMask("TransparentObstacle"));

            if (hit)
            {
                breakable.GetHurt(lineRenderer.GetPosition(0), hit.point);
                //if (collision.gameObject.layer == layerMask) 
                //{
                //    breakable.GetHurt(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
                //}
            }
            else
            {
                breakable.GetHurt(lineRenderer.GetPosition(0));
            }
        }
    }
}
