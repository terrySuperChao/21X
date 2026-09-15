using UnityEngine;
namespace Miscalculation.CharacterLobby
{
    /// <summary>可选等比适配器，仅修改自己的 RectTransform。背景和所有环境效果共用设计根。</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class LobbyDesignFit : MonoBehaviour
    {
        RectTransform rect,parentRect;Vector2 last;
        void Awake(){rect=(RectTransform)transform;parentRect=rect.parent as RectTransform;last=new Vector2(-1,-1);}
        void LateUpdate(){if(!parentRect)return;Vector2 size=parentRect.rect.size;if(size==last)return;last=size;float scale=Mathf.Min(size.x/1920,size.y/1080);if(!LobbyMath.Finite(scale)||scale<=0)return;
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(1920,1080);rect.localScale=Vector3.one*scale;}
    }
}
