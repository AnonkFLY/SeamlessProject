Shader "AnonkShader/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BurnAmount("BurnAmount",Range(0,1)) = 0
        _LineWidth("LineWidth",Range(0,0.1)) = 0
        _OffsetValue("OffsetValue",Range(-0.5,0.5)) = 0
        _DissolveColor("DissolveColor",Color)=(0,0,0,0)
        _BaseColor("BaseColor",Color) = (0,0,0,0)
        _DiffuseColor("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha 
            //Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 pos:NORMAL;
                float3 normal:NORMAL1;
                fixed3 worldNormal:Color0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BurnAmount;
            float _LineWidth;
            float _OffsetValue;
            float _SmooValue;
            float4 _DissolveColor;
            float4 _BaseColor;
            float3 _DiffuseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.pos = v.vertex;
                o.normal = v.vertex+(v.normal*_BurnAmount);
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld,v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float offsetValue = col.r-_BurnAmount+_OffsetValue;
                clip(offsetValue);

                float edgeFactor = saturate(offsetValue / _LineWidth)*step(offsetValue-_LineWidth,0);
                fixed3 color=lerp(_BaseColor,_DissolveColor,edgeFactor);

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                fixed3 worldLightDir = _WorldSpaceLightPos0.xyz;
                fixed3 diffuse = saturate(dot(worldLightDir,i.worldNormal));
                col.xyz = ambient+diffuse*_LightColor0.rgb*_DiffuseColor;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(col);
            }
            ENDCG
        }
    }
}
