using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Collider2D[] colliders;
    // Start is called before the first frame update
    void Start()
    {
       colliders = GetComponentsInChildren<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //每隔x秒激活子物体y秒
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //damage others
    }
}
