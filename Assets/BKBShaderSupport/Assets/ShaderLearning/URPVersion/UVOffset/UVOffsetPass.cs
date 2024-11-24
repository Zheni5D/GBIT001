using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class UVOffsetPass : ScriptableRenderPass
{
    Material m_material;
    static readonly string Tag = "CustomRenderPassEvent";       //定义一个静态的字符创名称，用在FrameDebug的渲染事件中
    static readonly int MainTexID = Shader.PropertyToID("_MainTex");                    //设置渲染的主纹理
    static readonly int tempTexID = Shader.PropertyToID("_TempTexture");   
    public UVOffsetPass(RenderPassEvent renderEvent , Shader m_shader)
    {
        renderPassEvent = renderEvent;                               //设置渲染事件位置，这里的renderPassEvent是从ScriptableRenderPass里面得到的
        var shader = m_shader;                                       //传入shader
        if(shader == null){return;}
        m_material = CoreUtils.CreateEngineMaterial(m_shader);      //通过指定的shader创建材质
    }  

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
    }
    
    public UVOffsetVolume volume;
    RenderTargetIdentifier source;                                               //定义当前相机的图像
    RenderTexture RtTexture;                                                     //定义临时的存储的RT纹理

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(m_material == null)
        {
            Debug.LogError("材质初始化失败！");
        }
        if(!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("相机的后处理位未激活！");
            return;
        }
        //get volume
        var stack = VolumeManager.instance.stack;       //实例化volume到堆栈
        volume = stack.GetComponent<UVOffsetVolume>();    //获取Volume上的组件上添加的脚本
        if(volume == null)
        {
            Debug.LogError("Volumen组件获取失败!");
            return;
        }
        //固定操作
        //获取一个新的命令缓冲区，并分配一个名称，命名命名缓冲区将为缓冲区执行饮食添加分析生成器
        CommandBuffer cmd = CommandBufferPool.Get(Tag);          //定义pass的名字，该名字会在FrameDebug窗口显示相应的pass
        OnRenderImage(cmd , renderingData);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
       
    }

    private void OnRenderImage(CommandBuffer cmd , RenderingData renderingData) {
        source = renderingData.cameraData.renderer.cameraColorTargetHandle;     //
        RenderTextureDescriptor tempDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        int rtWidth = tempDescriptor.width;
        int rtHeight = tempDescriptor.height;

        cmd.SetGlobalTexture(MainTexID , source);
        cmd.GetTemporaryRT(tempTexID , rtWidth , rtHeight , depthBuffer:0 , FilterMode.Trilinear , format:RenderTextureFormat.Default);

         //将volume里面的值传递到shader对应的属性
        m_material.SetFloat("_Indensity" , volume._Indensity.value);

        //固定操作
        cmd.Blit(source , tempTexID , m_material);           //将从相机获取到的图像通过材质shader里面的计算，传递到定义的临时RT贴图
        cmd.Blit(tempTexID,source);       
        
        cmd.ReleaseTemporaryRT(tempTexID); //将计算完成之后释放申请的临时RT
    }

        
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }
}
