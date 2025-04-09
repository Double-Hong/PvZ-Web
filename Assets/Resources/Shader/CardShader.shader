Shader "Custom/CardShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Saturation ("Saturation", Range(0, 2)) = 1.0  // 饱和度范围 0 到 2，1 为正常
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // 输入纹理和饱和度参数
            sampler2D _MainTex;
            float _Saturation;

            // 顶点着色器
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 像素着色器
            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                
                // 将颜色转换为灰度
                float gray = dot(col.rgb, half3(0.299, 0.587, 0.114));

                // 调整饱和度
                col.rgb = lerp(half3(gray, gray, gray), col.rgb, _Saturation);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
