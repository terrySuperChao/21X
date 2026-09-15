using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Miscalculation.CharacterLobby.Debugging
{
    /// <summary>可选运行时调参模块，不创建 Canvas/EventSystem。删除此程序集不影响核心动效。</summary>
    public sealed class LobbyConsole : MonoBehaviour
    {
        public LobbyController controller;
        [Tooltip("请使用项目已有中文 TMP 字体；不依赖验证工程字体。")]
        public TMP_FontAsset font;
        public bool restoreLastSavedPreset=true;
        public KeyCode toggleKey=KeyCode.F1;
        public bool Visible=>panel&&panel.gameObject.activeSelf;
        public bool Dirty {get;private set;}
        public string LastStatus {get;private set;}
        public string LastFile {get;private set;}
        RectTransform panel,tooltip,modal,content;
        GameObject launcher;TextMeshProUGUI status,stats,tooltipText,header;
        TMP_InputField presetName;Button lamp,pause;Action confirmed;
        readonly List<Slider> sliders=new List<Slider>();
        readonly List<TMP_InputField> inputs=new List<TMP_InputField>();
        readonly List<Toggle> toggles=new List<Toggle>();
        readonly List<Selectable> navigation=new List<Selectable>();
        LobbyParameters beforeImport;GameObject previousFocus;bool built,sync;float statTime,frameTotal;int frames;
        static readonly Color Ink=new Color(.075f,.085f,.115f,.98f),Paper=new Color(.9f,.88f,.83f),Accent=new Color(.3f,.86f,.85f),FieldColor=new Color(.12f,.14f,.18f);
        string Key=>"CharacterLobbyMotion.LastPreset."+controller.art.contentHash;
        void Start()
        {
            if(!controller){enabled=false;return;}controller.Initialize();if(controller.Current==null){enabled=false;return;}
            if(!font)font=TMP_Settings.defaultFontAsset;
            Build();built=true;controller.ParametersChanged+=Changed;RefreshValues();
            if(restoreLastSavedPreset&&PlayerPrefs.HasKey(Key)){
                string path=PlayerPrefs.GetString(Key);if(File.Exists(path))TryImport(path);else SetStatus("上次保存文件不存在，使用出厂参数。",false);
            }
            if(string.IsNullOrEmpty(LastStatus))SetStatus("调参只影响当前运行；请导出 JSON 保存。F1 隐藏/展开。",false);
        }
        void OnEnable(){if(built){controller.ParametersChanged+=Changed;RefreshValues();}}
        void OnDisable(){if(controller)controller.ParametersChanged-=Changed;if(tooltip)tooltip.gameObject.SetActive(false);if(modal)modal.gameObject.SetActive(false);confirmed=null;
            if(EventSystem.current&&EventSystem.current.currentSelectedGameObject&&EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))EventSystem.current.SetSelectedGameObject(previousFocus);}
        void Changed(){Dirty=true;if(built)RefreshValues();}
        RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        Image Box(RectTransform r,Color c,bool raycast=true){var img=r.gameObject.AddComponent<Image>();img.color=c;img.raycastTarget=raycast;return img;}
        TextMeshProUGUI Text(Transform parent,string text,float x,float y,float w,float h,float size=19)
        {var r=Rect(text,parent,x,y,w,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.fontSize=size;t.color=Paper;t.text=text;t.raycastTarget=false;t.enableWordWrapping=true;t.overflowMode=TextOverflowModes.Overflow;return t;}
        Button Button(Transform parent,string label,float x,float y,float w,float h,Action action)
        {var r=Rect(label,parent,x,y,w,h);var img=Box(r,FieldColor);var b=r.gameObject.AddComponent<Button>();b.targetGraphic=img;var colors=b.colors;colors.highlightedColor=new Color(.65f,.9f,.93f);colors.selectedColor=new Color(.65f,.9f,.93f);b.colors=colors;
            var t=Text(r,label,6,3,w-12,h-6,18);t.alignment=TextAlignmentOptions.Center;b.onClick.AddListener(()=>action());navigation.Add(b);return b;}
        TMP_InputField Input(Transform parent,string label,float x,float y,float w,float h)
        {
            var r=Rect(label,parent,x,y,w,h);var img=Box(r,FieldColor);var input=r.gameObject.AddComponent<TMP_InputField>();input.targetGraphic=img;
            var viewport=Rect("Text Area",r,7,2,w-14,h-4);viewport.gameObject.AddComponent<RectMask2D>();var t=Text(viewport,"",0,0,w-14,h-4,18);t.alignment=TextAlignmentOptions.MidlineLeft;t.enableWordWrapping=false;input.textViewport=viewport;input.textComponent=t;input.fontAsset=font;input.customCaretColor=true;input.caretColor=Accent;input.characterLimit=120;navigation.Add(input);return input;
        }
        Toggle Toggle(Transform parent,string label,float x,float y,float w,Action<bool> action,string description)
        {
            var r=Rect(label,parent,x,y,w,32);var hit=Box(r,new Color(0,0,0,0));var toggle=r.gameObject.AddComponent<Toggle>();toggle.targetGraphic=hit;
            var square=Rect("Box",r,0,4,22,22);Box(square,FieldColor);var check=Rect("Check",square,4,4,14,14);toggle.graphic=Box(check,Accent,false);
            Text(r,label,30,2,w-30,30,17);toggle.onValueChanged.AddListener(v=>{if(!sync)action(v);});AddTip(r,description);toggles.Add(toggle);navigation.Add(toggle);return toggle;
        }
        void AddTip(RectTransform r,string text)
        {var tip=r.gameObject.AddComponent<LobbyTooltipTarget>();tip.owner=this;tip.description=text;if(!r.GetComponent<Graphic>())Box(r,new Color(0,0,0,0));}
        void Build()
        {
            panel=Rect("Lobby Parameter Console",transform,0,18,484,1044);panel.anchorMin=panel.anchorMax=new Vector2(1,1);panel.pivot=new Vector2(1,1);panel.anchoredPosition=new Vector2(-18,-18);Box(panel,Ink);
            var title=Rect("Drag Header",panel,0,0,484,54);Box(title,new Color(.11f,.13f,.17f));var drag=title.gameObject.AddComponent<LobbyConsoleDrag>();drag.target=panel;
            header=Text(title,"角色大厅动效 · v"+LobbySettings.Version,18,10,386,36,21);
            Button(title,"收起",410,10,62,34,()=>SetVisible(false));
            Text(panel,"Unity 原生验证 / 1920×1080 / V4 RGBA · 24 帧",18,62,450,26,16).color=Accent;
            lamp=Button(panel,"台灯：开",18,96,140,38,()=>controller.SetLamp(!controller.Current.lampOn));
            pause=Button(panel,"暂停动效",172,96,140,38,()=>{controller.Paused=!controller.Paused;RefreshValues();});
            Button(panel,"烟雾下一帧",326,96,140,38,()=>{controller.StepSmokeFrame();RefreshValues();});
            Button(panel,"入场预览",18,142,140,36,controller.PlayEntrance);
            Button(panel,"离场至黑场",172,142,140,36,controller.PlayExit);
            Button(panel,"重置播放",326,142,140,36,controller.ResetPlayback);
            Toggle(panel,"青紫烟雾",18,190,142,v=>Mutate(p=>p.smokeEnabled=v),"仅开关独立烟雾层；当前背景自带的静态烟雾不会消失。正式换用无烟背景。");
            Toggle(panel,"窗口下雨",174,190,140,v=>Mutate(p=>p.rainEnabled=v),"开关窗口雨滴，不改变雨区校准参数。");
            Toggle(panel,"灯下微尘",326,190,140,v=>Mutate(p=>p.dustEnabled=v),"开关微尘；台灯关闭时自动不可见。");
            Toggle(panel,"雨区边界",18,226,200,v=>Mutate(p=>p.rainMaskDebug=v),"显示三角形雨区轮廓。窗口与所有效果共用同一设计坐标；不会存窗口像素。");
            Toggle(panel,"减少动态",250,226,214,v=>Mutate(p=>p.reducedMotion=v),"降低烟雾、雨滴和微尘速度，停止碎火星发射；黑场切换缩短为 180ms。");
            var scrollRect=Rect("Parameters Scroll",panel,14,272,456,544);Box(scrollRect,new Color(.04f,.05f,.07f,.8f));var scroll=scrollRect.gameObject.AddComponent<ScrollRect>();
            var viewport=Rect("Viewport",scrollRect,6,4,444,536);Box(viewport,new Color(0,0,0,0));viewport.gameObject.AddComponent<RectMask2D>();
            content=Rect("Content",viewport,0,0,436,LobbyParameterSpec.All.Length*74+24);scroll.viewport=viewport;scroll.content=content;scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=34;
            for(int i=0;i<LobbyParameterSpec.All.Length;i++){
                var spec=LobbyParameterSpec.All[i];float y=8+i*74;
                var label=Text(content,spec.Label,6,y,272,28,18);AddTip(label.rectTransform,spec.Description+"\n范围："+spec.Min+"—"+spec.Max+" "+spec.Unit+"；步长："+spec.Step+"。");label.raycastTarget=true;
                var inp=Input(content,spec.Key,310,y,114,30);inp.characterLimit=14;inputs.Add(inp);
                var track=Rect("Slider "+spec.Key,content,10,y+38,410,24);Box(track,new Color(.08f,.1f,.14f));var s=track.gameObject.AddComponent<Slider>();var fillArea=Rect("Fill Area",track,6,8,398,8);var fill=Rect("Fill",fillArea,0,0,398,8);fill.anchorMin=Vector2.zero;fill.anchorMax=Vector2.one;fill.offsetMin=fill.offsetMax=Vector2.zero;Box(fill,Accent,false);s.fillRect=fill;
                var handleArea=Rect("Handle Area",track,6,0,398,24);var handle=Rect("Handle",handleArea,0,0,16,0);handle.pivot=new Vector2(.5f,.5f);var hi=Box(handle,Paper);s.targetGraphic=hi;s.handleRect=handle;s.minValue=spec.Min;s.maxValue=spec.Max;sliders.Add(s);navigation.Add(s);
                s.onValueChanged.AddListener(v=>{if(sync)return;float q=Mathf.Round(v/spec.Step)*spec.Step;Mutate(p=>spec.Set(p,Mathf.Clamp(q,spec.Min,spec.Max)));});
                inp.onEndEdit.AddListener(v=>{if(sync)return;if(float.TryParse(v,NumberStyles.Float,CultureInfo.InvariantCulture,out float n)&&LobbyMath.Finite(n)&&n>=spec.Min&&n<=spec.Max)Mutate(p=>spec.Set(p,n));else{SetStatus(spec.Label+"：请输入范围内的数字。",true);RefreshValues();}});
            }
            Text(panel,"预设名称",18,828,110,30,18);presetName=Input(panel,"Preset Name",126,828,340,32);presetName.SetTextWithoutNotify("大厅自定义参数");presetName.onEndEdit.AddListener(_=>{Dirty=true;UpdateStatus();});
            Button(panel,"导入 JSON",18,872,140,38,ImportDialog);Button(panel,"导出 JSON",172,872,140,38,ExportDialog);Button(panel,"保存当前文件",326,872,140,38,SaveCurrent);
            Button(panel,"撤销导入",18,918,140,32,UndoImport);Button(panel,"恢复默认",172,918,140,32,()=>Confirm("恢复出厂参数？未保存调参将丢失。",()=>{controller.Apply(controller.defaults?controller.defaults.parameters:new LobbyParameters());SetStatus("已恢复出厂参数，尚未保存。",false);}));
            stats=Text(panel,"",326,918,140,32,15);
            status=Text(panel,"",18,958,448,76,16);status.overflowMode=TextOverflowModes.Ellipsis;
            launcher=Button(transform,"展开控制台 · F1",0,18,210,40,()=>SetVisible(true)).gameObject;var lr=(RectTransform)launcher.transform;lr.anchorMin=lr.anchorMax=lr.pivot=new Vector2(1,1);lr.anchoredPosition=new Vector2(-18,-18);launcher.SetActive(false);
            tooltip=Rect("Single Tooltip",transform,0,0,428,174);Box(tooltip,new Color(.035f,.045f,.065f,.99f),false);tooltipText=Text(tooltip,"",16,12,396,150,18);tooltip.gameObject.SetActive(false);
            modal=Rect("Confirm",panel,0,0,484,1044);Box(modal,new Color(0,0,0,.75f));var dialog=Rect("Dialog",modal,24,370,436,218);Box(dialog,new Color(.06f,.08f,.11f,1));Text(dialog,"",18,18,400,120,20);
            var yes=Button(dialog,"确认",30,160,170,40,()=>{var call=confirmed;CloseModal();call?.Invoke();});var no=Button(dialog,"取消",236,160,170,40,CloseModal);
            var yn=new Navigation{mode=Navigation.Mode.Explicit,selectOnLeft=no,selectOnRight=no,selectOnDown=no,selectOnUp=no};yes.navigation=yn;yn.selectOnLeft=yn.selectOnRight=yn.selectOnDown=yn.selectOnUp=yes;no.navigation=yn;modal.gameObject.SetActive(false);
        }
        void Mutate(Action<LobbyParameters> action){try{var p=controller.Current.Clone();action(p);controller.Apply(p);}catch(Exception e){SetStatus(e.Message,true);RefreshValues();}}
        public void SetVisible(bool visible)
        {
            if(!built)return;ShowTooltip(null,null);panel.gameObject.SetActive(visible);launcher.SetActive(!visible);frameTotal=statTime=0;frames=0;
            if(EventSystem.current){if(visible){previousFocus=EventSystem.current.currentSelectedGameObject;EventSystem.current.SetSelectedGameObject(lamp.gameObject);}else EventSystem.current.SetSelectedGameObject(launcher);}
        }
        public void ShowTooltip(RectTransform source,string description)
        {
            if(!tooltip)return;if(!source||string.IsNullOrEmpty(description)){tooltip.gameObject.SetActive(false);return;}
            tooltip.gameObject.SetActive(true);tooltip.SetAsLastSibling();tooltipText.text=description;
            var root=(RectTransform)transform;Vector3 point=root.InverseTransformPoint(source.position);float left=point.x+root.rect.width*root.pivot.x-436,top=root.rect.height*(1-root.pivot.y)-point.y;
            tooltip.anchoredPosition=new Vector2(Mathf.Clamp(left,8,Mathf.Max(8,root.rect.width-436)),-Mathf.Clamp(top,8,Mathf.Max(8,root.rect.height-182)));
        }
        void RefreshValues()
        {
            if(!built)return;sync=true;var p=controller.Current;
            for(int i=0;i<sliders.Count;i++){float v=LobbyParameterSpec.All[i].Get(p);sliders[i].SetValueWithoutNotify(v);if(!inputs[i].isFocused)inputs[i].SetTextWithoutNotify(v.ToString("0.###",CultureInfo.InvariantCulture));}
            bool[] values={p.smokeEnabled,p.rainEnabled,p.dustEnabled,p.rainMaskDebug,p.reducedMotion};for(int i=0;i<values.Length;i++)toggles[i].SetIsOnWithoutNotify(values[i]);
            lamp.GetComponentInChildren<TextMeshProUGUI>().text=p.lampOn?"台灯：开 → 关":"台灯：关 → 开";
            pause.GetComponentInChildren<TextMeshProUGUI>().text=controller.Paused?"继续动效":"暂停动效";sync=false;UpdateStatus();
        }
        void SetStatus(string message,bool error){LastStatus=message;if(status)status.color=error?new Color(1,.55f,.5f):Paper;UpdateStatus();}
        void UpdateStatus(){if(status)status.text=(Dirty?"[未保存] ":"[已保存/默认] ")+(LastStatus??"");}
        public bool TryImport(string path)
        {
            try{var preset=LobbyPreset.ReadFile(path,controller.art);var old=controller.Current.Clone();controller.Apply(preset.parameters);beforeImport=old;presetName.SetTextWithoutNotify(preset.presetName);LastFile=Path.GetFullPath(path);Dirty=false;PlayerPrefs.SetString(Key,LastFile);PlayerPrefs.Save();SetStatus("已导入："+Path.GetFileName(path)+"\n"+preset.importNote,false);return true;}
            catch(Exception e){SetStatus("导入失败，参数未改："+e.Message,true);return false;}
        }
        public bool TryExport(string path)
        {
            try{var preset=LobbyPreset.Create(controller.Current,controller.art,presetName.text);string json=preset.ToJson();LobbyPreset.Parse(json,controller.art);LobbyPreset.WriteFile(path,json);LastFile=Path.GetFullPath(path);PlayerPrefs.SetString(Key,LastFile);PlayerPrefs.Save();Dirty=false;SetStatus("已保存："+LastFile,false);return true;}catch(Exception e){SetStatus("保存失败："+e.Message,true);return false;}
        }
        string PresetDir {get {string p=Path.Combine(Application.persistentDataPath,"Presets");Directory.CreateDirectory(p);return p;}}
        void ImportDialog(){try{string path=LobbyFileDialog.Choose(false,PresetDir,"");if(!string.IsNullOrEmpty(path))TryImport(path);}catch(Exception e){SetStatus(e.Message,true);}}
        void ExportDialog(){try{string path=LobbyFileDialog.Choose(true,PresetDir,"Lobby_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".json");if(!string.IsNullOrEmpty(path))TryExport(path);}catch(Exception e){SetStatus(e.Message,true);}}
        void SaveCurrent(){if(string.IsNullOrEmpty(LastFile)){ExportDialog();return;}Confirm("覆盖保存当前 JSON 文件？\n"+Path.GetFileName(LastFile),()=>TryExport(LastFile));}
        void UndoImport(){if(beforeImport==null){SetStatus("暂无可撤销的导入。",false);return;}controller.Apply(beforeImport);beforeImport=null;SetStatus("已撤销上次导入，当前值尚未保存。",false);}
        void Confirm(string text,Action action){confirmed=action;modal.GetComponentInChildren<TextMeshProUGUI>().text=text;modal.gameObject.SetActive(true);modal.SetAsLastSibling();if(EventSystem.current)EventSystem.current.SetSelectedGameObject(modal.GetComponentInChildren<Button>().gameObject);}
        void CloseModal(){modal.gameObject.SetActive(false);confirmed=null;if(EventSystem.current)EventSystem.current.SetSelectedGameObject(lamp.gameObject);}
        void Update()
        {
            if(!built)return;
            #if ENABLE_LEGACY_INPUT_MANAGER
            bool typing=EventSystem.current&&EventSystem.current.currentSelectedGameObject&&EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if(!typing&&InputKey(toggleKey)&&!modal.gameObject.activeSelf)SetVisible(!Visible);
            if(InputKey(KeyCode.Tab))MoveFocus(typing);
            #endif
            if(Visible){frameTotal+=Time.unscaledDeltaTime;frames++;statTime+=Time.unscaledDeltaTime;if(statTime>=.5f){stats.SetText("{0:1} FPS · 帧 {1:0}",frames/Mathf.Max(.001f,frameTotal),controller.SmokeFrame+1);frames=0;frameTotal=statTime=0;}}
        }
        #if ENABLE_LEGACY_INPUT_MANAGER
        static bool InputKey(KeyCode key)=>UnityEngine.Input.GetKeyDown(key);
        void MoveFocus(bool typing){if(!EventSystem.current)return;if(modal.gameObject.activeSelf)return;int index=navigation.FindIndex(s=>s&&s.gameObject==EventSystem.current.currentSelectedGameObject);int dir=UnityEngine.Input.GetKey(KeyCode.LeftShift)||UnityEngine.Input.GetKey(KeyCode.RightShift)?-1:1;
            for(int i=0;i<navigation.Count;i++){index=(index+dir+navigation.Count)%navigation.Count;var s=navigation[index];if(s&&s.IsActive()&&s.IsInteractable()){EventSystem.current.SetSelectedGameObject(s.gameObject);if(s.transform.IsChildOf(content)){float y=-((RectTransform)s.transform).anchoredPosition.y;content.anchoredPosition=new Vector2(0,Mathf.Clamp(y-200,0,content.rect.height-536));}break;}}}
        #endif
    }
    public sealed class LobbyTooltipTarget : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISelectHandler,IDeselectHandler
    {
        public LobbyConsole owner;public string description;
        public void OnPointerEnter(PointerEventData e){owner.ShowTooltip((RectTransform)transform,description);}
        public void OnPointerExit(PointerEventData e){owner.ShowTooltip(null,null);}
        public void OnSelect(BaseEventData e){owner.ShowTooltip((RectTransform)transform,description);}
        public void OnDeselect(BaseEventData e){owner.ShowTooltip(null,null);}
        void OnDisable(){if(owner)owner.ShowTooltip(null,null);}
    }
    public sealed class LobbyConsoleDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public RectTransform target;Vector2 start,position;
        public void OnBeginDrag(PointerEventData e){RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)target.parent,e.position,e.pressEventCamera,out start);position=target.anchoredPosition;}
        public void OnDrag(PointerEventData e){var parent=(RectTransform)target.parent;RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,e.position,e.pressEventCamera,out Vector2 p);Vector2 value=position+p-start;value.x=Mathf.Clamp(value.x,-Mathf.Max(0,parent.rect.width-target.rect.width),0);value.y=Mathf.Clamp(value.y,-Mathf.Max(0,parent.rect.height-54),0);target.anchoredPosition=value;}
        public void OnEndDrag(PointerEventData e){}
    }
}
