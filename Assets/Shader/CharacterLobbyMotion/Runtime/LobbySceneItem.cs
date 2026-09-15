using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.CharacterLobby
{
    /// <summary>
    /// Applies the lobby's pre-baked lamp and moonlight field to one scene Graphic.
    /// This component only handles lighting. Image appearance/disappearance belongs to
    /// the global motion package and is intentionally not implemented here.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LobbySceneItem : MonoBehaviour
    {
        [Tooltip("要处理的 uGUI 图片；为空时读取同物体 Graphic。纯 UI 不应挂此组件。")]
        public Graphic target;
        [Tooltip("提供共享灯光状态与材质模板。组件不会读取或修改业务数据。")]
        public LobbyController controller;

        Material runtimeMaterial;

        void Awake()
        {
            if(!target)target=GetComponent<Graphic>();
        }

        void Start()
        {
            if(!controller)controller=GetComponentInParent<LobbyController>();
            if(!controller||!target){Debug.LogError("[LobbySceneItem] 缺少 Controller 或 Graphic",this);enabled=false;return;}
            controller.Initialize();
            controller.RegisterSceneItem(this);
            enabled=false;
        }

        void OnDestroy()
        {
            if(controller)controller.UnregisterSceneItem(this);
            if(runtimeMaterial)Destroy(runtimeMaterial);
        }

        internal void Configure(Material template,Texture2D lightField,Vector4 designRect,LobbyParameters p,float lamp)
        {
            if(!target||!template)return;
            if(!runtimeMaterial){runtimeMaterial=new Material(template);runtimeMaterial.name=template.name+" ("+name+")";target.material=runtimeMaterial;}
            runtimeMaterial.SetTexture("_LightField",lightField);
            runtimeMaterial.SetVector("_DesignRect",designRect);
            ApplyLighting(p,lamp,designRect);
        }

        internal void ApplyLighting(LobbyParameters p,float lamp,Vector4 designRect)
        {
            if(!runtimeMaterial)return;
            runtimeMaterial.SetVector("_DesignRect",designRect);
            runtimeMaterial.SetFloat("_Lamp",Mathf.Clamp01(lamp));
            runtimeMaterial.SetFloat("_LampStrength",p.sceneLampStrength);
            runtimeMaterial.SetFloat("_FarBrightness",p.sceneFarBrightness);
            runtimeMaterial.SetFloat("_OffBrightness",p.sceneOffBrightness);
            runtimeMaterial.SetFloat("_MoonStrength",p.sceneMoonStrength);
        }
    }
}
