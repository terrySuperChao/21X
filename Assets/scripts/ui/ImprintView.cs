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

        List<RectTransform> baseTargetAreaList = new List<RectTransform>{ this.baseTargetArea1,this.baseTargetArea2,this.baseTargetArea3 };
        List<RectTransform> triggerTargetAreList = new List<RectTransform> { this.triggerTargetArea1, this.triggerTargetArea2, this.triggerTargetArea3 };

        List<IAssembleCard> assembleCard = ImprintDataMgr.Instance.getAssembleCard();
        for (int i = 0; i < assembleCard.Count; i++)
        {
            //基础类
            BasePartInfo baseInfo = GameStaticConfigMgr.Instance.getBasePartConfig().getBasePartId(assembleCard[i].getBaseDataId());
            RectTransform baseTargetArea = baseTargetAreaList[i];
            this.cardPartController.addCardPartItem(i, baseInfo, baseTargetArea, TargetPart.basePart);
            if (baseInfo != null) {
                this.createSelectCardPart(baseTargetArea, baseInfo);
            }

            //触发类
            TriggerPartInfo triggerInfo = GameStaticConfigMgr.Instance.getTriggerPartConfig().getTriggerPartId(assembleCard[i].getTriggerId());
            RectTransform triggerTargetArea = triggerTargetAreList[i];
            this.cardPartController.addCardPartItem(i, triggerInfo, triggerTargetArea, TargetPart.triggerPart);
            if (triggerInfo != null)
            {
                this.createSelectCardPart(triggerTargetArea, triggerInfo);
            }
        }

    }

    public GameObject createCardPartObject()
    {
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(this.content.transform, false);
        return partGameObject;
    }

    public void createSelectCardPart(Transform targetArea,IPart partInfo) {
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(targetArea, false);
        partGameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        CardPart cardPart = partGameObject.GetComponent<CardPart>();
        cardPart.loadPartImage(partInfo);
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

    public void onBasePartClick(int index) {
        this.cardPartController.deleteTargetArea(this,index, TargetPart.basePart);
    }

    public void onTriggerPartClick(int index) {
        this.cardPartController.deleteTargetArea(this,index, TargetPart.triggerPart);
    }
}
