Shader "Miscalculation/CharacterLobby/Background" {
    Properties {[PerRendererData] _MainTex ("Texture",2D)="white" {}
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15 _OffTex("Lamp off",2D)="black" {} _OnTex("Lamp on",2D)="black" {} _Lamp("Lamp",Range(0,1))=1}
    SubShader {Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] ColorMask [_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass { CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata {float4 vertex:POSITION;float4 color:COLOR;float4 uv:TEXCOORD0;};
            struct v2f {float4 vertex:SV_POSITION;float4 color:COLOR;float4 uv:TEXCOORD0;float2 local:TEXCOORD1;};
            float4 _ClipRect;
            v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.color=v.color;o.uv=v.uv;o.local=v.vertex.xy;return o;}
            float uiClip(v2f i) {
            #ifdef UNITY_UI_CLIP_RECT
                return UnityGet2DClipping(i.local,_ClipRect);
            #else
                return 1;
            #endif
            }
            sampler2D _OffTex,_OnTex;float _Lamp;
            float3 satColor(float3 c,float s){return lerp(dot(c,float3(.2126,.7152,.0722)),c,s);}
            float3 softLight(float3 b,float3 s){
                float3 d=lerp(((16*b-12)*b+4)*b,sqrt(max(0,b)),step(.25,b));
                return lerp(b-(1-2*s)*b*(1-b),b+(2*s-1)*(d-b),step(.5,s));
            }
            fixed4 frag(v2f i):SV_Target {
                float2 uv=i.uv.xy;float l=saturate(_Lamp);
                float3 off=satColor(tex2D(_OffTex,uv).rgb,lerp(.96,.92,l));
                off=lerp((off-.5)*1.02+.5,off*.94,l);
                float3 on=satColor(tex2D(_OnTex,uv).rgb,lerp(.92,1,l))*lerp(.88,1,l);
                float3 b=lerp(saturate(off),on,l);
                float2 p=float2(uv.x,1-uv.y);
                float rad=length((p-lerp(float2(.34,.38),float2(.35,.40),l))/lerp(float2(.42,.54),float2(.46,.58),l));
                float ra=saturate(1-rad/lerp(.68,.70,l))*lerp(.12,.24,l);
                float lineTint=saturate(1-(p.x*.883+p.y*.469)/lerp(.48,.51,l))*lerp(.2,.08,l);
                float strength=lerp(1,.24,l);
                float3 tint=lerp(float3(23,63,96),float3(255,225,171),l)/255;
                b=lerp(b,softLight(b,tint),saturate((ra+lineTint*(1-ra))*strength));
                return float4(b,i.color.a*uiClip(i));
            }
        ENDCG }
    }
}
