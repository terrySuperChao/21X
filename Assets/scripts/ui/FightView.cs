using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FightView : MonoBehaviour
{
    public GameObject npcHeadImage;
    public Transform npcTransform;    
    public Text npcPointText;
    public Text npcWinsText;
    public Text npcWinRateText;
    public Text npcBloodText;
    public Text npcAttackText;
    public Text npcDefenseText;
    public Text npcMagicText;

    public GameObject userHeadImage;
    public Transform userTransform;
    public Text userPointText;
    public Text userWinsText;
    public Text userWinRateText;
    public Text userBloodText;
    public Text userAttackText;
    public Text userDefenseText;
    public Text userMagicText;

    public Button stopPokerBtn;
    public Button dealPokerBtn;

    public GameObject pokerPrefab;
    public Transform rootTransform;

    public GameObject resultPanel;
    public Text resultText;

    public GameObject npcTipsPanel;
    public GameObject userTipsPanel;

    public GameObject effectImage;
    public GameObject attackImage;
    public Text attackText;
    
    private FightSettle _gameSettle = new FightSettle();
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
                npcPointText.text = "0";
                npcWinsText.text = user.getWins().ToString();
                npcWinRateText.text = string.Format("{0:P1}", user.getWinRate());
                npcBloodText.text = user.getBlood() + "/" + user.getMaxBlood();
                npcAttackText.text = user.getAttack().ToString();
                npcDefenseText.text = user.getDefense().ToString();
                npcMagicText.text = user.getMagic() + "/" + user.getMaxMagic();
            }
            else
            {
                userPointText.text = "0";
                userWinsText.text = user.getWins().ToString();
                userWinRateText.text = string.Format("{0:P1}", user.getWinRate());
                userBloodText.text = user.getBlood() + "/"+user.getMaxBlood();
                userAttackText.text = user.getAttack().ToString();
                userDefenseText.text = user.getDefense().ToString();
                userMagicText.text = user.getMagic()+"/"+user.getMaxMagic();
            }
        }
    }

    private IEnumerator dealPokerAfterAction() {
        yield return new WaitForSeconds(1.0f);
        updateUserInfo();
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
        IUser user = (IUser)obj[0];

        Transform transform = null;
        if (user == null) {
            resultText.text = "本回合平局";
            transform = null;
        }
        else {
            if (user.isNpc()) {
                resultText.text = "本回合NPC获胜";
                transform = npcTransform;
            }
            else {

                resultText.text = "本回合玩家获胜";
                transform = userTransform;
            }
        }
        resultPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (transform)
        {
            Text text = null;
            Text addText = null;
            List <IPoker> pokers = HandPokerMgr.Instance.getHandPoker(user);
            List<int> values = _gameSettle.getPokerValue(pokers);
            for (int i = 0; i < pokers.Count; i++)
            {
                float addValue = values[i];
                float maxValue = 999999;
                switch (pokers[i].getSuit())
                {
                    case 1: // 方
                        addValue *= 0.5f;
                        text = user.isNpc() ? npcDefenseText : userDefenseText;
                        break;
                    case 2: // 红
                        addValue *= 0.5f;
                        text = user.isNpc() ? npcBloodText : userBloodText;
                        maxValue = user.getMaxBlood();
                        break;
                    case 3: // 黑
                        addValue *= 1.0f;
                        text = user.isNpc() ? npcAttackText : userAttackText;
                        break;
                    case 4: // 梅
                        addValue *= 1.0f;
                        text = user.isNpc() ? npcMagicText : userMagicText;
                        maxValue = user.getMaxMagic();
                        break;
                    default:
                        break;
                }

                if (addText == null)
                {
                    addText = Instantiate(text, rootTransform);
                }
                addText.transform.position = transform.GetChild(i).transform.position;
                addText.text = "+" + addValue;
                iTween.MoveTo(addText.gameObject, text.transform.position, 0.5f);
                yield return new WaitForSeconds(0.6f);

                float oldValue = 0;
                float.TryParse(text.text.Split("/")[0], out oldValue);
                if (oldValue + addValue > maxValue)
                {
                    text.text = maxValue + "/" + maxValue;
                }
                else {
                    if (maxValue != 999999)
                    {
                        text.text = (oldValue + addValue)+ "/" + maxValue;
                    }
                    else {
                        text.text = (oldValue + addValue).ToString();
                    }
                }
            }

            if (addText != null) {
                Destroy(addText.gameObject);
            }
            
            float attack = _gameSettle.getSettleAttack();
            float defense = _gameSettle.getSettleDefense();
            if (attack > 0) { 
                Text bloodText;
                Text defenseText;
                if (user.isNpc())
                {
                    bloodText = userBloodText;
                    defenseText = userDefenseText;

                    attackImage.transform.position = npcHeadImage.transform.position;
                    effectImage.transform.position = userHeadImage.transform.position;
                    iTween.MoveTo(attackImage, userHeadImage.transform.position, 1.0f);
                }
                else
                {
                    bloodText = npcBloodText;
                    defenseText = npcDefenseText;
                    attackImage.transform.position = userHeadImage.transform.position;
                    effectImage.transform.position = npcHeadImage.transform.position;
                    iTween.MoveTo(attackImage, npcHeadImage.transform.position, 1.0f);
                }

                attackImage.SetActive(true);
                yield return new WaitForSeconds(1.1f);
                attackImage.SetActive(false);
                effectImage.SetActive(true);
                attackText.text = "-" + attack.ToString();
                yield return new WaitForSeconds(0.5f);
                effectImage.SetActive(false);

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].isNpc())
                    {
                        npcAttackText.text = list[i].getAttack().ToString();
                    }
                    else
                    {
                        userAttackText.text = list[i].getAttack().ToString();
                    }
                }

                Text tempText = null;
                if (defense > 0) {
                    if(tempText == null)
                    {
                        tempText = Instantiate(defenseText, rootTransform);
                    }
                    tempText.transform.position = defenseText.transform.position;
                    tempText.text = "-" + defense;
                    tempText.color = Color.red;
                    iTween.MoveBy(tempText.gameObject, new Vector3(0,-50,0), 0.5f);
                    yield return new WaitForSeconds(0.52f);

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].isNpc())
                        {
                            npcDefenseText.text = list[i].getDefense().ToString();
                        }
                        else
                        {
                            userDefenseText.text = list[i].getDefense().ToString();
                        }
                    }
                }

                if (attack > defense)
                {
                    if (tempText == null)
                    {
                        tempText = Instantiate(bloodText, rootTransform);
                    }
                    tempText.transform.position = bloodText.transform.position;
                    tempText.text = "-" + (attack - defense);
                    tempText.color = Color.red;
                    iTween.MoveBy(tempText.gameObject, new Vector3(0, -50, 0), 0.5f);
                    yield return new WaitForSeconds(0.52f);

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].isNpc())
                        {
                            npcBloodText.text = list[i].getBlood().ToString();
                        }
                        else
                        {
                            userBloodText.text = list[i].getBlood().ToString();
                        }
                    }
                }
                if (tempText != null) {
                    Destroy(tempText.gameObject);
                }

            }
            
            if (user.getMagic() >= ConfigMgr.INIT_MAGIC_VALUE) {
                yield return new WaitForSeconds(0.5f);
                attackImage.SetActive(true);
                if (user.isNpc())
                {
                    attackImage.transform.position = npcHeadImage.transform.position;
                    effectImage.transform.position = userHeadImage.transform.position;
                    iTween.MoveTo(attackImage, userHeadImage.transform.position, 1.0f);
                }
                else
                {
                    attackImage.transform.position = userHeadImage.transform.position;
                    effectImage.transform.position = npcHeadImage.transform.position;
                    iTween.MoveTo(attackImage, npcHeadImage.transform.position, 1.0f);
                }
                yield return new WaitForSeconds(1.0f);
                attackImage.SetActive(false);
                effectImage.SetActive(true);
                attackText.text = "-50";
                yield return new WaitForSeconds(0.5f);
                effectImage.SetActive(false);

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].getUserId() == user.getUserId())
                    {
                        list[i].addMagic(-list[i].getMagic());
                    }
                    else
                    {
                        list[i].addBlood(-50);
                    }
                }
            }
        }

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
            child[i].gameObject.GetComponent<Poker>().loadBackPoker();
            iTween.MoveTo(child[i].gameObject, pokerPrefab.transform.position, 0.5f);
        }
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < child.Count; i++)
        {
            Destroy(child[i].gameObject);
        }

        //结算了
        bool isOver = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].getBlood() <= 0) {
                isOver = true;
            }
        }

        if (isOver)
        {
            EventDispatcher.Instance.emit("returnToLobby");
        }
        else {
            PokerPileMgr.Instance.shuffle();
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
        GameCtrl.Instance.setHandleMessageComplete();
        yield return new WaitForSeconds(0.1f);
    }
}
