using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(simpleFSM))]
[ExecuteInEditMode]
public class EnemyAIMonitor : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color outlineColor;
    #if UNITY_EDITOR
    [SerializeField]private simpleFSM fsm;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        Refresh();
    }

    

    public simpleFSM GetFSM()
    {
        return fsm;
    }

    public void Refresh()
    {
        fsm = GetComponent<simpleFSM>();
    }

    //Enemy: can be selected from actor and stupid;optional:time crack;add patorl point
    #endif

    private void Start()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", outlineColor);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}
