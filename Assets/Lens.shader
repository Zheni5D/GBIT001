Shader "Custom/LensDistortionWithScaling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.2
        _ScaleFactor ("Scale Factor", Range(0, 1)) = 0.9 // 缩放因子
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _DistortionStrength;
            float _ScaleFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 Distort(float2 uv, float strength)
            {
                float2 center = float2(0.5, 0.5);
                float2 dist = uv - center;
                float len = length(dist);
                dist *= 1 + len * smoothstep(0, 1, len * strength);
                return center + dist;
            }

            fixed4 frag (v2f i) : SV_TARGET
            {
                float2 scaleCenter = float2(0.5, 0.5);
                float scale = _ScaleFactor;

                // 调整UV以实现缩放
                float2 adjustedUV = (i.uv - scaleCenter) * scale + scaleCenter;

                // 执行镜头失真
                float2 distortedUV = Distort(adjustedUV, _DistortionStrength);

                fixed4 col = tex2D(_MainTex, distortedUV);
                return col;
            }

            ENDCG
        }
    }
}
