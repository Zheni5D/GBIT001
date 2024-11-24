using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostProcessingBase : MonoBehaviour
{
    protected void CheckResources()
    {
        bool isSupported = CheckSupported();

        if(!isSupported)
            NotSupported();
    }

    protected bool CheckSupported()
    {
        return true;
    }
    // Start is called before the first frame update
    // Called when the platform doesn't support this effect
    protected void NotSupported() {
        enabled = false;
    }

    // 在Start中检查资源和条件是否满足
    protected void Start() {
        CheckResources();
    }

    protected Material CheckShaderAndCreateMaterial(Shader shader,Material material)
    {
        if(shader == null)
            return null;
        if(shader.isSupported && material && material.shader == shader)
            return material;
        else
        {
            //使用new创建临时材质
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if(material)
                return material;
            else
                return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
