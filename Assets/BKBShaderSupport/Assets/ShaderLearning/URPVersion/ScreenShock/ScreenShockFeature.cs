using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenShockFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PostSettings{
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Shader shader;
    }
    ScreenShockPass m_ScriptablePass;
    public PostSettings settings = new PostSettings();

    /// <inheritdoc/>
    public override void Create()
    {
       //这里定义的name是在RenderFeature上面显示的名字，并不是FrameDebug面板的事件名称
        this.name = "ScreenShock";  
        //初始化渲染事件                
        m_ScriptablePass = new ScreenShockPass(settings.renderPassEvent , settings.shader);  

    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


