using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private Texture _normalTexture;
    public Texture hoverTexture;
    public RawImage rawImage;
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.rawImage == null) {
            this.rawImage = this.gameObject.GetComponent<RawImage>();
        }
         
        if (this.rawImage == null) {
            return;
        }

        if (this._normalTexture == null) {
            this._normalTexture = this.rawImage.texture;
        }

        if (this.hoverTexture != null) {
            this.rawImage.texture = this.hoverTexture;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.rawImage == null){
            return;
        }

        if (this._normalTexture != null){
            this.rawImage.texture = this._normalTexture;
        }
    }
}