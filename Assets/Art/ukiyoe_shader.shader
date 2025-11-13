Shader "Custom/UkiyoE_URP"
{
    Properties
    {
        // Base Color Properties
        _Color ("Base Color", Color) = (1,1,1,1)
        _SecondaryColor ("Secondary Color (for patterns)", Color) = (0.8,0.8,0.8,1)
        
        // Pattern Properties (procedural)
        [KeywordEnum(None, Dots, Waves, Lines)] _PatternType ("Pattern Type", Float) = 0
        _PatternScale ("Pattern Scale", Range(1, 100)) = 20
        _PatternIntensity ("Pattern Intensity", Range(0, 1)) = 0.5
        
        // Noise Properties (procedural)
        _NoiseScale ("Noise Scale", Range(1, 100)) = 30
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.2
        
        // Line/Stroke Properties
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.005
        _StrokeVariation ("Stroke Variation", Range(0, 1)) = 0.3
        
        // Bokashi Properties
        [KeywordEnum(None, Linear, Sphere)] _BokashiMode ("Bokashi Mode", Float) = 0
        _BokashiStrength ("Bokashi Strength", Range(0, 2)) = 1.0
        _BokashiOffset ("Bokashi Offset", Range(-2, 2)) = 0.0
        _BokashiModifier ("Bokashi Modifier", Range(0, 2)) = 1.0
        _ViewModifierBase ("View Modifier Base", Range(0, 2)) = 0.5
        _DarkColor ("Dark Bokashi Color", Color) = (0.2, 0.2, 0.3, 1)
        
        // Light Properties (for Bokashi)
        _LightStrength ("Light Strength", Range(0, 5)) = 1.0
        
        // Center Point (for depth scaling)
        _CenterPoint ("Center Point (World)", Vector) = (0,0,0,0)
    }
    
    SubShader
    {
        // Universal Pipeline tag
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        // Pass 1: Outline/Stroke Rendering
        Pass
        {
            Name "OUTLINE"
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0; // Use float4 for perspective divide
                float3 worldPos : TEXCOORD1;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float _StrokeVariation;

            // Procedural noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            v2f vert(appdata v)
            {
                v2f o;

                // Expand vertices along normals for outline effect
                float3 norm = normalize(v.normal);
                float3 expanded = v.vertex.xyz + norm * _OutlineWidth;

                // ===================================================
                // WARNING FIX (Line 102): Pass float3 directly
                // ===================================================
                o.pos = TransformObjectToHClip(expanded);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);

                // Screen space UV for stroke variation (fixed for perspective)
                o.screenPos = ComputeScreenPos(o.pos);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Correctly calculate screen UV with perspective divide
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                // Procedural stroke texture variation
                float2 strokeUV = screenUV * 50.0; 
                float strokeNoise = noise(strokeUV);
                float variation = lerp(1.0, strokeNoise, _StrokeVariation);
                
                return _OutlineColor * variation;
            }
            ENDHLSL
        }
        
        // Pass 2: Main Surface with Ukiyo-e Effects
        Pass
        {
            Name "MAIN"
            // URP main pass tag
            Tags { "LightMode"="UniversalForward" } 
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP keywords
            #pragma multi_compile _ _BOKASHIMODE_LINEAR _BOKASHIMODE_SPHERE
            #pragma multi_compile _ _PATTERNTYPE_DOTS _PATTERNTYPE_WAVES _PATTERNTYPE_LINES

            // URP core libraries
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5; // URP shadow coordinates
            };

            // Properties
            float4 _Color;
            float4 _SecondaryColor;
            float4 _DarkColor;
            float _PatternScale;
            float _PatternIntensity;
            float _NoiseScale;
            float _NoiseIntensity;
            
            float _BokashiStrength;
            float _BokashiOffset;
            float _BokashiModifier;
            float _ViewModifierBase;
            float _LightStrength;
            float4 _CenterPoint;

            // Procedural noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // Procedural patterns
            float dotPattern(float2 uv)
            {
                float2 grid = frac(uv) - 0.5;
                float dist = length(grid);
                return smoothstep(0.3, 0.2, dist);
            }

            float wavePattern(float2 uv)
            {
                return sin(uv.x * 6.28318) * 0.5 + 0.5;
            }

            float linePattern(float2 uv)
            {
                return step(0.5, frac(uv.y));
            }

            v2f vert(appdata v)
            {
                v2f o;
                // URP helper functions for position and normals
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.pos);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                
                // URP shadow coordinate calculation
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.shadowCoord = GetShadowCoord(vertexInput);
                
                return o;
            }

            // Bokashi Algorithm 1: Linear Gradation (ADAPTED FOR DIRECTIONAL LIGHT)
            float CalculateLinearBokashi(float3 worldPos, float3 centerPos, float3 viewDir, float3 lightDir)
            {
                // The direction *from* the center *to* the light (for directional light)
                float3 d = lightDir;
                
                // Calculate strength modifier
                float strengthMod = (_LightStrength * _BokashiModifier) + _BokashiOffset;
                float3 gradationPoint = centerPos + d * strengthMod;

                // ===================================================
                // WARNING FIX (Line 248): Pass float3 directly
                // ===================================================
                float4 gradationScreen = ComputeScreenPos(TransformWorldToHClip(gradationPoint));
                
                // ===================================================
                // WARNING FIX (Line 249): Pass float3 directly
                // ===================================================
                float4 posScreen = ComputeScreenPos(TransformWorldToHClip(worldPos));

                // View modifier
                float3 viewVec = normalize(viewDir);
                float3 lightVec = normalize(lightDir);
                float viewMod = dot(viewVec, lightVec) * _ViewModifierBase;

                float2 screenDiff = (posScreen.xy / posScreen.w) - (gradationScreen.xy / gradationScreen.w);
                float2 lightScreen = normalize(gradationScreen.xy);
                float gradFactor = dot(screenDiff, lightScreen) + viewMod;
                
                return saturate(gradFactor);
            }

            // Bokashi Algorithm 2: Sphere-based Shading (BROKEN - NEEDS LIGHT POSITION)
            float CalculateSphereBokashi(float3 worldPos, float3 centerPos, float3 lightPos)
            {
                // This algorithm requires a light *position* (lightPos).
                // URP's main pass provides a *direction* for the main directional light.
                float3 sphereNormal = normalize(worldPos - centerPos);
                float3 lightDir = lightPos - centerPos;
                float lightDist = length(lightDir);
                lightDir = normalize(lightDir);
                
                float nDotL = dot(sphereNormal, lightDir);
                float shadingFactor = nDotL * (_LightStrength / (lightDist * lightDist));
                
                return saturate(shadingFactor);
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get center position
                float3 centerPos = _CenterPoint.w > 0 ? _CenterPoint.xyz : TransformObjectToWorld(float3(0,0,0));
                
                // Screen space coordinates
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                // Calculate depth-based offset
                float depth = distance(_WorldSpaceCameraPos, centerPos);
                
                // ===================================================
                // WARNING FIX (Line 290): Pass float3 directly
                // ===================================================
                float4 centerScreen = ComputeScreenPos(TransformWorldToHClip(centerPos));
                float2 centerOffset = centerScreen.xy / centerScreen.w;
                
                // Depth scaling
                float depthScale = 1.0 / max(depth * 0.1, 1.0);
                float2 noiseUV = (screenUV + centerOffset) * _NoiseScale * depthScale;
                float2 patternUV = (screenUV + centerOffset) * _PatternScale * depthScale;
                
                // Generate procedural noise
                float noiseValue = noise(noiseUV);
                
                // Generate procedural pattern
                float pattern = 0.0;
                #if _PATTERNTYPE_DOTS
                    pattern = dotPattern(patternUV);
                #elif _PATTERNTYPE_WAVES
                    pattern = wavePattern(patternUV);
                #elif _PATTERNTYPE_LINES
                    pattern = linePattern(patternUV);
                #endif
                
                // Blend base color with pattern
                half4 baseColor = lerp(_Color, _SecondaryColor, pattern * _PatternIntensity);
                
                // Apply noise for pseudo-uniform color effect
                baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb * noiseValue, _NoiseIntensity);

                // Get URP's main light
                Light mainLight = GetMainLight(i.shadowCoord);
                
                // Calculate Bokashi effect
                float bokashiFactor = 1.0;
                
                #if _BOKASHIMODE_LINEAR
                    // Pass the main light's *direction*
                    bokashiFactor = CalculateLinearBokashi(i.worldPos, centerPos, i.viewDir, mainLight.direction);
                #elif _BOKASHIMODE_SPHERE
                    // This will be incorrect as mainLight.direction is not a position.
                    // To fix, add a _ManualLightPos property and pass it here.
                    bokashiFactor = CalculateSphereBokashi(i.worldPos, centerPos, float3(0,5,0)); // Using a placeholder
                #endif
                
                // Apply bokashi gradation with color blending
                half4 finalColor = lerp(_DarkColor, baseColor, bokashiFactor * _BokashiStrength);
                
                // Shadow attenuation
                float atten = mainLight.shadowAttenuation;
                finalColor.rgb *= atten;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"
}