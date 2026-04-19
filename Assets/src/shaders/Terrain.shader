Shader "Custom/Terrain"
{
    Properties
    {
        _ColorGrass ("ColorGrass", Color) = (1,1,1,1)
        _GrassTex ("Grass Texture (RGB)", 2D) = "white" {}

        _ColorCliff ("ColorCliff", Color) = (1,1,1,1)
        _CliffTex ("Cliff Texture (RGB)", 2D) = "white" {}
        _CliffNormalThresh("Cliff Normal", Range(-1, 1)) = 0.5

        _ColorSnow ("ColorSnow", Color) = (1,1,1,1)
        _SnowTex ("Snow Texture (RGB)", 2D) = "white" {}
        _SnowHeight("Snow Height", Range(-10, 100)) = 10

        _GroundCliffBlend("Ground <-> Cliff Blend", Range(0, 6)) = 1
        _GroundGroundBlend("Ground <-> Ground Blend", Range(0, 6)) = 1

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "TerrainCompatible" = "True" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_GrassTex;
            float3 worldPos;
            float3 worldNormal;
        };

        fixed4 _ColorGrass;
        fixed4 _ColorCliff;
        fixed4 _ColorSnow;

        sampler2D _GrassTex;
        sampler2D _CliffTex;
        sampler2D _SnowTex;

        half _Glossiness;
        half _SnowHeight;
        float _CliffNormalThresh;
        half _ClipValue;
        half _Metallic;
        
        half _GroundCliffBlend;
        half _GroundGroundBlend;
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        float invlerp(float a, float b, float v)
        {
            return (v - a) / (b - a);
        }

        void vert (inout appdata_full v)
        {
            // float4 world =mul(unity_ObjectToWorld, v.vertex);
            float4 localpos = v.vertex;
            localpos.x = sin(_Time.y) * localpos.x;

            v.vertex = localpos;
        }

        fixed4 smoothblend(fixed4 cola, fixed4 colb, float smoothparam, float range, float middle)
        {
            float blendval = saturate(invlerp(middle - range, middle + range, smoothparam));
            return lerp(cola, colb, blendval);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 col = fixed4(1,1,1,1);
            fixed4 colgrass = tex2D (_GrassTex, IN.uv_GrassTex) * _ColorGrass;
            fixed4 colsnow = tex2D (_SnowTex, IN.uv_GrassTex) * _ColorSnow;
            fixed4 colcliff = tex2D (_CliffTex, IN.uv_GrassTex) * _ColorCliff;

            // -- calculate blended ground color
            fixed4 colground = smoothblend(colgrass, colsnow, IN.worldPos.y, _GroundGroundBlend, _SnowHeight);
            col = smoothblend(colcliff, colground, IN.worldNormal.y, _GroundCliffBlend, _CliffNormalThresh);

            o.Albedo = col;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
