using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FocusPower : PostProcessingBase
{
    public Shader onlyColorShader;
    private Material onlyColorMaterial;
    public Material material
    {
        get{
            if(onlyColorMaterial == null)
                {
                    onlyColorMaterial = CheckShaderAndCreateMaterial(onlyColorShader,onlyColorMaterial);
                }
                return onlyColorMaterial;
        }
    }

    public Color color;
    

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material != null)
        {
            material.SetColor("_Color",color);
            Graphics.Blit(src,dest,material);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }

    public void changeColor(bool b)
    {
        if(b)
            DOTween.To(() => color.b,a => color.b = a, 172/255f,.5f);
        else
             DOTween.To(() => color.b,a => color.b = a, 255/255f,.5f);
    }
}
