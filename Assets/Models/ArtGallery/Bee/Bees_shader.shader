Shader "Gallery/Bees" {
   Properties
   {
       _MainTex ("Texture", 2D) = "white" {}
       _PosTex("position texture", 2D) = "black"{}
   }
   SubShader
   {
       Tags { "RenderType"="Opaque" }
       LOD 100 Cull Off
 
       Pass
       {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
 
           #include "UnityCG.cginc"
           #pragma multi_compile_instancing
           #define ts _PosTex_TexelSize
 
           struct appdata_t
           {
               float4 vertex : POSITION;
               float4 texcoord : TEXCOORD0;
               float4 texcoord1 : TEXCOORD1;
           };
 
           sampler2D _MainTex, _PosTex;
           float4 _PosTex_TexelSize;
     
           float3x3 XRotationMatrix(float sina, float cosa)
           {
               return float3x3(
                   1,   0,       0,
                   0,   cosa,   -sina,
                   0,   sina,   cosa);
           }
     
           float3x3 YRotationMatrix(float sina, float cosa)
           {
               return float3x3(
                   cosa,    0,   -sina,
                   0,        1,   0,
                   sina,    0,   cosa);
           }
     
           float3x3 ZRotationMatrix(float sina, float cosa)
           {
               return float3x3(
                   cosa,    -sina,   0,
                   sina,    cosa,   0,
                   0,        0,       1);
           }
     
           appdata_t vert (appdata_t v)
           {
               float x = (v.texcoord.z + 0.5) * ts.x;//VertexID & Size of bake anim texture
               float y;
 
               y = fmod(_Time.y, 1.0) * 4;//Loop animation
 
               float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
               pos += v.vertex;
         
               //float rotAngle = v.texcoord1.x * 1000; //ROTATION
               float rotAngle = 0;
               //Convert to radian
               rotAngle *= 0.01745329251994329576923690768489;//rotAngle * UNITY_PI / 180.0
               //Calculate sin cos
               float sina, cosa;
                sincos(rotAngle, sina, cosa);
         
               float randRot = (v.texcoord.w + v.texcoord1.x);
               float3 pivot = float3(v.texcoord.w, v.texcoord1.x, v.texcoord1.y);
         
               //Move to root
               pos.xyz -= pivot;
         
               //Rotate
               pos.xyz = mul(YRotationMatrix(sina, cosa), pos.xyz);
         
               if(randRot > 1)
                   pos.xyz = mul(XRotationMatrix(sina, cosa), pos.xyz);
               else
                   pos.xyz = mul(ZRotationMatrix(sina, cosa), pos.xyz);
         
               //Move it back
               pos.xyz += pivot;
 
               appdata_t o;
               o.vertex = UnityObjectToClipPos(pos);
               o.texcoord = v.texcoord;
               return o;
           }
     
           half4 frag (appdata_t i) : SV_Target
           {
               return tex2D(_MainTex, i.texcoord.xy);
           }
           ENDCG
       }
   }
}