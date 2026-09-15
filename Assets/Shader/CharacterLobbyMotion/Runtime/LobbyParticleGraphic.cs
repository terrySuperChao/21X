using UnityEngine;
using UnityEngine.UI;
namespace Miscalculation.CharacterLobby
{
    /// <summary>环境粒子合并为一个 Graphic。最大 168 + 280 + 3 + 7 + 42 + 4 = 504 个四边形。</summary>
    public sealed class LobbyParticleGraphic : MaskableGraphic
    {
        public LobbyController owner;
        public int LastQuadCount {get;private set;}
        static readonly Vector3[] Ash={new Vector3(1498,365,1.05f),new Vector3(1522,349,.78f),new Vector3(1543,373,1.18f),new Vector3(1587,353,.84f),new Vector3(1612,378,1.12f),new Vector3(1641,360,.72f),new Vector3(1657,393,.92f)};
        protected override void Awake(){base.Awake();raycastTarget=false;}
        public void RequestRefresh(){if(!CanvasUpdateRegistry.IsRebuildingGraphics())SetVerticesDirty();}
        static Color C(float r,float g,float b,float a)=>new Color(r/255,g/255,b/255,Mathf.Clamp01(a));
        void Quad(VertexHelper vh,Vector2 p,Vector2 half,Color c,float kind,float rotation=0)
        {
            if(!LobbyMath.Finite(p.x)||!LobbyMath.Finite(p.y)||!LobbyMath.Finite(half.x)||!LobbyMath.Finite(half.y)||half.x<=0||half.y<=0||LastQuadCount>=504)return;
            int start=vh.currentVertCount;float co=Mathf.Cos(rotation),si=Mathf.Sin(rotation);
            for(int j=0;j<4;j++){
                float x=(j==0||j==3)?-1:1,y=j<2?-1:1;
                Vector2 q=new Vector2(x*half.x,y*half.y);q=new Vector2(q.x*co-q.y*si,q.x*si+q.y*co)+p;
                UIVertex v=UIVertex.simpleVert;v.position=new Vector3(q.x-960,540-q.y);v.color=c;v.uv0=new Vector4(x,y,kind,half.y/half.x);vh.AddVert(v);
            }
            vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);LastQuadCount++;
        }
        void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color c,float kind)
        {Vector2 d=b-a;Quad(vh,(a+b)*.5f,new Vector2(width*.5f,d.magnitude*.5f+width*.5f),c,kind,Mathf.Atan2(d.y,d.x)-Mathf.PI*.5f);}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();LastQuadCount=0;
            if(!owner||owner.Simulation==null||!LobbyMath.Finite(rectTransform.lossyScale.x)||Mathf.Abs(rectTransform.lossyScale.x)<.00001f)return;
            LobbyParameters p=owner.Current;LobbySimulation sim=owner.Simulation;float t=(float)(sim.Time%10000),lamp=owner.LampLevel;
            if(p.rainEnabled){int n=Mathf.FloorToInt(LobbySimulation.RainCap*p.rainDensity/1.45f);for(int i=0;i<n;i++){
                var d=sim.Rain[i];float depth=Mathf.Clamp01(d.p.y/p.rainMaskBottomY),len=d.length*Mathf.Lerp(.94f,1.08f,depth);
                Line(vh,d.p,d.p+new Vector2(len*.12f,len),d.width,C(150,192,220,Mathf.Min(.42f,d.alpha*Mathf.Lerp(.82f,1.38f,depth))),0);
            }}
            if(p.dustEnabled&&lamp>.02f){int n=Mathf.FloorToInt(LobbySimulation.DustCap*Mathf.Min(p.dustDensity,1.25f)/1.25f);for(int i=0;i<n;i++){
                var d=sim.Motes[i];Vector2 q=d.p+new Vector2(Mathf.Sin(d.phase*1.7f)*4,Mathf.Cos(d.phase)*2);
                float a=Mathf.Min(.82f,d.alpha*p.dustBrightness*lamp*(p.reducedMotion?.45f:1)),r=d.radius*p.dustSize;
                Quad(vh,q,Vector2.one*r*2.6f,C(255,219,158,a*.19f),1);Quad(vh,q,Vector2.one*r,C(255,231,183,a),1);
            }}
            if(p.emberIntensity>0){float pulse=.62f+Mathf.Sin(t*2.1f)*.16f+Mathf.Sin(t*.71f+1.4f)*.12f;
                float a=Mathf.Clamp(pulse*Mathf.Lerp(1.3f,.72f,lamp)*p.emberIntensity,0,2.8f),size=p.emberSize;
                Vector2 q=new Vector2(1564,399);Quad(vh,q,Vector2.one*9*size,C(255,110,44,.42f*a),2);
                Quad(vh,q,new Vector2(4.8f,2.15f)*size,C(255,122,45,.66f*a),1,.39f);
                Vector2 offset=new Vector2(-.7f,-.25f)*size;offset=new Vector2(offset.x*Mathf.Cos(.39f)-offset.y*Mathf.Sin(.39f),offset.x*Mathf.Sin(.39f)+offset.y*Mathf.Cos(.39f));
                Quad(vh,q+offset,new Vector2(1.65f,.72f)*size,C(255,239,185,.82f*a),1,.39f);
            }
            if(p.sparkIntensity>0){for(int i=0;i<Ash.Length;i++){
                var e=Ash[i];float phase=i*1.731f+.43f,speed=.62f+(i%4)*.19f;
                float slow=.5f+Mathf.Sin(t*speed+phase)*.5f,fast=.5f+Mathf.Sin(t*speed*4.1f+phase*2.7f)*.5f;
                float flash=Mathf.Pow(Mathf.Clamp01(slow*.72f+fast*.28f),5.2f),a=Mathf.Clamp((.08f+flash*.68f)*p.sparkIntensity*Mathf.Lerp(1.18f,.76f,lamp),0,1.8f);
                Quad(vh,e,Vector2.one*e.z*(2.4f+flash*1.7f),C(255,180,90,a*.72f),2);
            }
            for(int i=0;i<sim.SparkCount;i++){
                var d=sim.Sparks[i];float progress=Mathf.Clamp01(d.age/d.life),fade=Mathf.Pow(1-progress,1.7f)*p.sparkIntensity*d.heat,tail=Mathf.Clamp01(progress*5);
                Line(vh,d.p,d.p-d.velocity*.025f*tail,d.size*4,C(255,76,24,.12f*fade),3);
                Line(vh,d.p,d.p-d.velocity*.018f*tail,d.size,C(255,190,88,.9f*fade),4);
                Quad(vh,d.p,Vector2.one*d.size*.58f,C(255,244,194,.95f*fade),1);
            }}
            if(p.rainMaskDebug){Vector2 a=new Vector2(p.rainMaskTopLeftX,1),b=new Vector2(p.rainMaskTopRightX,1),c=new Vector2(p.rainMaskBottomX,p.rainMaskBottomY);
                Line(vh,a,b,2,C(74,239,246,.95f),4);Line(vh,b,c,2,C(74,239,246,.95f),4);Line(vh,c,a,2,C(74,239,246,.95f),4);
                // 第四个调试四边形显示贴图遮挡范围，保持总网格上限不变。
                Quad(vh,new Vector2(960,540),new Vector2(960,540),C(244,90,217,.42f),5);
            }
        }
    }
}
