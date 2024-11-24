Shader "Hidden/Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Indensity("Indensity",float) = 1.0
        _Frequency("Frequency",float) = 0
        _Offset("_Offset",float) = 0
        _LinesWidth("_LinesWidth",float) = 0
        _Amount("_Amount",float) = 0
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline" "DisableBatching"="True"}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USING_FREQUENCY_INFINITE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            uniform half4 _Params;
            uniform half4 _Params2;
            
            #define _Alpha _Params2.z

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float _Indensity;
            float _Frequency;
            float _LinesWidth;
            float _Offset;
            float _Amount;
            CBUFFER_END
            float randomNoise(float2 c)
            {
                return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            //随机数函数
            float randomNoise(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            }
            
            float trunc(float x, float num_levels)
            {
                return floor(x * num_levels) / num_levels;
            }
            
            float2 trunc(float2 x, float2 num_levels)
            {
                return floor(x * num_levels) / num_levels;
            }
            
            float3 rgb2yuv(float3 rgb)
            {
                float3 yuv;
                yuv.x = dot(rgb, float3(0.299, 0.587, 0.114));
                yuv.y = dot(rgb, float3(-0.14713, -0.28886, 0.436));
                yuv.z = dot(rgb, float3(0.615, -0.51499, -0.10001));
                return yuv;
            }
            
            float3 yuv2rgb(float3 yuv)
            {
                float3 rgb;
                rgb.r = yuv.x + yuv.z * 1.13983;
                rgb.g = yuv.x + dot(float2(-0.39465, -0.58060), yuv.yz);
                rgb.b = yuv.x + yuv.y * 2.03211;
                return rgb;
            }
            v2f vert (appdata v)
            {
                v2f o;
              
                // float3 offset = (0,0,0);
                // offset.x = 10 * randomNoise(_Time.y,2);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }
            
            

            half4 frag (v2f i) : SV_Target
            {
                

                float2 uv = i.uv;
                half _TimeX = _Time.x * 2.0;
		
                half strength = 0;
                #if USING_FREQUENCY_INFINITE
                    strength = 10;
                #else
                    strength = 0.5 + 0.5 * cos(_TimeX * _Frequency);
                #endif
                _TimeX = _TimeX * strength;

                //	[1] 生成随机强度梯度线条
                float truncTime = trunc(_TimeX, 4.0);
                float uv_trunc = randomNoise(trunc(uv.yy, float2(8, 8)) + 100.0 * truncTime);
                float uv_randomTrunc = 6.0 * trunc(_TimeX, 24.0 * uv_trunc);

                // [2] 生成随机非均匀宽度线条
                float blockLine_random = 0.5 * randomNoise(trunc(uv.yy + uv_randomTrunc, float2(8 * _LinesWidth, 8 * _LinesWidth)));
                blockLine_random += 0.5 * randomNoise(trunc(uv.yy + uv_randomTrunc, float2(7, 7)));
                blockLine_random = blockLine_random * 2.0 - 1.0;
                blockLine_random = sign(blockLine_random) * saturate((abs(blockLine_random) - _Amount) / (0.4));
                blockLine_random = lerp(0, blockLine_random, _Offset);

                
                // 生成源色调的blockLine Glitch
                float2 uv_blockLine = uv;
                uv_blockLine = saturate(uv_blockLine + float2(0.1 * blockLine_random, 0));
                half4 blockLineColor = tex2D(_MainTex, abs(uv_blockLine));

                // 将RGB转到YUV空间，并做色调偏移
                // RGB -> YUV
                float3 blockLineColor_yuv = rgb2yuv(blockLineColor.rgb);
                // adjust Chrominance | 色度
                blockLineColor_yuv.y /= 1.0 - 3.0 * abs(blockLine_random) * saturate(0.5 - blockLine_random);
                // adjust Chroma | 浓度
                blockLineColor_yuv.z += 0.125 * blockLine_random * saturate(blockLine_random - 0.5);
                float3 blockLineColor_rgb = yuv2rgb(blockLineColor_yuv);

                // 与源场景图进行混合
                half4 sceneColor = tex2D(_MainTex, i.uv);
                return lerp(sceneColor, float4(blockLineColor_rgb, blockLineColor.a), 1.0);
            }
            
            ENDHLSL
        }
    }
}
