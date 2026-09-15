Shader "Miscalculation/CharacterLobby/SceneItem" {
    Properties {
        [PerRendererData] _MainTex ("Texture",2D)="white" {}
        _Color ("Tint",Color)=(1,1,1,1)
        _LightField ("Packed Light Field",2D)="gray" {}
        _Lamp ("Lamp",Range(0,1))=1
        _LampStrength ("Lamp Strength",Range(0,2))=1
        _FarBrightness ("Lamp Far Brightness",Range(.1,1.2))=.48
        _OffBrightness ("Off Ambient",Range(.05,.8))=.32
        _MoonStrength ("Moon Strength",Range(0,1.5))=.46
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass { CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata {float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
            struct v2f {float4 vertex:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;float2 local:TEXCOORD1;float4 screen:TEXCOORD2;};
            sampler2D _MainTex,_LightField;float4 _MainTex_ST,_Color,_ClipRect,_DesignRect;
            float _Lamp,_LampStrength,_FarBrightness,_OffBrightness,_MoonStrength;
            v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.screen=ComputeScreenPos(o.vertex);o.uv=TRANSFORM_TEX(v.uv,_MainTex);o.local=v.vertex.xy;o.color=v.color*_Color;return o;}
            fixed4 frag(v2f i):SV_Target {
                fixed4 source=tex2D(_MainTex,i.uv)*i.color;
                float2 screenUv=i.screen.xy/max(.0001,i.screen.w);
                float2 designUv=(screenUv-_DesignRect.xy)/max(float2(.0001,.0001),_DesignRect.zw);
                float2 packed=tex2D(_LightField,saturate(designUv)).rg;
                float lampGain=lerp(_FarBrightness,1.08,saturate(packed.r*_LampStrength));
                float3 warm=lerp(float3(1,1,1),float3(1.075,1.01,.86),saturate(packed.r*_LampStrength)*.58);
                float offGain=saturate(_OffBrightness+packed.g*_MoonStrength);
                float3 cool=lerp(float3(.79,.84,.96),float3(.72,.84,1.08),saturate(packed.g)*.55);
                float3 lit=lerp(source.rgb*offGain*cool,source.rgb*lampGain*warm,saturate(_Lamp));
                #ifdef UNITY_UI_CLIP_RECT
                    source.a*=UnityGet2DClipping(i.local,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                    clip(source.a-.001);
                #endif
                return float4(lit,source.a);
            }
        ENDCG }
    }
}
