Shader "Miscalculation/Motion/MemoryReveal"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _NoiseTex("Noise", 2D) = "gray" {}
        _Visibility("Visibility", Range(0,1)) = 1
        _SeedOffset("Seed", Vector) = (0,0,0,0)
        _FragmentScale("Fragment Scale", Range(.45,2.5)) = 1.15
        _FragmentSoftness("Fragment Softness", Range(.008,.24)) = .055
        _FragmentIrregularity("Irregularity", Range(0,1)) = .68
        _FringeStrength("Fringe", Range(0,.65)) = .24
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; fixed4 color:COLOR; float2 uv:TEXCOORD0; };
            sampler2D _MainTex, _NoiseTex; fixed4 _Color; float4 _SeedOffset;
            float _Visibility, _FragmentScale, _FragmentSoftness, _FragmentIrregularity, _FringeStrength;
            v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 baseColor=tex2D(_MainTex,i.uv)*i.color;
                float2 p=(i.uv-.5)*_FragmentScale+.5+_SeedOffset.xy;
                float n=tex2D(_NoiseTex,p).r;
                float n2=tex2D(_NoiseTex,p*2.37+_SeedOffset.zw).g;
                float islands=lerp(n,n*.72+n2*.28,_FragmentIrregularity);
                float threshold=1-_Visibility;
                float mask=smoothstep(threshold-_FragmentSoftness,threshold+_FragmentSoftness,islands);
                float fringe=smoothstep(threshold-_FragmentSoftness*2.1,threshold,islands)*(1-mask)*_FringeStrength;
                baseColor.rgb+=float3(.18,.03,.24)*fringe;
                baseColor.a*=saturate(mask+fringe);
                return baseColor;
            }
            ENDCG
        }
    }
}
