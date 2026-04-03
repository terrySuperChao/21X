using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class DraggableUI : MonoBehaviour,IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _offset;
    private Vector3 _initPos;
    private Func<GameObject, bool> _callBack;
    private RectTransform _rectTransform;

    public void initPos(Vector3 initPos)
    {
        this._rectTransform = gameObject.GetComponent<RectTransform>();
        this._initPos = initPos;
    }

    public void resetInitPos()
    {
        this._rectTransform.position = this._initPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("XXXXX");
        RectTransformUtility.ScreenPointToWorldPointInRectangle(this._rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos);
        this._offset = this._rectTransform.position - mousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("XXXXXXXXXXXXXXXXX");
        if (eventData.pointerPressRaycast.gameObject == gameObject) {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this._rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 mousePos)){
                this._rectTransform.position = mousePos + this._offset;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("XXXXXXXXXXXXXXXXX11111111111111");
        if (this._callBack != null)
        {
            if (this._callBack(this.gameObject)){

            }
            else {
                this.resetInitPos();
            }
        }
        else {
            this.resetInitPos();
        }
    }

    public void setCallBack(Func<GameObject,bool> callBack) {
        this._callBack = callBack;
    }

    

}