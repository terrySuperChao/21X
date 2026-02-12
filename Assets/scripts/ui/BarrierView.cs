using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierView : MonoBehaviour,IBaseView
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

    public GameObject sureBtn;
    public GameObject searchBtn;
    public GameObject stopPokerBtn;
    public GameObject dealPokerBtn;
    public GameObject reDealPokerBtn1;
    public GameObject reDealPokerBtn2;
    
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
        this.sureBtn.GetComponent<Button>().interactable = false;
        this.searchBtn.GetComponent<Button>().interactable = false;
        this.stopPokerBtn.GetComponent<Button>().interactable = false;
        this.dealPokerBtn.GetComponent<Button>().interactable = false;
        this.reDealPokerBtn1.GetComponent<Button>().interactable = false;
        this.reDealPokerBtn2.GetComponent<Button>().interactable = false;

        List<IPoker> playerList = BarrierDataMgr.Instance.getPlayerPokers();
        foreach (IPoker player in playerList) {
            GameObject pokerGameObject =  Object.Instantiate(this.pokerPrefab);
            pokerGameObject.transform.SetParent(this.playerPokers.transform,false);
        }

        List<IPoker> npcList = BarrierDataMgr.Instance.getNpcPokers();
        foreach (IPoker player in playerList)
        {
            GameObject pokerGameObject = Object.Instantiate(this.pokerPrefab);
            pokerGameObject.transform.SetParent(this.npcPokers.transform, false);
        }

        if (BarrierDataMgr.Instance.getState() == BarrierState.idle) {
            StartCoroutine(startDealPoker());
        }
    }
    private IEnumerator startDealPoker() {
        yield return new WaitForSeconds(0.5f);

        int count = 3;
        int space = 20;
        int width = 323;        
        for (int i = 0; i < count; i++) {
            this.createPoker(i, this.npcPokers);
            yield return new WaitForSeconds(0.3f);
        }

        for (int i = 0; i < count; i++)
        {
            this.createPoker(i, this.playerPokers);
            yield return new WaitForSeconds(0.3f);
        }

        for (int i = 0; i < this.playerPokers.transform.childCount; i++) {
            DraggableUI draggableUI = this.playerPokers.transform.GetChild(i).gameObject.AddComponent<DraggableUI>();
            draggableUI.setCallBack((GameObject gameObject) =>{
                Vector3 pointLocal = this.npcPokers.transform.InverseTransformPoint(gameObject.transform.position);
                for (int i = 0; i < this.npcPokers.transform.childCount; i++) {
                    GameObject npcPoker = this.npcPokers.transform.GetChild(i).gameObject;
                    float pokerWidth = npcPoker.GetComponent<RectTransform>().rect.width;
                    float pokerHeight = npcPoker.GetComponent<RectTransform>().rect.height;
                    if (npcPoker.transform.position.x - pokerWidth / 2 < pointLocal.x &&
                        npcPoker.transform.position.x + pokerWidth / 2 > pointLocal.x &&
                        npcPoker.transform.position.y - pokerHeight / 2 < pointLocal.y &&
                        npcPoker.transform.position.y + pokerHeight / 2 > pointLocal.y) {

                        int matchPointA = npcPoker.GetComponent<Poker>().getPoker().getValue();
                        int matchPointB = gameObject.GetComponent<Poker>().getPoker().getValue();
                        int offsetX = (int)(npcPoker.transform.position.x - pointLocal.x);
                        int offsetY = (int)(npcPoker.transform.position.y - pointLocal.y);
                        BarrierDataMgr.Instance.setMatchPoker(matchPointA, matchPointB, offsetX, offsetY);

                        //重置坐标
                        for (int j = 0; j < count; j++)
                        {
                            if (this.playerPokers.transform.GetChild(i) != gameObject) {
                                float x = i * (width + space) + width / 2 - ((count * width) + (count - 1) * space);
                                this.playerPokers.transform.GetChild(i).position = new Vector3(x,0,0);
                            }
                        }
                        return true;
                    }
                }
                return false;
            });
        }
    }

    public void createPoker(int index,GameObject parent) {
        int count = 3;
        int space = 20;
        int width = 323;

        float x = index * (width + space) + width / 2 - ((count * width) + (count - 1) * space);
        IPoker poker = BarrierDataMgr.Instance.dealNpcPoker();
        GameObject pokerGameObject = Object.Instantiate(this.pokerPrefab);
        pokerGameObject.GetComponent<Poker>().loadPokerRes(poker);
        pokerGameObject.transform.SetParent(parent.transform, true);
        pokerGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        Vector3 pointLocal = parent.transform.TransformPoint(new Vector3(x, 0, 0));
        iTween.MoveTo(pokerGameObject.gameObject, pointLocal, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onDealPokerClick() {
        
    }

    public void onReDealPokerClick1() { 

    }

    public void onReDealPokerClick2() { 

    }

    public void onStopPokerClick() {
        
    }

    public void onCardClick()
    {
        
    }

    public void onSureClick() { 

    }

    public void onSearchClick() { 

    }

    public void onPopClick() {
        UIMgr.Instance.showView("PopView");
    }
}
