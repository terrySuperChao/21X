using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Miscalculation.CharacterLobby
{
    /// <summary>只管理自身效果子树。不会创建/替换 Canvas、EventSystem 或业务 Button，不修改全局时间/渲染设置。</summary>
    public sealed class LobbyController : MonoBehaviour
    {
        public LobbyArt art;
        public LobbySettings defaults;
        public RectTransform designRoot;
        [Tooltip("可选：场景前景阴影层。应位于受光物件之上、纯 UI 之下；未绑定时回退到效果根。")]
        public RectTransform sceneForegroundShadowRoot;
        [Tooltip("可选：全场景黑场层。由接入者指定覆盖范围；不绑定时自动在自身效果下创建。")]
        public Image transitionCover;
        public bool showBackground=true,playEntranceOnEnable=true;
        public GameObject debugConsoleRoot;
        public LobbyParameters Current {get;private set;}
        public LobbySimulation Simulation {get;private set;}
        public float LampLevel {get;private set;}
        public bool Paused {get;set;}
        public bool Transitioning=>fadeActive;
        public float BlackAlpha=>transitionCover?transitionCover.color.a:0;
        public event Action ParametersChanged,EntranceCompleted,ExitCompleted;
        RawImage background,windowShadow,smokeA,smokeB;
        LobbyParticleGraphic particles;Material bgMaterial,fxMaterial;
        int lastSmokeFrameA=-1,lastSmokeFrameB=-1;
        float lampStart,lampTarget,lampElapsed,fadeStart,fadeTarget,fadeElapsed,fadeDuration;
        bool ready,fadeActive,fadeEnter;
        readonly List<LobbySceneItem> sceneItems=new List<LobbySceneItem>(12);
        readonly Vector3[] designCorners=new Vector3[4];
        Vector4 designScreenRect=new Vector4(0,0,1,1);
        int lastScreenWidth=-1,lastScreenHeight=-1;
        double smokeClock;
        int SmokeFrameCount=>art?art.FrameCount:0;
        public int SmokeFrame=>SmokeFrameCount>0?((int)smokeClock)%SmokeFrameCount:0;
        public int ParticleQuadCount=>particles?particles.LastQuadCount:0;
        static readonly int Lamp=Shader.PropertyToID("_Lamp"),RainMask=Shader.PropertyToID("_RainMask"),RainOcclusionTex=Shader.PropertyToID("_RainOcclusionTex");
        void Start(){Initialize();}
        void OnEnable(){if(ready&&playEntranceOnEnable)PlayEntrance();}
        public void Initialize()
        {
            if(ready)return;
            if(!art||!art.IsValid||!designRoot){Debug.LogError("[Lobby] 缺少美术或 1920×1080 效果根节点绑定",this);enabled=false;return;}
            Current=defaults&&defaults.parameters!=null?defaults.parameters.Clone():new LobbyParameters();
            try{Current.Validate();}catch(Exception e){Debug.LogError("[Lobby] 非法出厂参数："+e.Message,this);enabled=false;return;}
            Simulation=new LobbySimulation(Current);LampLevel=Current.lampOn?1:0;lampStart=lampTarget=LampLevel;
            bgMaterial=new Material(art.backgroundMaterial);fxMaterial=new Material(art.particleMaterial);fxMaterial.SetTexture(RainOcclusionTex,art.rainOcclusionMask);
            bgMaterial.SetTexture("_OffTex",art.lampOff);bgMaterial.SetTexture("_OnTex",art.lampOn);
            if(showBackground){background=NewRect<RawImage>("Lobby Background",designRoot);background.texture=art.lampOff;background.material=bgMaterial;background.transform.SetAsFirstSibling();}
            windowShadow=NewRect<RawImage>("Lobby Window Frame Shadow",sceneForegroundShadowRoot?sceneForegroundShadowRoot:designRoot);windowShadow.texture=art.lampOffWindowShadow;
            particles=NewRect<LobbyParticleGraphic>("Lobby Environment",designRoot);particles.material=fxMaterial;particles.owner=this;
            smokeA=NewRect<RawImage>("Smoke Frame A",designRoot);smokeB=NewRect<RawImage>("Smoke Frame B",designRoot);
            smokeA.material=smokeB.material=art.screenMaterial;
            if(!transitionCover){transitionCover=NewRect<Image>("Scene Black Transition",designRoot);transitionCover.color=new Color(0,0,0,0);}
            transitionCover.transform.SetAsLastSibling();transitionCover.raycastTarget=false;
            ready=true;RefreshDesignScreenRect(true);UpdateMaterial();RenderSmoke();
            if(playEntranceOnEnable)PlayEntrance();
        }
        public static T NewRect<T>(string name,Transform parent) where T:Graphic
        {var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer));go.transform.SetParent(parent,false);T g=go.AddComponent<T>();g.raycastTarget=false;var r=g.rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;return g;}
        /// <summary>参数先完整校验并克隆，失败保持当前效果、种子和灯光状态不变。</summary>
        public void Apply(LobbyParameters p)
        {
            if(!ready)Initialize();if(!ready)throw new InvalidOperationException("大厅尚未初始化");
            p.Validate();LobbyParameters copy=p.Clone();bool seed=copy.seed!=Current.seed;
            bool rain=copy.rainMaskTopLeftX!=Current.rainMaskTopLeftX||copy.rainMaskTopRightX!=Current.rainMaskTopRightX||copy.rainMaskBottomX!=Current.rainMaskBottomX||copy.rainMaskBottomY!=Current.rainMaskBottomY;
            bool smokeWidth=copy.smokeStrandWidth!=Current.smokeStrandWidth;
            bool lamp=copy.lampOn!=Current.lampOn;Current=copy;
            if(seed){Simulation=new LobbySimulation(copy);smokeClock=0;}else if(rain)Simulation.Reflow(copy);
            if(smokeWidth)lastSmokeFrameA=lastSmokeFrameB=-1;
            if(lamp)BeginLamp(copy.lampOn);
            UpdateMaterial();ApplySceneLighting();ParametersChanged?.Invoke();RenderSmoke();particles.RequestRefresh();
        }
        public void SetLamp(bool on){if(!ready)Initialize();if(!ready)return;Current.lampOn=on;BeginLamp(on);ParametersChanged?.Invoke();}
        void BeginLamp(bool on){lampStart=LampLevel;lampTarget=on?1:0;lampElapsed=0;}
        /// <summary>完全禁用可选控制台（含热键/启动钮/事件监听），不暂停环境动效。</summary>
        public void SetDebugConsoleEnabled(bool value){if(debugConsoleRoot)debugConsoleRoot.SetActive(value);}
        public void PlayEntrance(){if(!ready)return;SetBlack(1);BeginFade(0,true);}
        public void PlayExit(){if(!ready)return;BeginFade(1,false);}
        void BeginFade(float target,bool enter){fadeStart=BlackAlpha;fadeTarget=target;fadeElapsed=0;fadeDuration=Current.reducedMotion?.18f:.52f;fadeEnter=enter;fadeActive=true;transitionCover.raycastTarget=true;}
        void SetBlack(float a){if(transitionCover)transitionCover.color=new Color(0,0,0,Mathf.Clamp01(a));}
        public void StepSmokeFrame(){Paused=true;int count=SmokeFrameCount;if(count>0)smokeClock=(Math.Floor(smokeClock)+1)%count;RenderSmoke();}
        public void ResetPlayback(){Simulation=new LobbySimulation(Current);smokeClock=0;RenderSmoke();}
        public void RegisterSceneItem(LobbySceneItem item)
        {
            if(!item)return;if(!ready)Initialize();if(!ready||sceneItems.Contains(item))return;sceneItems.Add(item);
            item.Configure(art.sceneItemMaterial,art.lightField,designScreenRect,Current,LampLevel);
        }
        public void UnregisterSceneItem(LobbySceneItem item){sceneItems.Remove(item);}
        void Update()
        {
            if(!ready)return;float dt=Mathf.Min(.05f,Time.unscaledDeltaTime);
            // 本地暂停不改变 Time.timeScale，控制台仍可切灯和调参。
            int count=SmokeFrameCount;
            if(!Paused&&count>0){Simulation.Step(dt,Current);smokeClock=(smokeClock+dt*4.8*Current.smokeRise*(Current.reducedMotion?.35:1))%count;}
            bool lightDirty=false;
            if(LampLevel!=lampTarget){lampElapsed+=dt;float t=Mathf.Clamp01(lampElapsed/Mathf.Max(.001f,Current.lampTransition*.001f));LampLevel=t>=1?lampTarget:Mathf.Lerp(lampStart,lampTarget,LobbyMath.Ease(t));lightDirty=true;}
            if(fadeActive){fadeElapsed+=dt;float t=Mathf.Clamp01(fadeElapsed/fadeDuration);SetBlack(t>=1?fadeTarget:Mathf.Lerp(fadeStart,fadeTarget,LobbyMath.Ease(t,fadeEnter)));
                if(t>=1){fadeActive=false;transitionCover.raycastTarget=fadeTarget>0;if(fadeEnter)EntranceCompleted?.Invoke();else ExitCompleted?.Invoke();}}
            if(Screen.width!=lastScreenWidth||Screen.height!=lastScreenHeight){RefreshDesignScreenRect(false);lightDirty=true;}
            if(lightDirty){UpdateMaterial();ApplySceneLighting();}RenderSmoke();
        }
        void LateUpdate(){if(ready)particles.RequestRefresh();}
        void UpdateMaterial()
        {
            if(bgMaterial)bgMaterial.SetFloat(Lamp,LampLevel);
            if(fxMaterial)fxMaterial.SetVector(RainMask,new Vector4(Current.rainMaskTopLeftX,Current.rainMaskTopRightX,Current.rainMaskBottomX,Current.rainMaskBottomY));
            if(windowShadow){bool visible=LampLevel<.999f;if(windowShadow.enabled!=visible)windowShadow.enabled=visible;if(visible)windowShadow.canvasRenderer.SetAlpha(1-LampLevel);}
        }
        void RefreshDesignScreenRect(bool force)
        {
            if(!designRoot)return;lastScreenWidth=Screen.width;lastScreenHeight=Screen.height;designRoot.GetWorldCorners(designCorners);
            Canvas canvas=designRoot.GetComponentInParent<Canvas>();Camera camera=canvas&&canvas.renderMode!=RenderMode.ScreenSpaceOverlay?canvas.worldCamera:null;
            Vector2 min=RectTransformUtility.WorldToScreenPoint(camera,designCorners[0]),max=RectTransformUtility.WorldToScreenPoint(camera,designCorners[2]);
            if(Screen.width>0&&Screen.height>0)designScreenRect=new Vector4(min.x/Screen.width,min.y/Screen.height,Mathf.Max(1,max.x-min.x)/Screen.width,Mathf.Max(1,max.y-min.y)/Screen.height);
            if(force)ApplySceneLighting();
        }
        void ApplySceneLighting()
        {
            for(int i=sceneItems.Count-1;i>=0;i--){var item=sceneItems[i];if(!item){sceneItems.RemoveAt(i);continue;}item.ApplyLighting(Current,LampLevel,designScreenRect);}
        }
        void RenderSmoke()
        {
            if(!ready||SmokeFrameCount<=0)return;int f=(int)smokeClock;float blend=(float)(smokeClock-f);
            float opacity=Mathf.Clamp01(Mathf.Lerp(.62f,1.08f,Mathf.Clamp01((Current.smokeDensity-.25f)/1.35f))*Current.smokeVisibility);
            bool visible=Current.smokeEnabled;
            if(smokeA.enabled!=visible)smokeA.enabled=visible;if(smokeB.enabled!=visible)smokeB.enabled=visible;
            if(!visible)return;
            RenderFrame(smokeA,f,opacity*(1-blend),ref lastSmokeFrameA);RenderFrame(smokeB,(f+1)%SmokeFrameCount,opacity*blend,ref lastSmokeFrameB);
        }
        void RenderFrame(RawImage view,int frame,float alpha,ref int lastFrame)
        {
            if(lastFrame!=frame){
                view.texture=art.smokeAtlases[frame/6];Vector2 anchor=art.anchors[frame];
                float w=282*Current.smokeStrandWidth,h=456;RectTransform r=view.rectTransform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(1564-anchor.x/512*w,-399+anchor.y/512*h);
                int local=frame%6,col=local%3,row=local/3;view.uvRect=new Rect((col*512+.5f)/1536,(512*(1-row)+.5f)/1024,511f/1536,511f/1024);lastFrame=frame;
            }
            view.canvasRenderer.SetAlpha(alpha);
        }
        void OnDestroy(){if(bgMaterial)Destroy(bgMaterial);if(fxMaterial)Destroy(fxMaterial);}
    }
}
