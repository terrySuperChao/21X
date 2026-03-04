using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierView : MonoBehaviour, IBaseView
{
   
    public GameObject pokerPrefab;
    public GameObject npcPokers;
    public GameObject playerPokers;
    public GameObject otherPokers;

    public GameObject sureBtn;
    public GameObject searchBtn;
    public GameObject stopPokerBtn;
    public GameObject dealPokerBtn;
    public GameObject refreshNpcPokerBtn;
    public GameObject refreshPlayerPokerBtn;
    public GameObject matchPoint;
    public GameObject point;
    public GameObject bustProbability;
    public GameObject refreshNpcPokerNum;
    public GameObject refreshPlayerPokerNum;
    public GameObject title;
    public GameObject desc;
    public GameObject chapterItem;

    public void init()
    {
        GameMessage.Instance.clearCacheMessage();
    }

    public void beforeShow()
    {

    }

    public void refresh()
    {

    }

    public void afterShow()
    {
        this.sureBtn.SetActive(false);
        this.searchBtn.SetActive(false);
        this.sureBtn.GetComponent<Button>().interactable = false;
        this.searchBtn.GetComponent<Button>().interactable = false;
        this.stopPokerBtn.GetComponent<Button>().interactable = false;
        this.dealPokerBtn.GetComponent<Button>().interactable = false;

        this.showChapterInfo();
        this.showRefreshNpcPokerNum();
        this.showRefreshPlayerPokerNum();

        this.createPokerList(this.npcPokers, BarrierDealType.npc);
        this.createPokerList(this.playerPokers, BarrierDealType.player);
        this.createPokerList(this.otherPokers, BarrierDealType.other);

        BarrierState state = BarrierDataMgr.Instance.getState();
        switch (state) {
            case BarrierState.startPoker:
                GameReqMgr.Instance.requestNewBarrier();
                GameMessage.Instance.setHandleMessageComplete();
                break;
            case BarrierState.dragPoker:
                this.addPlayerPokerDrag();
                break;
            case BarrierState.matchPoker:
                this.addPlayerPokerDrag();
                this.stopPokerBtn.GetComponent<Button>().interactable = true;
                this.dealPokerBtn.GetComponent<Button>().interactable = true;
                break;
            case BarrierState.dealPoker:
                this.stopPokerBtn.GetComponent<Button>().interactable = true;
                this.dealPokerBtn.GetComponent<Button>().interactable = true;
                break;
            case BarrierState.stopPoker:
                this.sureBtn.SetActive(true);
                this.sureBtn.GetComponent<Button>().interactable = true;
                break;
            case BarrierState.fillPoker:
                this.addPlayerPokerDrag();
                GameReqMgr.Instance.requestFillPoker();
                GameMessage.Instance.setHandleMessageComplete();
                break;
        }

        if (BarrierDataMgr.Instance.getMatchPointA() > 0 &&
            BarrierDataMgr.Instance.getMatchPointB() > 0)
        {
            GameObject matchGameObjectA = this.findPokerGameObject(this.npcPokers, BarrierDataMgr.Instance.getMatchPointA());
            GameObject matchGameObjectB = this.findPokerGameObject(this.playerPokers, BarrierDataMgr.Instance.getMatchPointB());
            matchGameObjectB.transform.position = new Vector3(BarrierDataMgr.Instance.getPokerPosX(), BarrierDataMgr.Instance.getPokerPosY(), 0);
            this.matchPokerPoint(matchGameObjectA, matchGameObjectB);
            this.showBustProbability();
        }
    }

    public void createPokerList(GameObject parent, BarrierDealType type) {
        List<IPoker> pokers = BarrierDataMgr.Instance.getPokers(type);
        for (int i = 0; i < pokers.Count; i++)
        {
            IPoker poker = pokers[i];
            if (poker.getValue() != 0) {
                Vector3 initPos = this.initPokerPos(i, parent, type);
                GameObject pokerGameObject = this.createPokerGameObject(i, parent, poker);
                pokerGameObject.transform.position = initPos;
            }
        }
    }

    public GameObject findPokerGameObject(GameObject parent, int value) {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject gameObject = parent.transform.GetChild(i).gameObject;
            Poker poker = gameObject.GetComponent<Poker>();
            if (poker != null && poker.getPoker().getValue() == value)
            {
                return gameObject;
            }
        }
        return null;
    }

    public GameObject createPokerGameObject(int index, GameObject parent, IPoker poker)
    {
        GameObject pokerGameObject = UnityEngine.Object.Instantiate(this.pokerPrefab);
        pokerGameObject.GetComponent<Poker>().loadPokerRes(poker);
        pokerGameObject.transform.SetParent(parent.transform, true);
        pokerGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        pokerGameObject.name = "poker" + index;
        pokerGameObject.transform.position = this.pokerPrefab.transform.position;
        return pokerGameObject;

    }

    public Vector3 initPokerPos(int index, GameObject parent, BarrierDealType type)
    {
        int count = 3;
        int space = 20;
        int width = 323;
        float x = 0;
        if (type == BarrierDealType.other)
        {
            x = index * space * 3 + width / 2;
        }
        else {
            x = index * (width + space) + width / 2 - ((count * width) + (count) * space) / 2;
        }
        return parent.transform.TransformPoint(new Vector3(x, 0, 0)); ;
    }

    public void addPlayerPokerDrag() {
        for (int i = 0; i < this.playerPokers.transform.childCount; i++)
        {
            GameObject gameObject = this.playerPokers.transform.GetChild(i).gameObject;
            Vector3 initPos = this.initPokerPos(i, this.playerPokers, BarrierDealType.player);
            this.addDraggableToPoker(gameObject, initPos);
        }
    }

    public void deletePlayerPokerDrag() {
        for (int i = 0; i < this.playerPokers.transform.childCount; i++)
        {
            DraggableUI draggableUI = this.playerPokers.transform.GetChild(i).gameObject.GetComponent<DraggableUI>();
            if (draggableUI != null)
            {
                draggableUI.enabled = false;
            }
        }
    }

    public void addDraggableToPoker(GameObject pokerGameObject, Vector3 initPos)
    {
        DraggableUI draggableUI = pokerGameObject.AddComponent<DraggableUI>();
        draggableUI.initPos(initPos);
        draggableUI.setCallBack((GameObject gameObject) =>
        {
            for (int i = 0; i < this.npcPokers.transform.childCount; i++)
            {
                GameObject npcPoker = this.npcPokers.transform.GetChild(i).gameObject;
                Vector3 pointLocal1 = this.npcPokers.transform.InverseTransformPoint(gameObject.transform.position);
                Vector3 pointLocal2 = this.npcPokers.transform.InverseTransformPoint(npcPoker.transform.position);
                float scaleX = npcPoker.GetComponent<RectTransform>().localScale.x;
                float scaleY = npcPoker.GetComponent<RectTransform>().localScale.y;
                float pokerWidth = npcPoker.GetComponent<RectTransform>().rect.width;
                float pokerHeight = npcPoker.GetComponent<RectTransform>().rect.height;
                if (pointLocal2.x - pokerWidth * scaleX / 2 < pointLocal1.x &&
                    pointLocal2.x + pokerWidth * scaleX / 2 > pointLocal1.x &&
                    pointLocal2.y - pokerHeight * scaleY / 2 < pointLocal1.y &&
                    pointLocal2.y + pokerHeight * scaleY / 2 > pointLocal1.y)
                {
                    int matchPointA = npcPoker.GetComponent<Poker>().getPoker().getValue();
                    int matchPointB = gameObject.GetComponent<Poker>().getPoker().getValue();
                    int pokerPosX = (int)gameObject.transform.position.x;
                    int pokerPosY = (int)gameObject.transform.position.y;
                    GameReqMgr.Instance.requestMatchPoker(matchPointA, matchPointB, pokerPosX, pokerPosY);

                    //重置坐标
                    for (int z = 0; z < this.playerPokers.transform.childCount; z++)
                    {
                        if (this.playerPokers.transform.GetChild(z).gameObject.name != gameObject.name)
                        {

                            this.playerPokers.transform.GetChild(z).GetComponent<DraggableUI>().resetInitPos();
                        }
                    }

                    this.stopPokerBtn.GetComponent<Button>().interactable = true;
                    this.dealPokerBtn.GetComponent<Button>().interactable = true;
                    this.matchPokerPoint(npcPoker, gameObject);
                    this.showBustProbability();

                    return true;
                }
            }

            if (BarrierDataMgr.Instance.getMatchPointB() == gameObject.GetComponent<Poker>().getPoker().getValue()) {
                this.matchPoint.SetActive(false);
                this.stopPokerBtn.GetComponent<Button>().interactable = false;
                this.dealPokerBtn.GetComponent<Button>().interactable = false;
                GameReqMgr.Instance.requestUnMatchPoker();
            }
            return false;
        });

    }

    private void matchPokerPoint(GameObject matchGameObjectA, GameObject matchGameObjectB)
    {
        Vector3 pos = matchGameObjectA.transform.position;
        float scaleX = matchGameObjectA.GetComponent<RectTransform>().localScale.x;
        float scaleY = matchGameObjectA.GetComponent<RectTransform>().localScale.y;
        float pokerWidth = matchGameObjectA.GetComponent<RectTransform>().rect.width;
        float pokerHeight = matchGameObjectA.GetComponent<RectTransform>().rect.height;

        this.matchPoint.SetActive(true);
        this.matchPoint.transform.position = matchGameObjectA.transform.TransformPoint(-pokerWidth / 2 - 30, pokerHeight / 2 + 30, pos.z);
        this.otherPokers.transform.position = matchGameObjectB.transform.TransformPoint(-pokerWidth / 2, 0, 0);
        this.point.GetComponent<Text>().text = BarrierDataMgr.Instance.getMatchPoint().ToString();
    }

    private void showBustProbability() {
        float number = BarrierDataMgr.Instance.getBustProbability();
        this.bustProbability.GetComponent<Text>().text = number + "%";
        this.bustProbability.SetActive(true);
        if (number <= 30)
        {
            this.bustProbability.GetComponent<Text>().color = Color.green;
        }
        else if (number <= 60)
        {
            this.bustProbability.GetComponent<Text>().color = Color.blue;
        }
        else if (number <= 90)
        {
            this.bustProbability.GetComponent<Text>().color = Color.red;
        }
        else
        {
            this.bustProbability.GetComponent<Text>().color = Color.red;
        }
    }

    private void showChapterInfo() {
        Chapter chapter = GameStaticConfigMgr.Instance.getChapterConfig().getChapter(BarrierDataMgr.Instance.getChapterId());
        if (chapter == null) return;

        GameObject parent = this.chapterItem.transform.parent.gameObject;
        for (int i = 0; i < chapter.childTotal-1; i++) {
            GameObject item = UnityEngine.Object.Instantiate(this.chapterItem);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            item.transform.SetParent(parent.transform, false);
        }

        for (int i = 0; i < parent.transform.childCount; i++) {
            if (i + 1 <= BarrierDataMgr.Instance.getBarrierId()) {
                parent.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
        }

        this.title.GetComponent<Text>().text = chapter.title;
        this.desc.GetComponent<Text>().text = chapter.bossDesc;
    }

    private void showRefreshNpcPokerNum() {
        int number = BarrierDataMgr.Instance.getRefreshNpcPokerNum();
        this.refreshNpcPokerNum.GetComponent<Text>().text = number.ToString();
        this.refreshNpcPokerBtn.GetComponent<Button>().interactable = number > 0;
    }

    private void showRefreshPlayerPokerNum()
    {
        int number = BarrierDataMgr.Instance.getRefreshPlayerPokerNum();
        this.refreshPlayerPokerNum.GetComponent<Text>().text = number.ToString();
        this.refreshPlayerPokerBtn.GetComponent<Button>().interactable = number > 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.BARRIERVIEW_NEWPOKER, this.newPokerHandle);
        EventDispatcher.Instance.on(GameConst.BARRIERVIEW_SUREPOKER, this.surePokerHandle);
    }

    private void OnDestroy(){
        EventDispatcher.Instance.off(GameConst.BARRIERVIEW_NEWPOKER, this.newPokerHandle);
        EventDispatcher.Instance.off(GameConst.BARRIERVIEW_SUREPOKER, this.surePokerHandle);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void newPokerHandle(params System.Object[] obj) {
        BarrierDealPoker poker = (BarrierDealPoker)obj[0];
        if (poker.type == BarrierDealType.npc)
        {
            StartCoroutine(newPoker(poker, this.npcPokers,false));
        }
        else if (poker.type == BarrierDealType.player)
        {
            StartCoroutine(newPoker(poker,this.playerPokers,true));
        }
        else if (poker.type == BarrierDealType.other) {
            StartCoroutine(newPoker(poker,this.otherPokers,false));
            newPokerAfter();
        }
    }


    private IEnumerator newPoker(BarrierDealPoker poker, GameObject parent,bool addDrag)
    {
        Vector3 initPos = this.initPokerPos(poker.index, parent, poker.type);
        GameObject pokerGameObject = this.createPokerGameObject(poker.index, parent, poker.poker);
        iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);
        yield return new WaitForSeconds(0.3f);

        if (addDrag) {
            this.addDraggableToPoker(pokerGameObject, initPos);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void newPokerAfter() {
        //取消拖动
        this.deletePlayerPokerDrag();

        BarrierState state = BarrierDataMgr.Instance.getState();
        if (state == BarrierState.stopPoker){
            this.bustProbability.SetActive(false);
            this.sureBtn.SetActive(true);
            this.sureBtn.GetComponent<Button>().interactable = true;
            this.stopPokerBtn.GetComponent<Button>().interactable = false;
            this.dealPokerBtn.GetComponent<Button>().interactable = false;
        }
        else
        {
            this.showBustProbability();
        }
        this.point.GetComponent<Text>().text = BarrierDataMgr.Instance.getMatchPoint().ToString();
    }

    public void surePokerHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }
        

    public void onDealPokerClick()
    {
        GameReqMgr.Instance.requestDealPoker();
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void refreshPokerType(BarrierDealType type, GameObject parent,int value) {
        for (int i = parent.transform.childCount - 1; i > -1; i--){
            GameObject child = parent.transform.GetChild(i).gameObject;
            Poker poker = child.GetComponent<Poker>();
            if (poker != null && poker.getPoker().getValue() != value)
            {
                Destroy(child);
            }
        }
        GameReqMgr.Instance.requestRefreshPoker(type);
        GameMessage.Instance.setHandleMessageComplete();
    }
    public void onRefreshNpcPokerClick()
    {
        this.refreshPokerType(BarrierDealType.npc, this.npcPokers, BarrierDataMgr.Instance.getMatchPointA());
        this.showRefreshNpcPokerNum();
    }

    public void onRefreshPlayerPokerClick()
    {
        this.refreshPokerType(BarrierDealType.player, this.playerPokers, BarrierDataMgr.Instance.getMatchPointB());
        this.showRefreshPlayerPokerNum();
    }

    public void onStopPokerClick()
    {
        this.deletePlayerPokerDrag();

        this.sureBtn.SetActive(true);
        this.sureBtn.GetComponent<Button>().interactable = true;
        this.stopPokerBtn.GetComponent<Button>().interactable = false;
        this.dealPokerBtn.GetComponent<Button>().interactable = false;
        GameReqMgr.Instance.requestStopPoker();
    }

    public void onCardClick()
    {

    }

    public void onSureClick()
    {
        GameReqMgr.Instance.requestSurePoker();
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onSearchClick()
    {

    }

    public void onPopClick()
    {
        UIMgr.Instance.showView("PopView");
    }
}
