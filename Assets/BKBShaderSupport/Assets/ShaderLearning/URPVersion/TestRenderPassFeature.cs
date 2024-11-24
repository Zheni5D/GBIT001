using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PostSettings{
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material featureMaterial;
    }
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material material;
        public Color color;
        public float passIntensity;
        public TestPostVolume customVolume;
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //新建RT需要的一个类
            RenderTextureDescriptor rt = new RenderTextureDescriptor(Camera.main.pixelWidth , Camera.main.pixelHeight,RenderTextureFormat.Default,0);
            RenderTexture rtTex = new RenderTexture(rt);          // 新建贴图

            RenderTargetIdentifier cameraColorTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;        //获取相机的RT

            var strack = VolumeManager.instance.stack;                              //实例化到堆栈
            customVolume = strack.GetComponent<TestPostVolume>();           //从堆栈中获取到Volume上面的组件

            //获取一个新的命令缓冲区，并分配一个名称，命名命名缓冲区将为缓冲区执行饮食添加分析生成器
            CommandBuffer cmd = CommandBufferPool.Get(name:"testPostRenderPass");    //定义pass的名字，该名字会在FrameDebug窗口显示相应的pass
       
            material.SetFloat("_Intensity" , customVolume.Intensity.value);            
            // material.SetColor("_Color",color);
            material.SetColor("_Color" , customVolume.changeColor.value);       //使用Volume上面的参数替代了RenderFeature里面的参数

            cmd.Blit(cameraColorTexture , rtTex , material);      //对相机里面的画面进行一些操作，将相机获取到的RT图，通过material材质的计算，输出到tex
            cmd.Blit(rtTex,cameraColorTexture);                   //将上一步的tex，在重新写回相机
            
            context.ExecuteCommandBuffer(cmd);                   //自定义CommandBuffer执行
            cmd.Clear();                                         //清楚缓冲区的所有命令
            CommandBufferPool.Release(cmd);                      //释放CommandBuffer

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;
    public PostSettings settings = new PostSettings();

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
        m_ScriptablePass.material = settings.featureMaterial;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


