using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVoffset : PostProcessingBase
{
    public Shader UVoffsetShader;
    private Material UVoffsetMaterial;
    public Material material
    {
        get{
            UVoffsetMaterial = CheckShaderAndCreateMaterial(UVoffsetShader,UVoffsetMaterial);
            return UVoffsetMaterial;
        }
    }

    [Range(0,0.1f)]
    public float Indensity;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material != null)
        {
            material.SetFloat("_Indensity",Indensity);
            Graphics.Blit(src,dest,material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }

}
