Shader "Unlit/SrarrySky"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}

         _starsFrequency("Stars count", Range(0.1, 20)) = 8.0
         _starsExposure("Stars exposure", Float) = 200
         _directionPow("Direction pow", Float) = 100

         _flickerMin("time min flickering", Range(0.1, 5)) = 0.4
         _flickerMax("time min flickering", Range(0.1, 5)) = 0.4
         _flickerTimeCoef("time coef", Range(0.01, 20)) = 1
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
            float4 _MainTex_ST;
            float _starsFrequency;
            float _starsExposure;
            float _flickerMin;
            float _flickerMax;
            float _flickerTimeCoef;
            float _directionPow;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 hash(float3 p) // без комментариев, выведено опытным путем
            {                     // если ктото, хочет подобрать лучше - велком.
                p = float3(
                    dot(p, float3(127.1, 311.7, 74.7)),
                    dot(p, float3(269.5, 183.3, 246.1)),
                    dot(p, float3(113.5, 271.9, 124.6)));

                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }
            float noise(float3 p) // я встречал десятки реализаций. гдето какие то лучше подходят.
            {                      // под разные задачи свое. здесь, мне кажется, это лучший шум будет
                float3 i = floor(p);
                float3 f = frac(p);

                float3 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(lerp(dot(hash(i + float3(0.0, 0.0, 0.0)), f - float3(0.0, 0.0, 0.0)),
                    dot(hash(i + float3(1.0, 0.0, 0.0)), f - float3(1.0, 0.0, 0.0)), u.x),
                    lerp(dot(hash(i + float3(0.0, 1.0, 0.0)), f - float3(0.0, 1.0, 0.0)),
                        dot(hash(i + float3(1.0, 1.0, 0.0)), f - float3(1.0, 1.0, 0.0)), u.x), u.y),
                    lerp(lerp(dot(hash(i + float3(0.0, 0.0, 1.0)), f - float3(0.0, 0.0, 1.0)),
                        dot(hash(i + float3(1.0, 0.0, 1.0)), f - float3(1.0, 0.0, 1.0)), u.x),
                        lerp(dot(hash(i + float3(0.0, 1.0, 1.0)), f - float3(0.0, 1.0, 1.0)),
                            dot(hash(i + float3(1.0, 1.0, 1.0)), f - float3(1.0, 1.0, 1.0)), u.x), u.y), u.z);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st = i.uv;
                float time = _Time.y;
                float3 stars_direction = normalize(float3(st * 2.0f - 1.0f, 1.0f)); // направление, лучше на себя
                float stars = pow(clamp(noise(stars_direction * _directionPow * 2), 0.0f, 1.0f), _starsFrequency) * _starsExposure; // сила звезд
                stars *= lerp(_flickerMin, _flickerMax, noise(stars_direction * _directionPow + float3(time, time, time)* _flickerTimeCoef)); // время мерцания

                return float4(float3(stars, stars, stars), 1.0);;
            }
            ENDCG
        }
    }
}
