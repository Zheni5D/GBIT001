using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShock : PostProcessingBase
{

    public Shader ScreenShockShader;
    private Material ScreenShockMaterial;
    public Material material
    {
        get{
            if(ScreenShockMaterial == null)
                {
                    ScreenShockMaterial = CheckShaderAndCreateMaterial(ScreenShockShader,ScreenShockMaterial);
                }
                return ScreenShockMaterial;
        }
    }

    public Vector2 point = new Vector2(0.5f,0.5f);
    [Range(0,1.2f)]
    public float Radius = .05f;
    public float Aspect = .5f;
    [Range(0.01f,0.1f)]
    public float Width = 3f;
    [Range(0,1f)]
    public float Indensity = 1f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material != null)
        {
            material.SetVector("_Center",point);
            material.SetFloat("_Radius",Radius);
            material.SetFloat("_Aspect",Aspect);
            material.SetFloat("_Width",Width);
            material.SetFloat("_Indensity",Indensity);
            Graphics.Blit(src,dest,material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }

    //  void FixedUpdate()
    // {
    //     Radius += .02f;
    //     Radius %= 2f;
    // }

}
