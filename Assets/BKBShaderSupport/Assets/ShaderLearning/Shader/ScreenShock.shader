Shader "Hidden/ScreenShock"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center("Center",Vector) = (0.0,0.0,0.0,0.0)
        _Radius("Radius",Float) = 1.0
        _Aspect("Aspect",Float) = 1.0
        _Width("Width",Float) = 1.0
        _Indensity("Indensity",Float) = 1.0
    }
    SubShader
    {
        Tags{"RenderPipeline" = "UniversalPipeline"}
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
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            Vector _Center;
            float _Radius;
            half _Aspect;
            half _Width;
            float _Indensity;
            uniform half4 _MainTex_TexelSize;
            CBUFFER_END
            TEXTURE2D(_MainTex);       
            SAMPLER(sampler_MainTex);
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #if UNITY_UV_STARTS_AT_TOP

                if (_MainTex_TexelSize.y < 0.0){
                    o.uv.y = 1.0 - o.uv.y;
                }
                #endif
                return o;
            }

            

            half4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - _Center.xy;
                //_Aspect的作用是对圆进行修正，保证在不同的分辨率下都为正圆
                float edgeWidth = length(float2(dir.x * _Aspect, dir.y)) - _Radius;   //当前点离环形中边（以内外圆半径平均值为半径的圆）的距离

                //计算uv坐标偏移 _Width=(R2-R1)/2
                float sinX = _Width + edgeWidth;
                float weight = 2 * _Width * sin(3.14 / (2 * _Width) * sinX);  //正弦函数：2d*sin(2π/4d x) 非常的AMAZING这个函数
                float2 offsetUV = dir * weight;             //偏移量 = 偏移方向 * 偏移权重

                //最后uv取值，判断是否在环形区域内，环形区域外直接取原来uv，区域内计算偏移值  
                                                                                    //只要位于(0,1)之间就行
                float2 resultUV = lerp(i.uv, i.uv + offsetUV, step(abs(edgeWidth) > _Width, 0.4));
                resultUV = lerp(i.uv,resultUV,_Indensity);
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,resultUV);
                
                return col;
            }
            ENDHLSL
        }
    }
}
