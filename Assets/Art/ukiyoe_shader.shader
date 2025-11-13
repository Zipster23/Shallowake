Shader "Custom/Ukiyoe3D_Lit_Fixed"
{
    Properties
    {
        // Base
        _BaseColor("Base Color", Color) = (1,1,1,1)

        // Outline
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0,0.2)) = 0.02

        // Noise
        _UseNoise("Use Noise", Float) = 0
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseScale("Noise Scale", Range(0.001,0.1)) = 0.02
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.25
        _NoiseTint("Noise Tint Color", Color) = (1,1,1,1)

        // Bokashi
        _UseBokashi("Use Bokashi Gradient", Float) = 0
        _BokashiStrength("Bokashi Strength", Range(0,1)) = 0.5
        _BokashiLightColor("Bokashi Light Color", Color) = (1,1,1,1)
        _BokashiShadowColor("Bokashi Shadow Color", Color) = (0,0,0,1)

        _CenterWorld("Object Center", Vector) = (0,0,0,1)
        _LightPosWorld("Light Position (override)", Vector) = (0,1,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        // Outline pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct Attributes { float3 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 pos = v.positionOS + normalize(v.normalOS) * _OutlineWidth;
                o.positionCS = TransformObjectToHClip(pos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // Main lit pass (fixed)
        Pass
        {
            Name "Main"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Textures
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            // Properties
            float4 _BaseColor;
            float _UseNoise;
            float _NoiseScale;
            float _NoiseStrength;
            float4 _NoiseTint;

            float _UseBokashi;
            float _BokashiStrength;
            float4 _BokashiLightColor;
            float4 _BokashiShadowColor;
            float4 _CenterWorld;
            float4 _LightPosWorld;

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
                float3 normalWS   : TEXCOORD1;
                float2 uv         : TEXCOORD2;
                float4 screenPos  : TEXCOORD3;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                // Clip space position
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                // World pos & normal (simple reliable transforms)
                float4 worldPos4 = mul(unity_ObjectToWorld, v.positionOS);
                o.worldPos = worldPos4.xyz;
                o.normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.uv = v.uv;
                o.screenPos = o.positionCS;
                return o;
            }

            float2 ScreenUV(float4 clipPos)
            {
                return (clipPos.xy / clipPos.w) * 0.5 + 0.5;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Prepare standard lighting contributions
                float3 N = normalize(i.normalWS);

                // Main directional light (URP helper)
                Light mainLight = GetMainLight();
                float3 mainDir = normalize(mainLight.direction); // points from surface toward light (negative of light vector)
                float3 mainColor = mainLight.color;

                // Lambert diffuse for main light
                float NdotL = max(dot(N, -mainDir), 0.0);
                float3 diffuse = mainColor * NdotL;

                // Additional lights (if present)
                #ifdef _ADDITIONAL_LIGHTS
                uint addCount = GetAdditionalLightsCount();
                for (uint li = 0u; li < addCount; ++li)
                {
                    Light addL = GetAdditionalLight(li, i.worldPos);
                    float3 addDir = normalize(addL.direction);
                    float ndl2 = max(dot(N, -addDir), 0.0);
                    diffuse += addL.color * ndl2;
                }
                #endif

                // Ambient via SH sampling
                float3 ambient = SampleSH(N);

                // Compose lit color
                float3 litLighting = diffuse + ambient; // simple additive
                // Multiply base color by lighting (preserve base hue)
                float3 col = _BaseColor.rgb * litLighting;

                // If lighting is too dark for visualization, clamp a minimal ambient so color never goes fully black
                col = max(col, _BaseColor.rgb * 0.05);

                // Noise modulation (optional)
                if (_UseNoise > 0.5)
                {
                    float2 uv = frac(ScreenUV(i.screenPos) / max(1e-6, _NoiseScale));
                    half n = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv).r;
                    // tint = lerp(1, noise tint, strength)
                    float3 tint = lerp(float3(1.0,1.0,1.0), _NoiseTint.rgb, _NoiseStrength);
                    // mix color slightly
                    col = lerp(col, col * tint * n, _NoiseStrength * 0.6);
                }

                // Bokashi gradient overlay (acts like a stylized rim/gradation)
                if (_UseBokashi > 0.5)
                {
                    float3 center = _CenterWorld.xyz;
                    float3 lpos   = _LightPosWorld.xyz;
                    float3 dir = normalize(lpos - center);
                    float3 npos = normalize(i.worldPos - center);
                    float g = saturate(dot(npos, dir) * 0.5 + 0.5);
                    float3 bok = lerp(_BokashiShadowColor.rgb, _BokashiLightColor.rgb, g);
                    col = lerp(col, col * bok, _BokashiStrength);
                }

                return half4(saturate(col), 1.0);
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/Lit"
}
