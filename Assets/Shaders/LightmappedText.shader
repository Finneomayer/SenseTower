Shader "Unlit/LightmappedText"
{
    Properties{
_Color("Main Color", Color) = (1,1,1,1)
_MainTex("Base (RGB)", 2D) = "white" {}
_BumpMap("Normalmap", 2D) = "bump" {}
_LightMap("Lightmap (RGB)", 2D) = "black" {}
_Alpha("Alpha",Range(0,1)) = 0.5
    }
        SubShader{
        LOD 300
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM

        #pragma surface surf Lambert nodynlightmap alpha
        struct Input {
        float2 uv_MainTex;
        float2 uv_BumpMap;
        float2 uv2_LightMap;
        };
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _LightMap;
        fixed _Alpha;
        fixed4 _Color;
        void surf(Input IN, inout SurfaceOutput o)
        {
        o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
        half4 lm = tex2D(_LightMap, IN.uv2_LightMap);
        o.Emission = lm.rgb * o.Albedo.rgb;
        o.Alpha = lm.a * _Color.a * _Alpha;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
}
FallBack "Legacy Shaders/Lightmapped/Diffuse"

}
