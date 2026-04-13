using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CloneDragController : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private RectTransform dragLayer;
    private Camera uiCamera;
    private ScrollRect scrollRect;
    private IPart partInfo;
    private CardPartController cardPartController;

    private bool isDragging;

    public void BeginDrag(RectTransform dragLayer, CardPartController cardPartController, Camera uiCamera, ScrollRect scrollRect, IPart partInfo)
    {
        this.dragLayer = dragLayer;
        this.cardPartController = cardPartController;
        this.uiCamera = uiCamera;
        this.scrollRect = scrollRect;
        this.partInfo = partInfo;

        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 不阻挡射线，防止遮住目标区域
        canvasGroup.blocksRaycasts = false;

        // 禁用 ScrollRect，防止拖动时列表滚动
        if (this.scrollRect != null)
            this.scrollRect.enabled = false;

        isDragging = true;

        // 立刻更新一次位置
        UpdatePosition(GetPointerPosition());
    }

    private void Update()
    {
        if (!isDragging) return;

        Vector2 pointerPos = GetPointerPosition();
        UpdatePosition(pointerPos);

        if (IsPointerReleased())
        {
            EndDrag(pointerPos);
        }
    }

    private Vector2 GetPointerPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        return Input.mousePosition;
#else
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        return Vector2.zero;
#endif
    }

    private bool IsPointerReleased()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        return Input.GetMouseButtonUp(0);
#else
        if (Input.touchCount == 0) return true;

        Touch touch = Input.GetTouch(0);
        return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
#endif
    }

    private void UpdatePosition(Vector2 screenPos)
    {
        if (dragLayer == null || rectTransform == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragLayer,
            screenPos,
            uiCamera,
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    private void EndDrag(Vector2 screenPos)
    {
        isDragging = false;

        if (scrollRect != null)
            scrollRect.enabled = true;

        bool success = false;
        RectTransform targetArea = null;
        List<RectTransform> targetAreas = cardPartController.getTargetAreas(partInfo.getTargetPart());
        for (int i = 0; i < targetAreas.Count; i++) {
            success = RectTransformUtility.RectangleContainsScreenPoint(
                targetAreas[i],
                screenPos,
                uiCamera
            );
            if (success) {
                targetArea = targetAreas[i];
                break;
            }
        }

        if (success)
        {
            cardPartController.matchTargetArea(targetArea,partInfo);
            transform.SetParent(targetArea, false);

            // 你可以改成吸附到某个格子，这里先简单放中间
            rectTransform.anchoredPosition = Vector2.zero;

            canvasGroup.blocksRaycasts = true;
            Debug.Log("放置成功");
        }
        else
        {
            Debug.Log("放置失败，销毁 clone");
            Destroy(gameObject);
        }
    }
}