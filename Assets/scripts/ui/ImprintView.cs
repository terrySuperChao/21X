using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImprintView : MonoBehaviour, IBaseView
{
    public RectTransform drayLayer;
    public RectTransform baseTargetArea1;
    public RectTransform baseTargetArea2;
    public RectTransform baseTargetArea3;
    public RectTransform triggerTargetArea1;
    public RectTransform triggerTargetArea2;
    public RectTransform triggerTargetArea3;
    public ScrollRect scrollRect;
    public GameObject content;
    public GameObject cardPartPrefab;
    private CardPartController cardPartController = new CardPartController();
    private List<LongPressCloneSource> longPressItems = new List<LongPressCloneSource>();
    public void init()
    {
        this.cardPartController.setDragCallBack(this.clearSelectPart);
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

        foreach (BasePartInfo part in baseParts) {
            if (part.getPartIds().Count > 0)
            {
                parts.Add(part);
            }
        }

        foreach (TriggerPartInfo part in triggerParts)
        {
             parts.Add(part);
        }

        
        for (int i = 0; i < parts.Count; i++) {
            GameObject partGameObject = this.createCardPartObject();
            CardPart cardPart = partGameObject.GetComponent<CardPart>();
            cardPart.loadPartImage(parts[i]);

            LongPressCloneSource longPressItem = partGameObject.AddComponent<LongPressCloneSource>();
            longPressItem.dragLayer = this.drayLayer;
            longPressItem.cardPartController = this.cardPartController;
            longPressItem.scrollRect = this.scrollRect;
            longPressItem.partInfo = parts[i];
            this.longPressItems.Add(longPressItem);
        }

        List<RectTransform> baseTargetAreaList = new List<RectTransform>{ this.baseTargetArea1,this.baseTargetArea2,this.baseTargetArea3 };
        List<RectTransform> triggerTargetAreList = new List<RectTransform> { this.triggerTargetArea1, this.triggerTargetArea2, this.triggerTargetArea3 };

        List<IAssembleCard> assembleCard = ImprintDataMgr.Instance.getAssembleCard();
        for (int i = 0; i < assembleCard.Count; i++)
        {
            //基础类
            BasePartInfo baseInfo = GameStaticConfigMgr.Instance.getBasePartConfig().getBasePartId(assembleCard[i].getBaseDataId());
            RectTransform baseTargetArea = baseTargetAreaList[i];
            if (baseTargetArea != null) { 
                this.cardPartController.addCardPartItem(i, baseInfo, baseTargetArea, TargetPart.basePart);
                if (baseInfo != null) {
                    this.createSelectCardPart(baseTargetArea, baseInfo);
                    this.seLongPressItemState(baseInfo,false);
                }
            }

            //触发类
            TriggerPartInfo triggerInfo = GameStaticConfigMgr.Instance.getTriggerPartConfig().getTriggerPartId(assembleCard[i].getTriggerId());
            RectTransform triggerTargetArea = triggerTargetAreList[i];
            if (triggerTargetArea != null){
                this.cardPartController.addCardPartItem(i, triggerInfo, triggerTargetArea, TargetPart.triggerPart);
                if (triggerInfo != null)
                {
                    this.createSelectCardPart(triggerTargetArea, triggerInfo);
                    this.seLongPressItemState(triggerInfo, false);
                }
            }
        }

    }

    private GameObject createCardPartObject()
    {
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(this.content.transform, false);
        return partGameObject;
    }

    private void createSelectCardPart(Transform targetArea,IPart partInfo) {
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(targetArea, false);
        partGameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        CardPart cardPart = partGameObject.GetComponent<CardPart>();
        cardPart.loadPartImage(partInfo);
    }

    //设置列表已经被选中的item不可操作
    private void seLongPressItemState(IPart partInfo, bool disable) {
        for (int i = 0; i < this.longPressItems.Count; i++) {
            if (this.longPressItems[i].partInfo == partInfo) {
                this.longPressItems[i].setEnable(disable);
                break;
            }
        }
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
        this.clearSelectPart(index, TargetPart.basePart);
    }

    public void onTriggerPartClick(int index) {
        this.clearSelectPart(index, TargetPart.triggerPart);
    }

    private void clearSelectPart(int index, TargetPart targetType) {
        Transform transform = this.cardPartController.getTargetArea(index, targetType);
        if (transform != null)
        {
            for (int j = transform.childCount - 1; j >= 0; j--)
            {
                Destroy(transform.GetChild(j).gameObject);
            }
            IPart partInfo = this.cardPartController.getTargetAreaPart(index, targetType);
            if (partInfo != null) {
                this.seLongPressItemState(partInfo,true);
                this.cardPartController.clearTargetAreaPart(partInfo);
            }
        }
    }
}
