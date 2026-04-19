Shader "Custom/Foliage"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ClipMaskTex ("Clip Mask (B)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        sampler2D _ClipMaskTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v)
        {
            float4 world =mul(unity_ObjectToWorld, v.vertex);

            float periodoffset = world.x + world.z;
            float4 localpos = v.vertex;
            localpos.x +=  sin(periodoffset + _Time.y) * localpos.y * 0.25;
            localpos.z +=  cos(periodoffset + _Time.y * 2.0) * localpos.y * 0.2;

            v.vertex = localpos;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // -- discard any pixels above a certain height
            // float3 localpos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
