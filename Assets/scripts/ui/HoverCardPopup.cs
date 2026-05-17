using UnityEngine;
using UnityEngine.EventSystems;
public class HoverCardPopup : HoverBasePopup
{
    protected override bool _onPointerEnterHandle()
    {   
        CardPartPopup cardPartPopup = this.popup.gameObject.GetComponent<CardPartPopup>();
        if (cardPartPopup != null) {
            CardPart cardPart = this.gameObject.GetComponent<CardPart>();
            if (cardPart != null){
                cardPartPopup.loadPartInfo(cardPart.getPartInfo());
                cardPartPopup.setAssembleCard(cardPart.getAssembleCard());
                return true;
            } 
        }
        return false;
    }
}