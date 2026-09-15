Shader "Miscalculation/CharacterLobby/Particles" {
    Properties { [PerRendererData] _MainTex ("Texture",2D)="white" {}
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15 _RainMask("Rain ABC",Vector)=(144,552,193,143)
        [NoScaleOffset] _RainOcclusionTex("Rain Occlusion",2D)="black" {} }
    SubShader {Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] ColorMask [_ColorMask]
        Blend One OneMinusSrcColor
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
            float4 _RainMask;
            sampler2D _RainOcclusionTex;
            fixed4 frag(v2f i):SV_Target {
                float kind=i.uv.z;float d;float2 uv=i.uv.xy;
                float2 designP=float2(i.local.x+960,540-i.local.y);
                // 非雨粒子不访问遮挡贴图；正常运行最多仅雨线片元增加一次 R8 采样。
                float occlusion=0;
                if(kind<.5||kind>4.5)occlusion=tex2D(_RainOcclusionTex,float2(designP.x/1920,1-designP.y/1080)).r;
                if(kind>4.5){
                    float a=saturate(i.color.a*occlusion*uiClip(i));return float4(i.color.rgb*a,a);
                }
                if(kind<.5 || kind>2.5) {
                    float ratio=max(.01,i.uv.w);
                    float2 q=float2(uv.x,max(0,abs(uv.y)*ratio-max(0,ratio-1)));
                    d=length(q);
                }else d=length(uv);
                float aa=max(.02,fwidth(d));
                float coverage=1-smoothstep(1-aa,1+aa,d);
                if(kind>1.5&&kind<3.5)coverage*=pow(saturate(1-d),kind<2.5?1.5:1);
                if(kind<.5){
                    float2 p=designP;
                    float t=p.y/max(1,_RainMask.w);
                    coverage*=step(0,t)*step(t,1)*step(lerp(_RainMask.x,_RainMask.z,t),p.x)*step(p.x,lerp(_RainMask.y,_RainMask.z,t));
                    coverage*=1-occlusion;
                }
                float a=saturate(i.color.a*coverage*uiClip(i));return float4(i.color.rgb*a,a);
            }
        ENDCG }
    }
}
