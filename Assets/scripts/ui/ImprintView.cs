using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImprintView : MonoBehaviour, IBaseView
{
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public RectTransform drayLayer;
    public RectTransform baseTargetArea1;
    public RectTransform baseTargetArea2;
    public RectTransform baseTargetArea3;
    public RectTransform triggerTargetArea1;
    public RectTransform triggerTargetArea2;
    public RectTransform triggerTargetArea3;
    public ScrollRect scrollRect;
    public GameObject content;
    public GameObject root;
    public GameObject cardPartPrefab;
    private CardPartController cardPartController = new CardPartController();
    public void init()
    {

    }

    public void beforeShow()
    {

    }

    public void refresh()
    {

    }

    public void afterShow()
    {
        this.cardPartController.addCardPartItem(null,this.baseTargetArea1,TargetPart.basePart);
        this.cardPartController.addCardPartItem(null, this.baseTargetArea1, TargetPart.basePart);
        this.cardPartController.addCardPartItem(null, this.baseTargetArea1, TargetPart.basePart);

        this.cardPartController.addCardPartItem(null, this.triggerTargetArea1, TargetPart.triggerPart);
        this.cardPartController.addCardPartItem(null, this.triggerTargetArea2, TargetPart.triggerPart);
        this.cardPartController.addCardPartItem(null, this.triggerTargetArea3, TargetPart.triggerPart);

        List<BasePartInfo> baseParts = GameStaticConfigMgr.Instance.getBasePartConfig().getBasePart();
        List<TriggerPartInfo> triggerParts = GameStaticConfigMgr.Instance.getTriggerPartConfig().getTriggerPart();
        List<IPart> parts = new List<IPart>();
        parts.AddRange(baseParts);
        parts.AddRange(triggerParts);
        
        for (int i = 0; i < parts.Count; i++) {
            GameObject partGameObject = this.createCardPartObject();
            CardPart cardPart = partGameObject.GetComponent<CardPart>();
            cardPart.loadPartImage(parts[i]);

            LongPressCloneSource longPressItem = partGameObject.AddComponent<LongPressCloneSource>();
            longPressItem.dragLayer = this.drayLayer;
            longPressItem.cardPartController = this.cardPartController;
            longPressItem.scrollRect = this.scrollRect;
            longPressItem.partInfo = parts[i];
        }
    }

    public GameObject createCardPartObject()
    {
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(this.content.transform, false);
        return partGameObject;
    }

    public void addDraggableToPoker(GameObject partGameObject, Vector3 initPos)
    {
        DraggableUI draggableUI = partGameObject.AddComponent<DraggableUI>();
        draggableUI.initPos(initPos);
        draggableUI.setCallBack((GameObject gameObject) =>
        {
            return false;
        });

    }

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exitPageHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    public void onExitClick()
    {
        GameReqMgr.Instance.requestExitPage();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
