using UnityEngine;
namespace Miscalculation.CharacterLobby
{
    public static class LobbyMath
    {
        public static bool Finite(float v) => !float.IsNaN(v) && !float.IsInfinity(v);
        public static float Fract(float v) => v - Mathf.Floor(v);
        public static float Radical(uint v)
        {
            v = (v << 16) | (v >> 16);
            v = ((v & 0x00ff00ff) << 8) | ((v & 0xff00ff00) >> 8);
            v = ((v & 0x0f0f0f0f) << 4) | ((v & 0xf0f0f0f0) >> 4);
            v = ((v & 0x33333333) << 2) | ((v & 0xcccccccc) >> 2);
            v = ((v & 0x55555555) << 1) | ((v & 0xaaaaaaaa) >> 1);
            return (float)(v / 4294967296.0);
        }
        public static bool InRain(Vector2 p, LobbyParameters s)
        {
            float t = p.y / s.rainMaskBottomY;
            return t >= 0 && t <= 1 && p.x >= Mathf.Lerp(s.rainMaskTopLeftX, s.rainMaskBottomX, t) && p.x <= Mathf.Lerp(s.rainMaskTopRightX, s.rainMaskBottomX, t);
        }
        static readonly Vector2[] DustArea = { new Vector2(310,170), new Vector2(1180,238), new Vector2(1230,720), new Vector2(405,740) };
        public static bool InDust(Vector2 p)
        {
            bool inside = false;
            for (int i=0,j=3; i<4; j=i++) {
                Vector2 a=DustArea[i],b=DustArea[j];
                if ((a.y>p.y)!=(b.y>p.y) && p.x<(b.x-a.x)*(p.y-a.y)/(b.y-a.y)+a.x) inside=!inside;
            }
            return inside;
        }
        public static Vector2 Triangle(float u, float v, LobbyParameters s)
        { float r=Mathf.Sqrt(Mathf.Clamp01(u)); return new Vector2(s.rainMaskTopLeftX*(1-r)+s.rainMaskTopRightX*r*(1-v)+s.rainMaskBottomX*r*v,s.rainMaskBottomY*r*v); }
        // CSS cubic-bezier 的 X 反解，而不是将时间直接当作贝塞尔参数。
        public static float Ease(float x, bool enter=true)
        {
            float x1=enter?.22f:.55f, x2=enter?.23f:.67f, y1=enter?.72f:.05f, y2=enter?1:.19f;
            float lo=0,hi=1,t=0;
            for(int i=0;i<14;i++){t=(lo+hi)*.5f; float q=1-t; float a=3*q*q*t*x1+3*q*t*t*x2+t*t*t; if(a<x)lo=t;else hi=t;}
            float z=1-t; return 3*z*z*t*y1+3*z*t*t*y2+t*t*t;
        }
    }
    public struct LobbyRandom
    {
        uint state;
        public LobbyRandom(uint seed) { state=seed==0?1:seed; }
        public float Next() { state^=state<<13;state^=state>>17;state^=state<<5;return (float)(state/4294967296.0); }
        public float Range(float a,float b) => Mathf.Lerp(a,b,Next());
    }
}
