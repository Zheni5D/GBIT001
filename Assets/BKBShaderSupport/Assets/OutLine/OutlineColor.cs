using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineColor : MonoBehaviour
{
    [SerializeField,ColorUsage(false,true)]
    private Color outlineColor;
    // Start is called before the first frame update
    void Start()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", outlineColor);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

    }
}
