Shader "Miscalculation/CharacterLobby/Screen" {
    Properties { [PerRendererData] _MainTex ("Texture",2D)="white" {}
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15 }
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
            sampler2D _MainTex;
            fixed4 frag(v2f i):SV_Target {float4 c=tex2D(_MainTex,i.uv.xy)*i.color;c.a*=uiClip(i);return float4(c.rgb*c.a,c.a);}
        ENDCG }
    }
}
