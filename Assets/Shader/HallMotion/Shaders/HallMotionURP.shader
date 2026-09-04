Shader "Miscalculation/UI/Hall Motion URP"
{
    Properties
    {
        [PerRendererData] _MainTex ("Static Black Base", 2D) = "black" {}
        [NoScaleOffset] _ArtTex ("Transparent Hall Artwork", 2D) = "black" {}
        _MasterIntensity ("Master Intensity", Range(0, 1.35)) = 0.82
        _MotionSpeed ("Motion Speed", Range(0.05, 1.4)) = 0.38
        _SwirlStrength ("Swirl Strength", Range(0, 3.5)) = 0.64
        _SwirlRadius ("Swirl Radius", Range(0.12, 0.85)) = 0.31
        _CoreBreathStrength ("Core Breath Strength", Range(0, 1.5)) = 0.54
        _CoreMotionPixels ("Core Motion Pixels", Range(0, 12)) = 3.2
        _InkWarpPixels ("Ink Warp Pixels", Range(0, 3)) = 0.75
        _LeftUiProtectWidth ("Left UI Protect Width", Range(0.18, 0.48)) = 0.30
        _EnergyStrength ("Energy Strength", Range(0, 1.5)) = 0.67
        _ParallaxPixels ("Parallax Pixels", Range(0, 10)) = 3.2
        _GrainStrength ("Grain Strength", Range(0, 1.5)) = 0.52
        _PrintDriftPixels ("Print Drift Pixels", Range(0, 4)) = 0.9
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "HallMotion"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ArtTex);
            SAMPLER(sampler_ArtTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float4 _Resolution;
                float2 _Pointer;
                float2 _SwirlCenter;
                float _MotionTime;
                float _MasterIntensity;
                float _MotionSpeed;
                float _SwirlStrength;
                float _SwirlRadius;
                float _CoreBreathStrength;
                float _CoreMotionPixels;
                float _InkWarpPixels;
                float _LeftUiProtectWidth;
                float _EnergyStrength;
                float _ParallaxPixels;
                float _GrainStrength;
                float _PrintDriftPixels;
                float _Anomaly;
                float _DebugMask;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                const float aspect = 16.0 / 9.0;

                // Only the far-left logo/menu strip is protected. Face, cards and
                // all other artwork intentionally remain available to the motion layers.
                const float protectFeather = 0.07;
                float protect = 1.0 - smoothstep(
                    max(0.0, _LeftUiProtectWidth - protectFeather),
                    _LeftUiProtectWidth,
                    uv.x);
                float movable = 1.0 - protect;

                float2 safeResolution = max(_Resolution.xy, float2(1.0, 1.0));
                half3 baseLayer = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;

                // Keep the black plate static and deform only the transparent
                // artwork. This avoids revealing a stale copy underneath motion.
                float2 mouseOffset = _Pointer * (_ParallaxPixels / safeResolution) * _MasterIntensity;
                float2 artUv = uv + mouseOffset * movable;

                // Rotation has almost no displacement at its mathematical center.
                // A continuous radial falloff keeps the core readable without
                // an authored mask boundary or tile seam.
                float2 coreDelta = artUv - _SwirlCenter;
                coreDelta.x *= aspect;
                float coreRadius = length(coreDelta);
                float coreGuide = 1.0 - smoothstep(_SwirlRadius * 0.08, _SwirlRadius * 0.56, coreRadius);
                float2 coreOrbit = float2(
                    sin(_MotionTime * _MotionSpeed * 0.73 + 0.35),
                    cos(_MotionTime * _MotionSpeed * 0.61 + 1.10))
                    * (_CoreMotionPixels / safeResolution) * _MasterIntensity
                    * coreGuide * movable;
                artUv += coreOrbit;
                float2 inkOffset = float2(
                    sin(artUv.y * 18.0 + artUv.x * 7.0 + _MotionTime * _MotionSpeed * 0.64),
                    cos(artUv.x * 15.0 - artUv.y * 5.0 - _MotionTime * _MotionSpeed * 0.51))
                    * (_InkWarpPixels / safeResolution) * _MasterIntensity * movable;
                artUv += inkOffset;

                float2 p = artUv - _SwirlCenter;
                p.x *= aspect;
                float radius = length(p);
                float angle = atan2(p.y, p.x);
                float influence = 1.0 - smoothstep(_SwirlRadius * 0.46, _SwirlRadius, radius);
                float coreBreath = sin(_MotionTime * _MotionSpeed * 0.82 + radius * 8.0)
                                 * _CoreBreathStrength * _MasterIntensity * influence * movable;
                p *= 1.0 + coreBreath * 0.006;
                float wave = sin(radius * 47.0 - _MotionTime * _MotionSpeed * 1.8 + angle * 3.0)
                           + 0.45 * sin(radius * 91.0 + _MotionTime * _MotionSpeed * 1.1 - angle * 5.0);
                float theta = (0.0075 + 0.006 * wave) * _SwirlStrength * _MasterIntensity * influence * movable;
                theta += _Anomaly * influence * movable * (0.035 + 0.012 * sin(angle * 8.0));
                float cs = cos(theta);
                float sn = sin(theta);
                p = mul(float2x2(cs, -sn, sn, cs), p);
                p.x /= aspect;
                float2 warpedUv = clamp(_SwirlCenter + p, 0.001, 0.999);

                half4 art = SAMPLE_TEXTURE2D(_ArtTex, sampler_ArtTex, warpedUv);
                float cyanMask = smoothstep(0.08, 0.48, min(art.g, art.b) - art.r * 0.55) * art.a;
                float magentaMask = smoothstep(0.07, 0.44, min(art.r, art.b) - art.g * 0.48) * art.a;
                float chromaMask = saturate(cyanMask + magentaMask);

                float driftPx = _PrintDriftPixels * _MasterIntensity * (0.55 + 0.45 * sin(_MotionTime * 0.72));
                driftPx += _Anomaly * 8.0;
                float2 drift = float2(driftPx / safeResolution.x, 0.35 * driftPx / safeResolution.y) * movable;
                half4 shiftedA = SAMPLE_TEXTURE2D(_ArtTex, sampler_ArtTex, clamp(warpedUv + drift, 0.001, 0.999));
                half4 shiftedB = SAMPLE_TEXTURE2D(_ArtTex, sampler_ArtTex, clamp(warpedUv - drift, 0.001, 0.999));
                half3 artPremul = art.rgb * art.a;
                half3 registeredPremul = half3(
                    shiftedA.r * shiftedA.a,
                    shiftedB.g * shiftedB.a,
                    shiftedB.b * shiftedB.a);
                float registeredAlpha = max(shiftedA.a, shiftedB.a);
                float registerMix = chromaMask * 0.48;
                artPremul = lerp(artPremul, registeredPremul, registerMix);
                float artAlpha = lerp(art.a, registeredAlpha, registerMix);

                float flow = 0.5 + 0.5 * sin(angle * 4.5 + radius * 37.0 - _MotionTime * _MotionSpeed * 2.65);
                flow *= 0.62 + 0.38 * sin(angle * 9.0 - radius * 19.0 + _MotionTime * _MotionSpeed * 1.35);
                flow = smoothstep(0.18, 0.92, flow);
                half3 energyTint = cyanMask * half3(0.07, 0.94, 1.0) + magentaMask * half3(0.86, 0.05, 1.0);
                artPremul += energyTint * flow * _EnergyStrength * _MasterIntensity * influence * movable * 0.18;

                float sweepPosition = lerp(-0.18, 1.18, smoothstep(0.0, 1.0, _Anomaly));
                float sweep = exp(-pow((artUv.y - sweepPosition) / 0.085, 2.0));
                artPremul *= 1.0 - sweep * _Anomaly * 0.32 * movable;
                artPremul += energyTint * _Anomaly * chromaMask * 0.11;

                half3 baseColor = baseLayer * (1.0 - artAlpha) + artPremul;

                float noise = Hash21(input.positionCS.xy + floor(_MotionTime * 12.0));
                float paperNoise = (noise - 0.5) * _GrainStrength * _MasterIntensity * 0.055;
                baseColor += paperNoise * half3(0.82, 0.72, 0.92);

                if (_DebugMask > 0.5)
                {
                    half3 maskColor = lerp(half3(0.05, 0.45, 0.50), half3(0.72, 0.05, 0.20), protect);
                    baseColor = lerp(baseColor, maskColor, 0.47);
                }

                return half4(saturate(baseColor), input.color.a);
            }
            ENDHLSL
        }
    }
}
