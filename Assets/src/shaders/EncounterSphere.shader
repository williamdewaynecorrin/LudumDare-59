Shader "Custom/EncounterSphere"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ClipMaskTex ("Clip Mask (B)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _ClipValue ("ClipValue", Range(0, 1)) = 0.75
        _TexScroll ("Tex Scroll", Vector) = (0, 0, 0, 0)
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
        half _ClipValue;
        half _Metallic;
        half3 _Rotation;
        fixed4 _Color;
        fixed2 _TexScroll;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v)
        {
            float4 world =mul(unity_ObjectToWorld, v.vertex);
            float4 localpos = v.vertex;
            float mag = frac((world.x * world.z) + world.y);
            float mag2 = frac((world.y * world.z) + world.x);

            float period = 4;
            float magmult = 0.006;
            localpos.x += sin(_Time.y * period) * mag * magmult;
            localpos.y += cos(_Time.y * period) * mag * magmult;
            localpos.z += sin(_Time.y * period * 0.5) * mag2 * magmult;

            v.vertex = localpos;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // -- discard any pixels above a certain height
            // float3 localpos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
            fixed clipmask = tex2D(_ClipMaskTex, IN.uv_MainTex).b;
            if(clipmask < _ClipValue)
                discard;

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + _TexScroll * _Time.y) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
