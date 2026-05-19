using UnityEngine;
using UnityEngine.EventSystems;
public class HoverBuffPopup : HoverBasePopup
{
    protected override bool _onPointerEnterHandle()
    {   
        BuffPartPopup buffPartPopup = this.popup.gameObject.GetComponent<BuffPartPopup>();
        if (buffPartPopup != null) {
            BuffPart buffPart = this.gameObject.GetComponent<BuffPart>();
            if (buffPart != null){
                buffPartPopup.setUser(buffPart.getUser());
                buffPartPopup.setBuffType(buffPart.getBuffType());
                return true;
            }
        }
        return false;
    }
}