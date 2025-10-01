Shader "Custom/WindShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Float) = 0.2
        _WindSpeed ("Wind Speed", Float) = 1
        _WindFrequency ("Wind Frequency", Float) = 2
        _WindTopOnly ("Wind Top Only", Float) = 1 // 1 = Only top, 0 = Entire sprite
        _Color ("Tint Color", Color) = (1,1,1,1) // Color and Alpha
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float _WindStrength;
            float _WindSpeed;
            float _WindFrequency;
            float _WindTopOnly;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                float localHeight = v.vertex.y;
                float spriteHeight = 1.0;
                float windHeight = 0.5;

                float influence = _WindTopOnly > 0.5 ? 
                    saturate((localHeight - (spriteHeight - windHeight)) / windHeight) : 1.0;

                float windOffset = sin(_Time.y * _WindSpeed + v.vertex.x * _WindFrequency) * _WindStrength;
                v.vertex.x += windOffset * influence;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                if (texColor.a < 0.01) discard;

                return texColor * _Color;
            }
            ENDCG
        }
    }
}
