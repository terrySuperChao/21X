using System;
using UnityEngine;
namespace Miscalculation.CharacterLobby
{
    /// <summary>有界纯数据模拟。固定数组，更新无 LINQ、无粒子 GameObject、无逐帧分配。</summary>
    public sealed class LobbySimulation
    {
        public const int RainCap=168,DustCap=140,SparkCap=14;
        public struct Drop { public Vector2 p;public float u,v,rank,length,speed,alpha,width;public int cycle; }
        public struct Dust { public Vector2 p;public float phase,speed,radius,alpha; }
        public struct Spark { public Vector2 p,velocity;public float age,life,gravity,size,heat; }
        public readonly Drop[] Rain=new Drop[RainCap];
        public readonly Dust[] Motes=new Dust[DustCap];
        public readonly Spark[] Sparks=new Spark[SparkCap];
        public int SparkCount {get;private set;}
        public double Time {get;private set;}
        LobbyRandom random; float sparkClock,nextSpark;
        public LobbySimulation(LobbyParameters p)
        {
            random=new LobbyRandom(p.seed);
            for(int i=0;i<RainCap;i++){Rain[i].u=(i+random.Range(.16f,.84f))/RainCap;Rain[i].v=LobbyMath.Radical((uint)i+1);Rain[i].rank=LobbyMath.Radical((uint)i);Place(ref Rain[i],p,false);}
            Array.Sort(Rain,(a,b)=>a.rank.CompareTo(b.rank));
            for(int i=0;i<DustCap;i++)PlaceDust(ref Motes[i]);
            nextSpark=random.Range(.75f,2.15f);
        }
        void Place(ref Drop d,LobbyParameters s,bool reset)
        {
            if(reset){d.cycle++;d.u=LobbyMath.Fract(d.u+d.cycle*.61803398875f);d.v=LobbyMath.Fract(d.v+d.cycle*.41421356237f);}
            d.p=LobbyMath.Triangle(Mathf.Clamp(d.u+random.Range(-.012f,.012f),.001f,.999f),Mathf.Clamp(d.v+random.Range(-.018f,.018f),.001f,.999f),s);
            if(!LobbyMath.InRain(d.p,s))d.p.y=.5f;
            bool near=random.Next()>.76f;
            d.length=near?random.Range(20,42):random.Range(8,21);d.speed=near?random.Range(150,235):random.Range(70,140);
            d.alpha=near?random.Range(.16f,.31f):random.Range(.06f,.16f);d.width=near?random.Range(.9f,1.5f):random.Range(.45f,.85f);
        }
        void PlaceDust(ref Dust d)
        {
            d.p=new Vector2(600,400);
            for(int i=0;i<64;i++){Vector2 p=new Vector2(random.Range(300,1230),random.Range(165,745));if(LobbyMath.InDust(p)){d.p=p;break;}}
            d.phase=random.Range(0,Mathf.PI*2);d.speed=random.Range(.18f,.46f);d.radius=random.Range(.62f,1.72f);d.alpha=random.Range(.11f,.32f);
        }
        public void Reflow(LobbyParameters p){for(int i=0;i<RainCap;i++)Place(ref Rain[i],p,true);}
        public void Step(float dt,LobbyParameters p)
        {
            if(!LobbyMath.Finite(dt)||dt<=0)return;dt=Mathf.Min(dt,.05f);Time+=dt;
            for(int i=0;i<RainCap;i++){
                Rain[i].p.y+=Rain[i].speed*dt*p.rainSpeed*(p.reducedMotion?.22f:1);
                if(!LobbyMath.InRain(Rain[i].p,p))Place(ref Rain[i],p,true);
            }
            for(int i=0;i<DustCap;i++){
                Motes[i].phase+=Motes[i].speed*dt*(p.reducedMotion?.18f:1);
                Motes[i].p.y-=Motes[i].speed*2.2f*dt*(p.reducedMotion?.18f:1);
                if(!LobbyMath.InDust(Motes[i].p))PlaceDust(ref Motes[i]);
            }
            sparkClock+=dt;
            if(sparkClock>=nextSpark){sparkClock=0;nextSpark=random.Range(.75f,2.25f);
                if(p.sparkIntensity>0&&!p.reducedMotion){int count=random.Next()>.72f?2:1;for(int i=0;i<count&&SparkCount<SparkCap;i++){
                    Sparks[SparkCount++]=new Spark {p=new Vector2(1564+random.Range(-2.4f,2.4f),399+random.Range(-1.6f,1.6f)),velocity=new Vector2(random.Range(-24,17),random.Range(-88,-38)),gravity=random.Range(88,142),life=random.Range(.18f,.48f),size=random.Range(.65f,1.55f),heat=random.Range(.72f,1.18f)};
                }}
            }
            for(int i=SparkCount-1;i>=0;i--){Sparks[i].age+=dt;Sparks[i].p+=Sparks[i].velocity*dt;Sparks[i].velocity.y+=Sparks[i].gravity*dt;if(Sparks[i].age>=Sparks[i].life)Sparks[i]=Sparks[--SparkCount];}
        }
        public bool IsFinite()
        {
            foreach(var d in Rain)if(!LobbyMath.Finite(d.p.x)||!LobbyMath.Finite(d.p.y))return false;
            foreach(var d in Motes)if(!LobbyMath.Finite(d.p.x)||!LobbyMath.Finite(d.p.y))return false;
            for(int i=0;i<SparkCount;i++)if(!LobbyMath.Finite(Sparks[i].p.x)||!LobbyMath.Finite(Sparks[i].p.y))return false;
            return SparkCount<=SparkCap;
        }
    }
}
