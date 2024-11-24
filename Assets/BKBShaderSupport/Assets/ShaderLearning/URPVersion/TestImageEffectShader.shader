Shader "Hidden/TestImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 texcoord     : TEXCOORD;
            };
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD;
            };

            TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float  _Intensity;
                float4 _Color;
            CBUFFER_END
           
            Varyings vert (Attributes v)
            {
                Varyings o=(Varyings)0;
                
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord , _MainTex);

                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                half4 FinalColor;

                float4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , i.uv);
                
                FinalColor = baseMap * _Intensity * _Color;

                return FinalColor;
            }
            ENDHLSL
        }
    }
}
