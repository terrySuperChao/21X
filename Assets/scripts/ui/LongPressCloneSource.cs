using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressCloneSource : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("长按时长")]
    public float longPressTime = 0.5f;

    [Header("克隆体拖拽层")]
    public RectTransform dragLayer;

    [Header("如果在 ScrollRect 中，拖拽时禁用它")]
    public ScrollRect scrollRect;

    public IPart partInfo;
    public CardPartController cardPartController;

    private bool isPointerDown;
    private bool hasCloned;
    private bool hasEnable = true;
    private Coroutine pressCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasEnable) return;

        isPointerDown = true;
        hasCloned = false;

        if (pressCoroutine != null)
            StopCoroutine(pressCoroutine);

        pressCoroutine = StartCoroutine(CheckLongPress(eventData));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!hasEnable) return;

        isPointerDown = false;
        if (pressCoroutine != null)
        {
            StopCoroutine(pressCoroutine);
            pressCoroutine = null;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        if (!hasEnable) return;
        
        // 如果你希望移出 item 后取消长按，就保留
        // 如果你希望按住后移出也能继续触发，可删除这个方法内容
        if (!hasCloned)
        {
            isPointerDown = false;

            if (pressCoroutine != null)
            {
                StopCoroutine(pressCoroutine);
                pressCoroutine = null;
            }
        }
    }

    private IEnumerator CheckLongPress(PointerEventData eventData)
    {
        float timer = 0f;

        while (timer < longPressTime)
        {
            if (!isPointerDown)
                yield break;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        hasCloned = true;
        CreateClone(eventData);
    }

    private void CreateClone(PointerEventData eventData)
    {
        if (dragLayer == null)
        {
            Debug.LogError("dragLayer 没设置");
            return;
        }

        // 生成克隆体
        GameObject clone = Instantiate(gameObject, dragLayer);
        clone.name = gameObject.name + "_Clone";

        // 删除源对象脚本，防止克隆体再次触发长按
        LongPressCloneSource source = clone.GetComponent<LongPressCloneSource>();
        if (source != null)
            Destroy(source);

        // 如果原 item 有 Button，避免克隆体点击触发
        Button btn = clone.GetComponent<Button>();
        if (btn != null)
            btn.enabled = false;

        // 禁用 ScrollRect，防止拖动时列表滚动
        if (this.scrollRect != null)
            this.scrollRect.enabled = false;

        // 添加拖拽脚本
        CloneDragController drag = clone.GetComponent<CloneDragController>();
        if (drag == null)
            drag = clone.AddComponent<CloneDragController>();

        drag.BeginDrag(dragLayer, cardPartController, eventData.pressEventCamera, partInfo,this.onDragCallBack);

        // 关键：长按成功后，原对象不再继续这次按压逻辑
        isPointerDown = false;
    }

    public void setEnable(bool hasEnable) {
        this.hasEnable = hasEnable;
    }

    private void onDragCallBack(bool hasEnable) {
        this.hasEnable = hasEnable;
        
        // 禁用 ScrollRect，防止拖动时列表滚动
        if (this.scrollRect != null)
            this.scrollRect.enabled = true;
    }
}