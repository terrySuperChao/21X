using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class BarrierView : MonoBehaviour, IBaseView
{
    public GameObject money;
    public GameObject diamond;
    public GameObject hp;
    public GameObject magic;
    public GameObject attack;
    public GameObject defense;
    public GameObject pokerPrefab;
    public GameObject npcPokers;
    public GameObject playerPokers;
    public GameObject otherPokers;

    public GameObject sureBtn;
    public GameObject searchBtn;
    public GameObject stopPokerBtn;
    public GameObject dealPokerBtn;
    public GameObject reDealPokerBtn1;
    public GameObject reDealPokerBtn2;
    public GameObject matchPoint;
    public GameObject point;

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
        this.sureBtn.SetActive(false);
        this.searchBtn.SetActive(false);
        this.sureBtn.GetComponent<Button>().interactable = false;
        this.searchBtn.GetComponent<Button>().interactable = false;
        this.stopPokerBtn.GetComponent<Button>().interactable = false;
        this.dealPokerBtn.GetComponent<Button>().interactable = false;
        this.reDealPokerBtn1.GetComponent<Button>().interactable = false;
        this.reDealPokerBtn2.GetComponent<Button>().interactable = false;

        BarrierState state = BarrierDataMgr.Instance.getState();
        if (state == BarrierState.startPoker)
        {
            StartCoroutine(startDealPoker());
            return;
        }
      
        GameObject matchGameObjectA = null;
        GameObject matchGameObjectB = null;
        List<IPoker> npcList = BarrierDataMgr.Instance.getPokers(BarrierDealType.npc);
        for (int i = 0; i < npcList.Count; i++)
        {
            IPoker poker = npcList[i];
            Vector3 initPos = this.initPokerPos(i, this.npcPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.npcPokers, poker);
            pokerGameObject.transform.position = initPos;

            if (poker.getValue() == BarrierDataMgr.Instance.getMatchPointA())
            {
                matchGameObjectA = pokerGameObject;
            }
        }

        List<IPoker> playerList = BarrierDataMgr.Instance.getPokers(BarrierDealType.player);
        for (int i = 0; i < playerList.Count; i++)
        {
            IPoker poker = playerList[i];
            Vector3 initPos = this.initPokerPos(i, this.playerPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.playerPokers, poker);
            pokerGameObject.transform.position = initPos;

            if (BarrierDataMgr.Instance.getState() == BarrierState.matchPoker ||
                BarrierDataMgr.Instance.getState() == BarrierState.fillPoker)
            {
                this.addDraggableToPoker(pokerGameObject, initPos);
            }

            if (poker.getValue() == BarrierDataMgr.Instance.getMatchPointB())
            {
                matchGameObjectB = pokerGameObject;
            }
        }

        List<IPoker> otherList = BarrierDataMgr.Instance.getPokers(BarrierDealType.other);
        for (int i = 0; i < otherList.Count; i++)
        {
            IPoker poker = otherList[i];
            Vector3 initPos = this.initOtherPos(i, this.otherPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.otherPokers, poker);
            pokerGameObject.transform.position = initPos;
        }

        if (state == BarrierState.matchPoker)
        {
            if (matchGameObjectA != null && matchGameObjectB != null)
            {
                this.stopPokerBtn.GetComponent<Button>().interactable = true;
                this.dealPokerBtn.GetComponent<Button>().interactable = true;
            }
        }
        else if (state == BarrierState.dealPoker)
        {
            this.stopPokerBtn.GetComponent<Button>().interactable = true;
            this.dealPokerBtn.GetComponent<Button>().interactable = true;
        }
        else if (state == BarrierState.stopPoker)
        {
            this.sureBtn.SetActive(true);
            this.sureBtn.GetComponent<Button>().interactable = true;
        }
        else if (state == BarrierState.fillPoker) {
            StartCoroutine(startFillPoker());
        }

        if (matchGameObjectA != null && matchGameObjectB != null)
        {
            matchGameObjectB.transform.position = new Vector3(BarrierDataMgr.Instance.getPokerPosX(), BarrierDataMgr.Instance.getPokerPosY(), 0);
            this.matchPokerPoint(matchGameObjectA, matchGameObjectB);
        }
    }
    private IEnumerator startDealPoker()
    {
        yield return new WaitForSeconds(0.5f);

        int count = 3;
        for (int i = 0; i < count; i++)
        {
            IPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.npc);
            Vector3 initPos = this.initPokerPos(i, this.npcPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.npcPokers, poker);
            iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }

        for (int i = 0; i < count; i++)
        {
            IPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.player);
            Vector3 initPos = this.initPokerPos(i, this.playerPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.playerPokers, poker);
            iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);
            this.addDraggableToPoker(pokerGameObject, initPos);
            yield return new WaitForSeconds(0.3f);
        }

        BarrierDataMgr.Instance.setState(BarrierState.matchPoker);
        GamePropertyMgr.Instance.save();
    }

    private IEnumerator startFillPoker()
    {
        yield return new WaitForSeconds(0.5f);

        int count = 3;
        for (int i = 2; i < count; i++)
        {
            IPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.npc);
            Vector3 initPos = this.initPokerPos(i, this.npcPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.npcPokers, poker);
            iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }

        for (int i = 2; i < count; i++)
        {
            IPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.player);
            Vector3 initPos = this.initPokerPos(i, this.playerPokers);
            GameObject pokerGameObject = this.createPokerGameObject(i, this.playerPokers, poker);
            iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);
            this.addDraggableToPoker(pokerGameObject, initPos);
            yield return new WaitForSeconds(0.3f);
        }

        BarrierDataMgr.Instance.setState(BarrierState.matchPoker);
        GamePropertyMgr.Instance.save();
    }

    public GameObject createPokerGameObject(int index, GameObject parent, IPoker poker)
    {
        GameObject pokerGameObject = Object.Instantiate(this.pokerPrefab);
        pokerGameObject.GetComponent<Poker>().loadPokerRes(poker);
        pokerGameObject.transform.SetParent(parent.transform, true);
        pokerGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        pokerGameObject.name = "poker" + index;
        pokerGameObject.transform.position = this.pokerPrefab.transform.position;
        return pokerGameObject;

    }

    public Vector3 initPokerPos(int index, GameObject parent)
    {
        int count = 3;
        int space = 20;
        int width = 323;

        float x = index * (width + space) + width / 2 - ((count * width) + (count) * space) / 2;
        Vector3 initPos = parent.transform.TransformPoint(new Vector3(x, 0, 0));

        return initPos;
    }

    public Vector3 initOtherPos(int index, GameObject parent)
    {
        int space = 20;
        int width = 323;

        float x = index * space + width / 2;
        Vector3 initPos = parent.transform.TransformPoint(new Vector3(x, 0, 0));

        return initPos;
    }

    public void addDraggableToPoker(GameObject pokerGameObject, Vector3 initPos)
    {
        DraggableUI draggableUI = pokerGameObject.AddComponent<DraggableUI>();
        draggableUI.initPos(initPos);
        draggableUI.setCallBack((GameObject gameObject) =>
        {
            for (int j = 0; j < this.npcPokers.transform.childCount; j++)
            {
                GameObject npcPoker = this.npcPokers.transform.GetChild(j).gameObject;
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

                    BarrierDataMgr.Instance.setState(BarrierState.dealPoker);
                    BarrierDataMgr.Instance.setMatchPoker(matchPointA, matchPointB, pokerPosX, pokerPosY);
                    GamePropertyMgr.Instance.save();

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

                    return true;
                }
            }
            
            BarrierDataMgr.Instance.setState(BarrierState.matchPoker);
            BarrierDataMgr.Instance.setMatchPoker(0, 0,0, 0);
            GamePropertyMgr.Instance.save();

            this.matchPoint.SetActive(false);
            this.stopPokerBtn.GetComponent<Button>().interactable = false;
            this.dealPokerBtn.GetComponent<Button>().interactable = false;

            return false;
        });

    }

    private void matchPokerPoint(GameObject matchGameObjectA,GameObject matchGameObjectB) {
        Vector3 pos = matchGameObjectA.transform.position;
        float scaleX = matchGameObjectA.GetComponent<RectTransform>().localScale.x;
        float scaleY = matchGameObjectA.GetComponent<RectTransform>().localScale.y;
        float pokerWidth = matchGameObjectA.GetComponent<RectTransform>().rect.width;
        float pokerHeight = matchGameObjectA.GetComponent<RectTransform>().rect.height;

        this.matchPoint.SetActive(true);
        this.matchPoint.transform.position = matchGameObjectA.transform.TransformPoint(-pokerWidth / 2 - 30, pokerHeight / 2 + 30, pos.z);
        this.otherPokers.transform.position = matchGameObjectB.transform.TransformPoint(20, 0, 0);
        this.point.GetComponent<Text>().text = BarrierDataMgr.Instance.getMatchPoint().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onDealPokerClick()
    {
        IPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.other);
        
        Vector3 initPos = this.initOtherPos(this.otherPokers.transform.childCount, this.otherPokers);
        GameObject pokerGameObject = this.createPokerGameObject(0, this.otherPokers, poker);
        iTween.MoveTo(pokerGameObject.gameObject, initPos, 0.5f);

        //取消拖动
        for (int i = 0; i < this.playerPokers.transform.childCount; i++) {
            DraggableUI draggableUI = this.playerPokers.transform.GetChild(i).gameObject.GetComponent<DraggableUI>();
            if (draggableUI != null)
            {
                draggableUI.enabled = false;
            }
        }

        int point = BarrierDataMgr.Instance.getMatchPoint();
        if (point > 21)
        {
            BarrierDataMgr.Instance.setState(BarrierState.stopPoker);
            GamePropertyMgr.Instance.save();

            this.sureBtn.SetActive(true);
            this.sureBtn.GetComponent<Button>().interactable = true;
            this.stopPokerBtn.GetComponent<Button>().interactable = false;
            this.dealPokerBtn.GetComponent<Button>().interactable = false;
        }
        else {
            BarrierDataMgr.Instance.setState(BarrierState.dealPoker);
            GamePropertyMgr.Instance.save();
        }
        this.point.GetComponent<Text>().text = point.ToString();
    }

    public void onReDealPokerClick1()
    {

    }

    public void onReDealPokerClick2()
    {

    }

    public void onStopPokerClick()
    {
        BarrierDataMgr.Instance.setState(BarrierState.stopPoker);
        GamePropertyMgr.Instance.save();

        this.sureBtn.SetActive(true);
        this.sureBtn.GetComponent<Button>().interactable = true;
        this.stopPokerBtn.GetComponent<Button>().interactable = false;
        this.dealPokerBtn.GetComponent<Button>().interactable = false;
    }

    public void onCardClick()
    {

    }

    public void onSureClick()
    {
        string viewName = "";
        PageIndex pageIndex = 0;
        List<IPoker> npcList = BarrierDataMgr.Instance.getPokers(BarrierDealType.npc);
        for (int i = 0; i < npcList.Count; i++)
        {
            if (npcList[i].getValue() == BarrierDataMgr.Instance.getMatchPointA())
            {
                if (i == 0)
                {
                    viewName = "GameView";
                    pageIndex = PageIndex.GameView;
                }
                else if (i == 1)
                {
                    viewName = "RelaxView";
                    pageIndex = PageIndex.RelaxView;
                }
                else if (i == 2)
                { 

                }
            }
        }
        if (pageIndex == 0) return;

        BarrierDataMgr.Instance.clearMatch();
        BarrierDataMgr.Instance.setState(BarrierState.fillPoker);
        GameDataMgr.Instance.setPageIndex(pageIndex);
        GamePropertyMgr.Instance.save();
        UIMgr.Instance.showView(viewName);
    }

    public void onSearchClick()
    {

    }

    public void onPopClick()
    {
        UIMgr.Instance.showView("PopView");
    }
}
