Shader "Custom/Ukiyoe3D_URP_Working"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,0.85,0.6,1)
        _PatternTex ("Pattern Texture (optional)", 2D) = "white" {}
        _UsePattern ("Use Pattern", Float) = 0
        _PatternTiling ("Pattern Tiling", Vector) = (1,1,0,0)

        _NoiseTex ("Noise Texture (screen-space)", 2D) = "white" {}
        _NoiseScale ("Noise Screen Scale", Float) = 0.02
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.1

        _StrokeTex("Stroke Texture (alpha)", 2D) = "white" {}
        _StrokeColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
        _StrokeTexScale("Stroke Screen Scale", Float) = 4.0

        _UseBokashi ("Use Bokashi Gradation", Float) = 0
        _BokashiStrength ("Bokashi Strength", Range(0,1)) = 0.4
        _BokashiMode("Bokashi Mode (1=Screen,2=Sphere)", Float) = 1
        _LightPosWorld("Light Position (world)", Vector) = (0,1,0,0)
        _CenterWorld("Object Center (world)", Vector) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        // ---- Outline pass ----
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_StrokeTex); SAMPLER(sampler_StrokeTex);
            float4 _StrokeColor;
            float _OutlineWidth;
            float _StrokeTexScale;

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float4 screenPos : TEXCOORD0; };

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 n = normalize(v.normalOS);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz + n * _OutlineWidth);
                o.screenPos = o.positionCS;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = frac(((i.screenPos.xy / i.screenPos.w) * 0.5 + 0.5) * _StrokeTexScale);
                half a = SAMPLE_TEXTURE2D(_StrokeTex, sampler_StrokeTex, uv).a;
                return half4(_StrokeColor.rgb, a);
            }
            ENDHLSL
        }

        // ---- Main pass ----
        Pass
        {
            Name "Main"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_PatternTex); SAMPLER(sampler_PatternTex);
            TEXTURE2D(_NoiseTex);   SAMPLER(sampler_NoiseTex);

            float4 _Color;
            float _UsePattern;
            float4 _PatternTiling;
            float _NoiseScale;
            float _NoiseIntensity;
            float _UseBokashi;
            float _BokashiStrength;
            float _BokashiMode;
            float4 _LightPosWorld;
            float4 _CenterWorld;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float3 worldNormal: TEXCOORD1;
                float2 uv         : TEXCOORD2;
                float4 screenPos  : TEXCOORD3;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float4 wp = mul(unity_ObjectToWorld, v.positionOS);
                o.worldPos = wp.xyz;
                o.worldNormal = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                o.screenPos = o.positionCS;
                return o;
            }

            float2 ClipToScreen01(float4 clipPos)
            {
                return (clipPos.xy / clipPos.w) * 0.5 + 0.5;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // --- Base Color ---
                half3 col = _Color.rgb;

                if (_UsePattern > 0.5)
                {
                    float2 uv = i.uv * _PatternTiling.xy;
                    col *= SAMPLE_TEXTURE2D(_PatternTex, sampler_PatternTex, uv).rgb;
                }

                // --- Screen-space noise ---
                float4 cClip = mul(UNITY_MATRIX_VP, float4(_CenterWorld.xyz,1));
                float2 cScr = ClipToScreen01(cClip);
                float2 pScr = ClipToScreen01(i.screenPos);
                float2 nUV = frac((pScr - cScr) / max(1e-6, _NoiseScale));
                half noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, nUV).r;
                col = lerp(col * (1.0 - _NoiseIntensity * noise), col, 1.0);

                // --- Bokashi (light gradient) ---
                if (_UseBokashi > 0.5)
                {
                    float3 c = _CenterWorld.xyz;
                    float3 l = _LightPosWorld.xyz;
                    float3 dir = normalize(l - c);
                    float3 n = normalize(i.worldPos - c);
                    float grad = dot(n, dir);
                    col *= lerp(1.0 - _BokashiStrength, 1.0, saturate(grad * 0.5 + 0.5));
                }

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/Unlit"
}
