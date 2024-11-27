using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : RoutineTrap
{
    private Collider2D[] colliders;
    [SerializeField]private Sprite spikeOnSprite;
    [SerializeField]private Sprite spikeOffSprite;
    // Start is called before the first frame update
    void Start()
    {
       colliders = GetComponentsInChildren<Collider2D>();
    }



    protected override void TrapOn()
    {
        foreach (Collider2D c in colliders)
        {
            c.enabled = true;
            c.gameObject.GetComponent<SpriteRenderer>().sprite = spikeOnSprite;
        }
    }

    protected override void TrapOff()
    {
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
            c.gameObject.GetComponent<SpriteRenderer>().sprite = spikeOffSprite;
        }
    }
}
