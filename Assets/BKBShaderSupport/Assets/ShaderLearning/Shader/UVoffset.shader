Shader "Hidden/UVoffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Indensity("Indensity",float) = 1.0
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


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

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }
            //随机数函数
            float randomNoise(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            }
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float _Indensity;
            CBUFFER_END

            half4 frag (v2f i) : SV_Target
            {
                float splitAmount = _Indensity * randomNoise(_Time.y, 2);

                half3 finalCol;
                finalCol.r = tex2D(_MainTex, half2(i.uv.x + splitAmount,i.uv.y)).r;
                finalCol.g = tex2D(_MainTex, i.uv).g;
                finalCol.b = tex2D(_MainTex, half2(i.uv.x - splitAmount,i.uv.y)).b;
                finalCol *= (1.0 - splitAmount * 0.5);
                //finalCol = tex2D(_MainTex,fixed2(i.uv.x + splitAmount,i.uv,y));

                return half4(finalCol,1.0);
            }
            ENDHLSL
        }
    }
}
