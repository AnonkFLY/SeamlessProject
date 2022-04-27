Shader "AnonkShader/Diffuse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DiffuseColor("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { 
            
        "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                fixed3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed3 worldNormal:Color0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _DiffuseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld,v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                fixed3 worldLightDir = _WorldSpaceLightPos0.xyz;
                fixed3 diffuse = saturate(dot(worldLightDir,i.worldNormal));
                col.xyz = ambient+diffuse*_LightColor0.rgb*_DiffuseColor;
                return col;
            }
            ENDCG
        }
    }
}
