using UnityEngine;
using UnityEngine.EventSystems;
public class HoverBasePopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public RectTransform popup;
    public Vector2 offset = new Vector2(-210, 130);
    private Camera uiCamera;
    private RectTransform canvasRect;
    private bool isHovering;

    private void Awake()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (this.popup == null){
            return;
        }

       
        if (this.canvasRect == null) {
            Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (canvas != null)
                canvasRect = canvas.GetComponent<RectTransform>();
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                uiCamera = canvas.worldCamera;
        }

        if (this._onPointerEnterHandle()) {
            this.popup.gameObject.SetActive(true);
        }

        this.isHovering = true;
        this.UpdatePopupPosition(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (this.isHovering && this.popup != null)
            this.UpdatePopupPosition(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.popup != null)
            this.popup.gameObject.SetActive(false);
        this.isHovering = false;
    }

    private void Update()
    {
        // 
        if (isHovering && popup != null)
        {
            //Vector2 screenPos = Input.mousePosition;
            //UpdatePopupPosition(screenPos);
        }
    }

    private void UpdatePopupPosition(PointerEventData eventData)
    {
        UpdatePopupPosition(eventData.position);
    }
    private void UpdatePopupPosition(Vector2 screenPos)
    {
        if (popup == null || canvasRect == null) return;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            uiCamera,
            out localPoint
        );

        localPoint += offset;
        if (localPoint.x < -canvasRect.rect.width / 2){
            localPoint.x += 50;
            offset.x *= -1;
        }
        if (localPoint.y > canvasRect.rect.height / 2) {
            localPoint.x -= 50;
            offset.y *= -1;
        }
        popup.anchoredPosition = localPoint;
    }

    protected virtual bool _onPointerEnterHandle() {
        return false;
    }
}