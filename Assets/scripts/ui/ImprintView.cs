using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImprintView : MonoBehaviour, IBaseView
{
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject content;
    public GameObject root;
    public GameObject cardPartPrefab;
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
        for (int i = 0; i < baseParts.Count; i++) {
            GameObject partGameObject = this.createCardPartObject();
            this.addDraggableToPoker(partGameObject, new Vector3(0, 0, 0));
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
        EventDispatcher.Instance.on(GameConst.IMPRINT_SELECT_PART, this.selectPartHandle);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
        EventDispatcher.Instance.off(GameConst.IMPRINT_SELECT_PART, this.selectPartHandle);
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

    public void selectPartHandle(params System.Object[] obj)
    {
        GameObject cardPart = (GameObject)obj[0];
        GameObject partGameObject = UnityEngine.Object.Instantiate(this.cardPartPrefab);
        partGameObject.transform.SetParent(this.root.transform, true);
        partGameObject.transform.position = cardPart.transform.position;
        partGameObject.GetComponent<CardPart>().setBtnEnable();
        this.addDraggableToPoker(partGameObject,new Vector3(0,0,0));
    }

    public void onExitClick()
    {
        GameReqMgr.Instance.requestExitPage();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
