using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    /// <summary>
    /// 可复用的按钮选中画线：从下方偏左缺口的左端起笔，顺时针画圈，
    /// 在缺口右端收笔后，再从圆圈左上方画一支位于圈外的箭头。
    /// 把它放在任意 Button 的无射线子节点上即可；它不读取或修改文字、布局、导航和业务 onClick。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SelectionDoodleGraphic : MaskableGraphic
    {
        const int CircleSegments=72;
        const int ArrowSegments=6;
        const float CirclePortion=.78f;
        public const float CircleStartDegrees=232f;
        public const float CircleClockwiseSweepDegrees=-322f;
        public static readonly Color32 ReferenceColor=new Color32(68,0,6,255);

        readonly Vector3[] worldCorners=new Vector3[4];
        readonly Vector2[] circlePoints=new Vector2[CircleSegments+1];
        readonly Vector2[] arrowPoints=new Vector2[ArrowSegments+1];
        readonly Vector2[] arrowLeftPoints=new Vector2[2],arrowRightPoints=new Vector2[2];
        readonly float[] pathDistances=new float[CircleSegments+1];

        [Tooltip("需要被圈选的范围。通常绑定按钮文字或按钮根；为空时使用父 RectTransform。")]
        public RectTransform fitTarget;
        [Tooltip("可选。启用后在该 Button 原有 onClick 执行时播放，不替换业务监听。")]
        public Button targetButton;
        public bool playOnTargetClick=true;
        [Tooltip("章节节点仅画圈；难度等文字选择可保留外置箭头。")]
        public bool showArrow=true;
        [Min(.05f)] public float duration=.72f;
        public Vector2 padding=new Vector2(18,10);
        [Range(1,12)] public float strokeWidth=3.8f;
        [Tooltip("描边两侧的抗锯齿羽化宽度（Canvas 设计像素）。")]
        [Range(.5f,3)] public float edgeFeather=1.15f;
        [Tooltip("仅控制圆圈开放笔画首尾的收尖距离（Canvas 设计像素）；外置箭头保持完整线宽。")]
        [Range(4,34)] public float taperLength=18;
        public UnityEvent completed=new UnityEvent();

        public bool IsSelected {get;private set;}
        public bool IsPlaying {get;private set;}
        public float Progress {get;private set;}
        public int LastQuadCount {get;private set;}
        public Vector2 LastFittedSize {get;private set;}
        public const int MaxQuadCount=(CircleSegments+ArrowSegments)*3;

        Vector2 lastCenter,lastSize;
        bool listenerBound,boundsReady;

        protected override void Awake()
        {
            base.Awake();raycastTarget=false;
            if(!fitTarget)fitTarget=transform.parent as RectTransform;
            if(!targetButton)targetButton=GetComponentInParent<Button>();
        }

        protected override void OnEnable(){base.OnEnable();Bind();SetVerticesDirty();}
        protected override void OnDisable(){Unbind();IsPlaying=false;base.OnDisable();}
        void Bind(){if(listenerBound||!playOnTargetClick||!targetButton)return;targetButton.onClick.AddListener(Play);listenerBound=true;}
        void Unbind(){if(!listenerBound)return;if(targetButton)targetButton.onClick.RemoveListener(Play);listenerBound=false;}

        public void SetDuration(float seconds){duration=Mathf.Max(.05f,seconds);}
        public void Play(){SetSelected(true,true);}
        public void ShowImmediate(){SetSelected(true,false);}
        public void Hide(){SetSelected(false,false);}
        public void SetSelected(bool selected,bool animate)
        {
            IsSelected=selected;Progress=selected?(animate?0:1):0;IsPlaying=selected&&animate;
            canvasRenderer.SetAlpha(selected?1:0);SetVerticesDirty();
            if(selected&&!animate)completed.Invoke();
        }

        void LateUpdate()
        {
            if(IsPlaying){float dt=Time.unscaledDeltaTime;if(!MotionMath.Finite(dt)||dt<0)dt=0;Progress=Mathf.Clamp01(Progress+dt/Mathf.Max(.05f,duration));SetVerticesDirty();if(Progress>=1){IsPlaying=false;completed.Invoke();}}
            if(ReadBounds(out Vector2 center,out Vector2 size)&&(!boundsReady||(center-lastCenter).sqrMagnitude>.0001f||(size-lastSize).sqrMagnitude>.0001f)){lastCenter=center;lastSize=size;boundsReady=true;SetVerticesDirty();}
        }

        bool ReadBounds(out Vector2 center,out Vector2 size)
        {
            center=Vector2.zero;size=Vector2.zero;if(!fitTarget)return false;fitTarget.GetWorldCorners(worldCorners);
            Vector2 min=new Vector2(float.PositiveInfinity,float.PositiveInfinity),max=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
            for(int i=0;i<4;i++){Vector3 p=rectTransform.InverseTransformPoint(worldCorners[i]);if(!MotionMath.Finite(p.x)||!MotionMath.Finite(p.y))return false;min=Vector2.Min(min,p);max=Vector2.Max(max,p);}
            center=(min+max)*.5f;size=max-min;return MotionMath.Finite(size.x)&&MotionMath.Finite(size.y)&&size.x>0&&size.y>0;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();LastQuadCount=0;if(!IsSelected||Progress<=0||!ReadBounds(out Vector2 center,out Vector2 size))return;
            lastCenter=center;lastSize=size;boundsReady=true;
            LastFittedSize=size;
            float safePaddingX=MotionMath.Finite(padding.x)?Mathf.Clamp(padding.x,0,200):18;
            float safePaddingY=MotionMath.Finite(padding.y)?Mathf.Clamp(padding.y,0,100):10;
            float safeWidth=MotionMath.Finite(strokeWidth)?Mathf.Clamp(strokeWidth,1,12):3.8f;
            float rx=Mathf.Max(22,size.x*.5f+safePaddingX),ry=Mathf.Max(17,size.y*.5f+safePaddingY);
            for(int i=0;i<=CircleSegments;i++){
                float u=i/(float)CircleSegments;
                float angle=(CircleStartDegrees+CircleClockwiseSweepDegrees*u)*Mathf.Deg2Rad;
                // 连续低频扰动保留手画的不规则感，同时避免逐段矩形造成折角。
                float radial=1+.014f*Mathf.Sin(u*Mathf.PI*4.1f+.35f)+.008f*Mathf.Sin(u*Mathf.PI*9.2f+1.1f);
                float x=Mathf.Cos(angle)*rx*radial;
                float y=Mathf.Sin(angle)*ry*(1+.020f*Mathf.Sin(u*Mathf.PI*5.3f+.8f));
                circlePoints[i]=center+new Vector2(x,y);
            }

            // 参考切图：箭头在圆圈左上方，箭尖与圈线之间保留可见间隙。
            const float arrowTargetDegrees=150f;float arrowAngle=arrowTargetDegrees*Mathf.Deg2Rad;
            Vector2 radialDirection=new Vector2(Mathf.Cos(arrowAngle),Mathf.Sin(arrowAngle)).normalized;
            Vector2 tip=center+new Vector2(Mathf.Cos(arrowAngle)*rx,Mathf.Sin(arrowAngle)*ry)+radialDirection*Mathf.Max(4.5f,safeWidth*1.35f);
            Vector2 tail=tip+new Vector2(-Mathf.Max(34,rx*.58f),Mathf.Max(19,ry*.88f));
            arrowPoints[0]=tail;
            arrowPoints[1]=Vector2.Lerp(tail,tip,.34f)+new Vector2(1.2f,-1.5f);
            arrowPoints[2]=Vector2.Lerp(tail,tip,.70f)+new Vector2(-.8f,1.0f);
            arrowPoints[3]=tip;
            Vector2 backward=(tail-tip).normalized;float headLength=Mathf.Max(13,safeWidth*4.4f);
            arrowPoints[4]=tip+Rotate(backward,34)*headLength;
            arrowPoints[5]=tip;
            arrowPoints[6]=tip+Rotate(backward,-34)*headLength;
            arrowLeftPoints[0]=arrowPoints[4];arrowLeftPoints[1]=arrowPoints[5];
            arrowRightPoints[0]=arrowPoints[5];arrowRightPoints[1]=arrowPoints[6];

            float circleT=Mathf.Clamp01(Progress/CirclePortion),arrowT=Mathf.Clamp01((Progress-CirclePortion)/(1-CirclePortion));
            float safeTaper=MotionMath.Finite(taperLength)?Mathf.Clamp(taperLength,4,34):18;
            AddSmoothPath(vh,circlePoints,CircleSegments,circleT,safeWidth,safeTaper,color);
            if(showArrow&&arrowT>0){
                // 收尖只属于圆圈。箭杆与两条箭翅保持完整线宽，避免短箭翅被首尾包络压没。
                AddSmoothPath(vh,arrowPoints,3,Mathf.Clamp01(arrowT*1.35f),safeWidth,0,color);
                if(arrowT>.58f){float headT=Mathf.Clamp01((arrowT-.58f)/.42f);AddSmoothPath(vh,arrowLeftPoints,1,headT,safeWidth,0,color);AddSmoothPath(vh,arrowRightPoints,1,headT,safeWidth,0,color);}
            }
        }

        static Vector2 Rotate(Vector2 v,float degrees){float a=degrees*Mathf.Deg2Rad,c=Mathf.Cos(a),s=Mathf.Sin(a);return new Vector2(v.x*c-v.y*s,v.x*s+v.y*c);}

        void AddSmoothPath(VertexHelper vh,Vector2[] points,int segments,float progress,float width,float taper,Color32 tint)
        {
            float scaled=Mathf.Clamp01(progress)*segments;int full=Mathf.Min(segments,Mathf.FloorToInt(scaled));float partial=scaled-full;
            int drawnSegments=full+(full<segments&&partial>.001f?1:0);if(drawnSegments<=0)return;
            Color32 transparent=tint;transparent.a=0;
            pathDistances[0]=0;
            for(int i=1;i<=drawnSegments;i++)pathDistances[i]=pathDistances[i-1]+Vector2.Distance(PathPoint(points,segments,full,partial,i-1),PathPoint(points,segments,full,partial,i));
            float drawnLength=pathDistances[drawnSegments];
            for(int i=0;i<=drawnSegments;i++){
                Vector2 point=PathPoint(points,segments,full,partial,i);
                Vector2 previous=PathPoint(points,segments,full,partial,Mathf.Max(0,i-1));
                Vector2 next=PathPoint(points,segments,full,partial,Mathf.Min(drawnSegments,i+1));
                Vector2 tangent=next-previous;if(tangent.sqrMagnitude<.0001f)tangent=Vector2.right;
                Vector2 normal=new Vector2(-tangent.y,tangent.x).normalized;
                float widthNoise=.96f+.055f*Mathf.Sin(i*1.17f+.4f)+.025f*Mathf.Sin(i*.43f+1.9f);
                float feather=MotionMath.Finite(edgeFeather)?Mathf.Clamp(edgeFeather,.5f,3):1.15f;
                float fromStart=pathDistances[i],toTip=drawnLength-pathDistances[i];
                float envelope=1;
                if(taper>.001f){
                    float startTaper=Mathf.SmoothStep(0,1,Mathf.Clamp01(fromStart/taper));
                    // 动画播放时当前笔尖也实时收尖；完成后它自然成为最终收笔尖端。
                    float endTaper=Mathf.SmoothStep(0,1,Mathf.Clamp01(toTip/taper));
                    envelope=Mathf.Max(.035f,Mathf.Min(startTaper,endTaper));
                }
                float half=Mathf.Max(.08f,width*widthNoise*.5f*envelope),outer=half+feather*envelope;
                int start=vh.currentVertCount;AddVertex(vh,point-normal*outer,transparent);AddVertex(vh,point-normal*half,tint);AddVertex(vh,point+normal*half,tint);AddVertex(vh,point+normal*outer,transparent);
                if(i>0){int previousStart=start-4;for(int band=0;band<3;band++){vh.AddTriangle(previousStart+band,previousStart+band+1,start+band+1);vh.AddTriangle(previousStart+band,start+band+1,start+band);LastQuadCount++;}}
            }
        }

        static Vector2 PathPoint(Vector2[] points,int segments,int full,float partial,int index)
        {
            if(index<=full)return points[Mathf.Min(index,segments)];
            return Vector2.Lerp(points[full],points[Mathf.Min(full+1,segments)],partial);
        }

        static void AddVertex(VertexHelper vh,Vector2 position,Color32 tint){UIVertex v=UIVertex.simpleVert;v.color=tint;v.position=position;vh.AddVert(v);}

#if UNITY_EDITOR
        protected override void Reset(){base.Reset();color=ReferenceColor;raycastTarget=false;}
        protected override void OnValidate(){base.OnValidate();duration=Mathf.Max(.05f,duration);strokeWidth=Mathf.Clamp(strokeWidth,1,12);edgeFeather=Mathf.Clamp(edgeFeather,.5f,3);taperLength=Mathf.Clamp(taperLength,4,34);raycastTarget=false;SetVerticesDirty();}
#endif
    }
}
