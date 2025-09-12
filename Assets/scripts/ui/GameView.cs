using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Transform npcTransform;
    public Text npcMoneyText;
    public Text npcPointText;
    public Text npcWinsText;
    public Text npcWinRateText;

    public Transform userTransform;
    public Text userMoneyText;
    public Text userPointText;
    public Text userWinsText;
    public Text userWinRateText;

    public Button stopPokerBtn;
    public Button dealPokerBtn;

    public GameObject pokerPrefab;
    public Transform rootTransform;
    public Transform discardTransform;

    public GameObject resultPanel;
    public Text resultText;

    public GameObject npcTipsPanel;
    public GameObject userTipsPanel;

    private CommonSettle _gameSettle = new CommonSettle();
    // Start is called before the first frame update
    void Start()
    {
        updateUserInfo();
        setBtnInteractable(false);
        PlayPokerMgr.Instance.setGameSettle(_gameSettle);

        EventDispatcher.Instance.on(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.on(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.on(GameConst.PLAYERACTION, this.playerAction);
        EventDispatcher.Instance.on(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.on(GameConst.GAMEOVER, this.gameOver);

        StartCoroutine(dealPokerAfterAction());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.off(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.off(GameConst.PLAYERACTION, this.playerAction);
        EventDispatcher.Instance.off(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.off(GameConst.GAMEOVER, this.gameOver);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void updateUserInfo() {
        List<IUser> list = PlayPokerMgr.Instance.getPlayers();
        for (int i = 0; i < list.Count; i++)
        {
            IUser user = list[i];
            if (user.isNpc())
            {
                npcMoneyText.text = user.getMoney().ToString();
                npcPointText.text = "0";
                npcWinsText.text = user.getWins().ToString();
                npcWinRateText.text = string.Format("{0:P1}", user.getWinRate());
            }
            else
            {
                userMoneyText.text = user.getMoney().ToString();
                userPointText.text = "0";
                userWinsText.text = user.getWins().ToString();
                userWinRateText.text = string.Format("{0:P1}", user.getWinRate());
            }
        }
    }

    private IEnumerator dealPokerAfterAction() {
        yield return new WaitForSeconds(1.0f);
        int money = _gameSettle.bettingMoney(PlayPokerMgr.Instance.getPlayers());

        Text text1 = Instantiate(npcMoneyText,rootTransform);
        text1.gameObject.transform.position = npcMoneyText.gameObject.transform.position;
        text1.alignment = TextAnchor.MiddleCenter;
        text1.text = money.ToString();

        Text text2 = Instantiate(userMoneyText,rootTransform);
        text2.gameObject.transform.position = userMoneyText.gameObject.transform.position;
        text2.alignment = TextAnchor.MiddleCenter;
        text2.text = money.ToString();
        yield return new WaitForSeconds(0.1f);

        updateUserInfo();

        iTween.MoveTo(text1.gameObject, pokerPrefab.transform.position, 0.5f);
        iTween.MoveTo(text2.gameObject, pokerPrefab.transform.position, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(text1.gameObject);
        Destroy(text2.gameObject);
        yield return new WaitForSeconds(0.5f);

        GameCtrl.Instance.setHandleMessageComplete();
    }

    private void addPoker(IUser user, IPoker poker,int point, Transform parent, Text text) {
        GameObject pokerObject = Instantiate(pokerPrefab, parent);
        pokerObject.GetComponent<Poker>().loadPokerRes(poker);
        pokerObject.transform.position = pokerPrefab.transform.position;

        Vector3 pos = parent.position;
        float count = parent.childCount;
        float index = count - 1;
        float scalex = pokerObject.transform.localScale.x;
        float width = pokerObject.GetComponent<RectTransform>().rect.width * scalex;
        float maxWidth = parent.gameObject.GetComponent<RectTransform>().rect.width;
        float offX = count <= 1 ? 60 : Math.Min((maxWidth - width * count) / (count - 1), 60);
        float startX = pos.x - index * (width * scalex + offX) / 2;

        for (int i = 0; i < count; i++)
        {
            Vector3 localPos = new Vector3(startX + (width * scalex + offX) * i, pos.y, pos.z);
            iTween.MoveTo(parent.GetChild(i).gameObject, localPos, 0.5f);
        }

        if (point < 21) {
            text.color = Color.black;
        } else if (point == 21) {
            text.color = new Color(255, 223, 0);
        } else {
            text.color = Color.red;
        }
        text.text = point.ToString();
    }

    public void onReturnClick() {
        Debug.Log("onReturnClick");
        EventDispatcher.Instance.emit("returnToLobby");
    }

    public void onCloseClick() {
        Debug.Log("onCloseClick");
        EventDispatcher.Instance.emit("returnToLobby");
    }

    public void onDealPokerClick() {
        setBtnInteractable(false);
        PlayPokerMgr.Instance.dealPoker();
        GameCtrl.Instance.setHandleMessageComplete();
    }

    public void onStopPokerClick() {
        setBtnInteractable(false);
        PlayPokerMgr.Instance.stopDealPoker();
        GameCtrl.Instance.setHandleMessageComplete();
    }

    private void dealPoker(params System.Object[] obj)
    {
        StartCoroutine(dealPokerHandle(obj));
    }

    private IEnumerator dealPokerHandle(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        IPoker poker = (IPoker)obj[1];
        int point = (int)obj[2];
 
        Transform transform;
        Text text;

        if (user.isNpc())
        {
            transform = npcTransform;
            text = npcPointText;
        }
        else {
            transform = userTransform;
            text = userPointText;
        }
        addPoker(user, poker, point, transform, text);

        if (!user.isNpc()) { 
            //获取真实的分数
            if (point == 21)
            {
                if (HandPokerMgr.Instance.isBlackJack(user)){
                    yield return new WaitForSeconds(0.5f);

                    userTipsPanel.SetActive(true);
                    Text tips = userTipsPanel.GetComponentInChildren<Text>();
                    tips.text = "Blackack";
                    tips.color = new Color(255, 223, 0);
                }
            }
            else if (point > 21)
            {
                yield return new WaitForSeconds(0.5f);
                userTipsPanel.SetActive(true);
                Text tips = userTipsPanel.GetComponentInChildren<Text>();
                tips.text = "爆牌！！";
                tips.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.6f);
        GameCtrl.Instance.setHandleMessageComplete();
    }

    private void stopDealPoker(params System.Object[] obj) {
        IUser user = (IUser)obj[0];
        GameObject panel = user.isNpc() ? npcTipsPanel : userTipsPanel;

        panel.SetActive(true);
        Text text = panel.GetComponentInChildren<Text>();
        text.text = "停牌";
        GameCtrl.Instance.setHandleMessageComplete();
    }

    private void playerAction(params System.Object[] obj) {
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            StartCoroutine(npcAutoDealPokerHandle(user));
        }
        else { 
            //用户自行操作
        }
        setBtnInteractable(!user.isNpc());
    }

    private IEnumerator npcAutoDealPokerHandle(IUser user){
        System.Random rd = new System.Random();
        yield return new WaitForSeconds(rd.Next(1, 2));
        int number = HandPokerMgr.Instance.getHandPokerPoint(user, false);
        if (number >= 17)
        {
            PlayPokerMgr.Instance.stopDealPoker();
        }
        else {
            PlayPokerMgr.Instance.dealPoker();
        }
        GameCtrl.Instance.setHandleMessageComplete();
    }

    private void gameOver(params System.Object[] obj) {
        StartCoroutine(gameOverHandle(obj));
    }

    private IEnumerator gameOverHandle(params System.Object[] obj) {
        setBtnInteractable(false);
        yield return new WaitForSeconds(0.5f);

        EventDispatcher.Instance.emit(GameConst.FLIPPOKER);
        yield return new WaitForSeconds(0.5f);

        List<IUser> list = PlayPokerMgr.Instance.getPlayers();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isNpc()) {
                int point = HandPokerMgr.Instance.getHandPokerPoint(list[i], false);
                npcPointText.text = point.ToString();

                //获取真实的分数
                if (point == 21)
                {
                    if (HandPokerMgr.Instance.isBlackJack(list[i]))
                    {
                        yield return new WaitForSeconds(0.5f);

                        npcTipsPanel.SetActive(true);
                        Text tips = npcTipsPanel.GetComponentInChildren<Text>();
                        tips.text = "Blackack";
                        tips.color = new Color(255, 223, 0);
                    }
                }
                else if (point > 21)
                {
                    yield return new WaitForSeconds(0.5f);
                    npcTipsPanel.SetActive(true);
                    Text tips = npcTipsPanel.GetComponentInChildren<Text>();
                    tips.text = "爆牌！！";
                    tips.color = Color.red;
                }
            }
        }

        yield return new WaitForSeconds(1f);
        int money = _gameSettle.getSettleMoney();

        string str1 = "";
        string str2 = "";
        int pos1 = 0;
        int pos2 = 0;
        Color color1;
        Color color2;

        IUser user = (IUser)obj[0];
        if (user == null)
        {
            resultText.text = "平局";
            str1 = "+" + money;
            str2 = "+" + money;
            pos1 = 30;
            pos2 = 30;
            color1 = Color.green;
            color2 = Color.green;
        }
        else {
            if (user.isNpc())
            {
                str1 = "+" + money;
                str2 = "-" + money;
                pos1 = 30;
                pos2 = -30;
                color1 = Color.green;
                color2 = Color.red;
                resultText.text = "NPC获胜";
            }
            else {
                str1 = "-" + money;
                str2 = "+" + money;
                pos1 = -30;
                pos2 = 30;
                color1 = Color.red;
                color2 = Color.green;
                resultText.text = "玩家获胜";
            }
        }
        resultPanel.SetActive(true);

        Text text1 = Instantiate(npcMoneyText, rootTransform);
        text1.gameObject.transform.position = npcMoneyText.gameObject.transform.position;
        text1.text = str1;
        text1.color = color1;
        iTween.MoveBy(text1.gameObject, new Vector3(0, pos1, 0), 0.5f);

        Text text2 = Instantiate(userMoneyText, rootTransform);
        text2.gameObject.transform.position = userMoneyText.gameObject.transform.position;
        text2.text = str1;
        text2.color = color2;
        iTween.MoveBy(text2.gameObject, new Vector3(0, pos2, 0), 0.5f);
        
        yield return new WaitForSeconds(0.5f);
        Destroy(text1.gameObject);
        Destroy(text2.gameObject);
        updateUserInfo();

        yield return new WaitForSeconds(0.5f);
        resultPanel.SetActive(false);

        npcTipsPanel.SetActive(false);
        Text textPanel1 = npcTipsPanel.GetComponentInChildren<Text>();
        textPanel1.text = "爆牌！！";
        textPanel1.color = Color.red;

        userTipsPanel.SetActive(false);
        Text textPanel2 = userTipsPanel.GetComponentInChildren<Text>();
        textPanel2.text = "爆牌！！";
        textPanel2.color = Color.red;

        userPointText.color = Color.white;
        npcPointText.color = Color.white;

        List<Transform> child = new List<Transform>();
        for (int i = 0; i < userTransform.childCount; i++) {
            child.Add(userTransform.GetChild(i));
        }
        for (int i = 0; i < npcTransform.childCount; i++)
        {
            child.Add(npcTransform.GetChild(i));
        }
        for (int i = 0; i < child.Count; i++)
        {
            iTween.MoveTo(child[i].gameObject, discardTransform.position, 0.5f);
            child[i].SetParent(discardTransform);
        }
        yield return new WaitForSeconds(1.0f);


        //结算了
        bool isOver = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].getMoney() <= 0) {
                isOver = true;
            }
        }

        if (isOver)
        {
            EventDispatcher.Instance.emit("returnToLobby");
        }
        else {
            HandPokerMgr.Instance.resetHandPoker();
            PlayPokerMgr.Instance.startPlayPoker();
            StartCoroutine(dealPokerAfterAction());
        }
            
    }

    private void setBtnInteractable(bool able) {
        stopPokerBtn.interactable = dealPokerBtn.interactable = able;
    }

    private void shufflePoker(params System.Object[] obj) {
        StartCoroutine(shufflePokerHandle(obj));
    }

    private IEnumerator shufflePokerHandle(params System.Object[] obj) {
        int number = (int)obj[0];
        if (number == 0)
        {
            List<Transform> child = new List<Transform>();
            for (int i = 0; i < discardTransform.childCount; i++)
            {
                child.Add(discardTransform.GetChild(i));
            }
            for (int i = child.Count-1; i > -1; i--)
            {
                child[i].gameObject.GetComponent<Poker>().loadBackPoker();
                iTween.MoveTo(child[i].gameObject, pokerPrefab.GetComponent<Transform>().position, 0.1f);
                yield return new WaitForSeconds(0.15f);
                Destroy(child[i].gameObject);
                if (i == 0)
                {
                    pokerPrefab.SetActive(true);
                }
            }
            GameCtrl.Instance.setHandleMessageComplete();
        }
        else if (number == 1) {

            GameCtrl.Instance.setHandleMessageComplete();
            yield return new WaitForSeconds(0.1f);
            pokerPrefab.SetActive(false);
        }
    }
}
