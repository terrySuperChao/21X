using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public GameObject npcHeadImage;
    public Transform npcPokers;
    public Text npcPointText;
    public Text npcWinsText;
    public Text npcWinRateText;
    public Text npcBloodText;
    public Text npcAttackText;
    public Text npcDefenseText;
    public Text npcMagicText;

    public GameObject userHeadImage;
    public Transform userPokers;
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
    public Text effectText;

    public GameObject cardPrefab;

    public Transform npcCards;
    public Transform userCards;
    public GameObject refactoringGameObject;
    public GameObject tipsView;

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.DEALCARD, this.dealCard);
        
        EventDispatcher.Instance.on(GameConst.CANDIDACYCARD, this.candidacyCard);
        EventDispatcher.Instance.on(GameConst.OKSELECTCARD, this.okSelectCard);
        EventDispatcher.Instance.on(GameConst.CANCELSELECTCARD, this.cancelSelectCard);

        EventDispatcher.Instance.on(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.on(GameConst.TURNPLAYER, this.turnPlayer);
        EventDispatcher.Instance.on(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.on(GameConst.WAITOPERATOR, this.waitOperator);
        EventDispatcher.Instance.on(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.on(GameConst.FIGHTFLOWSTATE, this.fightFlowState);
        EventDispatcher.Instance.on(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.on(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.on(GameConst.GAMEOVER, this.gameOver);

        EventDispatcher.Instance.on(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.on(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.on(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.on(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.on(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.on(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.on(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.on(GameConst.GAMENEXTROUND, this.gameNextRound);
        EventDispatcher.Instance.on(GameConst.SHOWTIPS, this.onShowTips);
        EventDispatcher.Instance.on(GameConst.EXIT_PAGE, this.exitPageHandle);

        FightPokerMgr.Instance.init();
        FightPokerMgr.Instance.runFlow();

        this.initPokers();
        this.initCards();
        this.initUserInfo();
        this.setBtnInteractable(false);

        Invoke("delayMessageComplete", 0.5f);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.DEALCARD, this.dealCard);

        EventDispatcher.Instance.off(GameConst.CANDIDACYCARD, this.candidacyCard);
        EventDispatcher.Instance.off(GameConst.OKSELECTCARD, this.okSelectCard);
        EventDispatcher.Instance.off(GameConst.CANCELSELECTCARD, this.cancelSelectCard);

        EventDispatcher.Instance.off(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.off(GameConst.TURNPLAYER, this.turnPlayer);
        EventDispatcher.Instance.off(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.off(GameConst.WAITOPERATOR, this.waitOperator);
        EventDispatcher.Instance.off(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.off(GameConst.FIGHTFLOWSTATE, this.fightFlowState);
        EventDispatcher.Instance.off(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.off(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.off(GameConst.GAMEOVER, this.gameOver);
        
        EventDispatcher.Instance.off(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.off(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.off(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.off(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.off(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.off(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.off(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.off(GameConst.GAMENEXTROUND, this.gameNextRound);
        EventDispatcher.Instance.off(GameConst.SHOWTIPS, this.onShowTips);
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
    }
    public void exitPageHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void updateUserInfo() {
        List<IUser> list = FightPokerMgr.Instance.getPlayers();
        foreach (var user in list)
        {
            this.setUserInfo(user, ValueType.blood);
            this.setUserInfo(user, ValueType.attack);
            this.setUserInfo(user, ValueType.defense);
            this.setUserInfo(user, ValueType.magic);
            this.setUserInfo(user, ValueType.point);
            this.setUserInfo(user, ValueType.winRate);
        }
    }

    private void initUserInfo()
    {
        this.updateUserInfo();
    }

    private void initPokers() {
        List<IUser> players = FightPokerMgr.Instance.getPlayers();
        foreach (var user in players)
        {
            List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(user);
            foreach (var poker in pokers)
            {
                int point = FightPokerMgr.Instance.getUserHandPokerPoint(user, user.isNpc());
                this.addPoker(new DealPokerPara(user, poker));
            }
            this.showUserState(user);
        }
    }

    private void initCards() {
        List<IUser> players = FightPokerMgr.Instance.getPlayers();
        foreach (var user in players)
        {
            List<ICard> cards = FightPokerMgr.Instance.getUserCards(user);
            foreach (var card in cards)
            {
                this.addCard(new SelectCardPara(user, card, new Vector3()));
            }
        }
    }

    private void fightFlowState(params System.Object[] obj) {
        FightFlowState state = (FightFlowState)obj[0];
        GameReqMgr.Instance.requestSaveFightFlowState(state);
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void addPoker(IDealPokerPara para) {
        Transform transform = para.getUser().isNpc() ? npcPokers : userPokers;
        
        GameObject pokerObject = Instantiate(pokerPrefab, transform);
        pokerObject.GetComponent<Poker>().loadPokerRes(para.getPoker());
        pokerObject.transform.position = pokerPrefab.transform.position;

        Vector3 pos = new Vector3(0, 0, 0);
        float count = transform.childCount;
        float index = count - 1;
        float scalex = pokerObject.transform.localScale.x;
        float width = pokerObject.GetComponent<RectTransform>().rect.width * scalex;
        float maxWidth = transform.gameObject.GetComponent<RectTransform>().rect.width;
        float offX = count <= 1 ? 120 : Math.Min((maxWidth - width * count) / (count - 1), 120);
        float startX = pos.x - index * (width * scalex + offX) / 2;
        
        for (int i = 0; i < count; i++)
        {
            Vector3 localPos = new Vector3(startX + (width * scalex + offX) * i, pos.y, pos.z);
            moveTo(transform.GetChild(i).gameObject, localPos);
        }
    }

    private void addCard(ISelectCardPara para){
        Transform parent = para.getUser().isNpc() ? npcCards : userCards;
        int index = getCardForTypeIndex(para.getCard(), parent);
        GameObject cardObject = Instantiate(cardPrefab, parent);
        cardObject.GetComponent<Card>().loadCard(para.getCard());
        cardObject.transform.position = parent.GetChild(index).position;
        cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        Destroy(parent.GetChild(index).gameObject);
    }


    // ------》》》》 1发卡牌
    public void dealCard(params System.Object[] obj)
    {
        IDealCardPara para = (IDealCardPara)obj[0];
        this.addCard(new SelectCardPara(para.getUser(), para.getCard(), new Vector3()));
        Invoke("delayMessageComplete", 0.5f);
    }

    // ------》》》》 1.1 选择卡牌
    public void candidacyCard(params System.Object[] obj)
    {
        ICandidacyCardPara para = (ICandidacyCardPara)obj[0];
        UIMgr.Instance.showTips("SelectCardView", obj[0]);
    }

    public void okSelectCard(params System.Object[] obj)
    {
        StartCoroutine(okSelectCardHandle(obj));
    }

    private IEnumerator okSelectCardHandle(params System.Object[] obj)
    {
        ISelectCardPara para = (ISelectCardPara)obj[0];
        IUser user = para.getUser();
        ICard card = para.getCard();
        
        GameObject cardObject = Instantiate(this.cardPrefab, this.rootTransform);
        cardObject.GetComponent<Card>().loadCard(card);
        cardObject.GetComponent<Card>().showNameText(false);
        cardObject.transform.position = para.getPosition();

        int index = this.getCardForTypeIndex(card, this.userCards);
        iTween.MoveTo(cardObject, this.userCards.GetChild(index).position, 0.5f);
        yield return new WaitForSeconds(0.6f);
        Destroy(this.userCards.GetChild(index).gameObject);
        cardObject.GetComponent<Card>().showNameText(true);
        cardObject.transform.SetParent(this.userCards);
        cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        Invoke("delayMessageComplete", 0.1f);
    }

    public void cancelSelectCard(params System.Object[] obj)
    {
        GameMessage.Instance.setHandleMessageComplete();
    }


    // ------》》》》 2发扑克牌
    private void dealPoker(params System.Object[] obj)
    {
        IDealPokerPara para = (IDealPokerPara)obj[0];
        this.addPoker(para);
        this.showUserState(para.getUser());

        Invoke("delayMessageComplete", 0.6f);
    }

    // ------》》》》 3轮到操作者
    private void turnPlayer(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        this.setBtnInteractable(!user.isNpc());

        Invoke("delayMessageComplete", 0.1f);
    }

    // ------》》》》 4 等待操作
    private void waitOperator(params System.Object[] obj) {
        StartCoroutine(waitOperatorHandle(obj));
    }

    private IEnumerator waitOperatorHandle(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            yield return new WaitForSeconds(RandomMgr.Instance.getRangeInt(1, 3));
            FightFlowState state = FightPokerMgr.Instance.getUserHandPokerPoint(user, false) >= 17 ? FightFlowState.stopDealPoker : FightFlowState.dealPoker;
            GameReqMgr.Instance.requestSaveFightFlowState(state);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            this.setBtnInteractable(true);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    // ------》》》》 5 统计点数
    private void stopDealPoker(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        this.showUserState(user);
        GameMessage.Instance.setHandleMessageComplete();
    }


    // ------》》》》 6 统计点数
    private void totalPokerPoint(params System.Object[] obj)
    {
        ITotalHandPokerPointPara para = (ITotalHandPokerPointPara)obj[0];
        this.showUserState(para.getUser());
        GameMessage.Instance.setHandleMessageComplete();
    }
    private void showUserState(IUser user) {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(user);
        int index = pokers.FindIndex(poker => poker.isBack());
        int point = FightPokerMgr.Instance.getUserHandPokerPoint(user,index == -1);
        bool isBack = FightPokerMgr.Instance.isUserHandPokerBlackJack(user);

        Text text = null;
        Text tips = null;
        GameObject panel = null;
        
        if (user.isNpc())
        {
            text = npcPointText;
            panel = npcTipsPanel;
            tips = npcTipsPanel.GetComponentInChildren<Text>();
        }
        else
        {
            text = userPointText;
            panel = userTipsPanel;
            tips = userTipsPanel.GetComponentInChildren<Text>();
        }
        
        if (isBack)
        {
            tips.text = "Blackack";
            tips.color = new Color(255, 223, 0);
            panel.SetActive(true);

        }

       if (point > 21)
       {
            tips.text = "爆牌";
            tips.color = Color.red;
            panel.SetActive(true);
        }
         
        if (user.getState() == UserState.end)
        {
            tips.text = "停牌";
            tips.color = Color.red;
            panel.SetActive(true);
        }

        if (point < 21)
        {
            text.color = Color.black;
        }
        else if (point == 21)
        {
            text.color = new Color(255, 223, 0);
        }
        else
        {
            text.color = Color.red;
        }
        text.text = point.ToString();
    }

    private void gameSettle(params System.Object[] obj)
    {
        StartCoroutine(gameSettleHandle(obj));
    }

    private IEnumerator gameSettleHandle(params System.Object[] obj)
    {
        setBtnInteractable(false);
        yield return new WaitForSeconds(0.5f);

        EventDispatcher.Instance.emit(GameConst.FLIPPOKER);
        yield return new WaitForSeconds(0.5f);

        IUser npc = FightPokerMgr.Instance.getPlayers().Find(user => user.isNpc());
        int point = FightPokerMgr.Instance.getUserHandPokerPoint(npc, false);
        npcPointText.text = point.ToString();

        yield return new WaitForSeconds(0.1f);
        this.showUserState(npc);

        yield return new WaitForSeconds(1f);
        IUser user = (IUser)obj[0];

        if (user == null)
        {
            resultText.text = "本回合平局";
        }
        else
        {
            if (user.isNpc())
            {
                resultText.text = "本回合NPC获胜";
            }
            else
            {
                resultText.text = "本回合玩家获胜";
            }
        }
        resultPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        resultPanel.SetActive(false);
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void gameOver(params System.Object[] obj)
    {
        GameReqMgr.Instance.requestSavePlayerInfo();
        GameReqMgr.Instance.requestExitPage();
        EventDispatcher.Instance.emit("returnToLobby");
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onReturnClick() {
        Debug.Log("onReturnClick");
        EventDispatcher.Instance.emit(GameConst.RETURNTOLOBBY);
    }

    public void onCloseClick() {
        Debug.Log("onCloseClick");
        EventDispatcher.Instance.emit(GameConst.RETURNTOLOBBY);
    }

    public void onDealPokerClick() {
        this.refactoringGameObject.SetActive(false);
        this.setBtnInteractable(false);
        GameReqMgr.Instance.requestSaveFightFlowState(FightFlowState.dealPoker);
    }

    public void onStopPokerClick() {
        this.refactoringGameObject.SetActive(false);
        this.setBtnInteractable(false);
        GameReqMgr.Instance.requestSaveFightFlowState(FightFlowState.stopDealPoker);
    }

    
    private void setBtnInteractable(bool able) {
        stopPokerBtn.interactable = dealPokerBtn.interactable = able;
    }

    private void shufflePoker(params System.Object[] obj) {
        Invoke("delayMessageComplete", 0.1f);
    }


    private int getCardForTypeIndex(ICard card, Transform parent) {
        int index = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            Card cardComp = parent.GetChild(i).GetComponent<Card>();
            if (cardComp != null && cardComp.getCard().getType() == card.getType())
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private Transform getPokerIdTransform(IUser user, IPoker poker)
    {
        Transform parent = user.isNpc() ? npcPokers : userPokers;
        for (int i = 0; i < parent.childCount; i++)
        {
            Poker pokerComp = parent.GetChild(i).GetComponent<Poker>();
            if (pokerComp != null && pokerComp.getPoker().getId() == poker.getId())
            {
                return parent.GetChild(i);
            }
        }
        return null;
    }

    private Transform getCardIdTransform(IUser user, ICard card)
    {
        Transform parent = user.isNpc() ? npcCards : userCards;
        for (int i = 0; i < parent.childCount; i++)
        {
            Card cardComp = parent.GetChild(i).GetComponent<Card>();
            if (cardComp != null && cardComp.getCard().getId() == card.getId())
            {
                return parent.GetChild(i);
            }
        }
        return null;
    }

    public void addPokerValue(params System.Object[] obj)
    {
        StartCoroutine(addPokerValueHandle(obj));
    }
    private IEnumerator addPokerValueHandle(params System.Object[] obj)
    {
        IUIPokerPara para = (IUIPokerPara)obj[0];
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        Text text = getText(para.getUser(), GameConst.SuitTransformValueType(suit));
        Transform child = getPokerIdTransform(para.getUser(), para.getPoker());
        if (child != null && text != null)
        {
            string str = "";
            if (para.getMult() > 1.0f)
            {
                str = "+" + (para.getAddValue() / para.getMult()) + " X" + para.getMult();
            }
            else {
                str = "+" + para.getAddValue();
            }

            Text addText = Instantiate(text, rootTransform);
            addText.transform.position = text.transform.position;
            addText.text = str;
            addText.color = Color.green;

            Vector3 localPos = addText.transform.localPosition;
            moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + 50, localPos.z));
            iTween.ScaleTo(child.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
            yield return new WaitForSeconds(0.51f);
            iTween.ScaleTo(child.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 0.1f);
            Destroy(addText.gameObject);
            text.text = getFinalContent(para.getUser(), GameConst.SuitTransformValueType(suit), para.getFinalValue());
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void addCardValue(params System.Object[] obj)
    {
        StartCoroutine(addCardValueHandle(obj));
    }

    private IEnumerator addCardValueHandle(params System.Object[] obj)
    {
        IUICommonPara para = (IUICommonPara)obj[0];
        Text text = getText(para.getUser(), para.getValueType());
        if (text != null) {
            Text addText = Instantiate(text, rootTransform);
            addText.transform.position = text.transform.position;
            if (para.getValue() > 0)
            {
                addText.text = "+" + para.getValue();
                addText.color = Color.green;
                Vector3 localPos = addText.transform.localPosition;
                moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + 50, localPos.z));
            }
            else {
                addText.text = "" + para.getValue();
                addText.color = Color.red;
                Vector3 localPos = addText.transform.localPosition;
                moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y - 50, localPos.z));
            }
            yield return new WaitForSeconds(0.51f);
            Destroy(addText.gameObject);
            text.text = getFinalContent(para.getUser(), para.getValueType(), para.getFinalValue());
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    private Text getText(IUser user, ValueType type) {
        Text text = null;
        switch (type)
        {
            case ValueType.defense: // 方
                text = user.isNpc() ? npcDefenseText : userDefenseText;
                break;
            case ValueType.blood: // 红
                text = user.isNpc() ? npcBloodText : userBloodText;
                break;
            case ValueType.attack: // 黑
                text = user.isNpc() ? npcAttackText : userAttackText;
                break;
            case ValueType.magic: // 梅
                text = user.isNpc() ? npcMagicText : userMagicText;
                break;
            default:
                break;
        }
        return text;
    }

    private void setUserInfo(IUser user, ValueType type) {
        if (user.isNpc())
        {
            switch (type)
            {
                case ValueType.defense: // 方
                    npcDefenseText.text = user.getDefense().ToString();
                    break;
                case ValueType.blood: // 红
                    npcBloodText.text = user.getBlood() + "/" + user.getMaxBlood();
                    break;
                case ValueType.attack: // 黑
                    npcAttackText.text = user.getAttack().ToString();
                    break;
                case ValueType.magic: // 梅
                    npcMagicText.text = user.getMagic() + "/ " + user.getMaxMagic();
                    break;
                case ValueType.winRate: //赢率
                    npcWinRateText.text = string.Format("{0:P1}", user.getWinRate());
                    break;
                default:
                    break;
            }
        }
        else {
            switch (type)
            {
                case ValueType.defense: // 方
                    userDefenseText.text = user.getDefense().ToString();
                    break;
                case ValueType.blood: // 红
                    userBloodText.text = user.getBlood() + "/" + user.getMaxBlood();
                    break;
                case ValueType.attack: // 黑
                    userAttackText.text = user.getAttack().ToString();
                    break;
                case ValueType.magic: // 梅
                    userMagicText.text = user.getMagic() + "/ " + user.getMaxMagic();
                    break;
                case ValueType.winRate: //赢率
                    userWinRateText.text = string.Format("{0:P1}", user.getWinRate());
                    break;
                default:
                    break;
            }
        }
    }

    private string getFinalContent(IUser user, ValueType type, float value)
    {
        float maxValue = -1;
        float finalValue = (float)Math.Round((value * 10 + 0.5) / 10, 1);
        switch (type)
        {
            case ValueType.defense: // 方
                break;
            case ValueType.blood: // 红
                maxValue = user.getMaxBlood();
                break;
            case ValueType.attack: // 黑
                break;
            case ValueType.magic: // 梅
                maxValue = user.getMaxMagic();
                break;
            default:
                break;
        }
        if (maxValue == -1)
        {
            return finalValue.ToString();
        }
        else {
            return finalValue + "/" + maxValue;
        }
    }

    public void commonAttack(params System.Object[] obj)
    {
        StartCoroutine(commonAttackHandle(obj));
    }

    private IEnumerator commonAttackHandle(params System.Object[] obj)
    {
        IUICommonPara para = (IUICommonPara)obj[0];
        this.setUserInfo(para.getUser(), para.getValueType());

        if (para.getUser().isNpc())
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

        attackImage.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        attackImage.SetActive(false);
        effectImage.SetActive(true);
        effectText.text = "-" + para.getValue().ToString();
        yield return new WaitForSeconds(0.5f);
        effectImage.SetActive(false);
        
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void flyFont(params System.Object[] obj)
    {
        StartCoroutine(flyFontHandle(obj));
    }

    private IEnumerator flyFontHandle(params System.Object[] obj)
    {
        IUIFlyFontPara para = (IUIFlyFontPara)obj[0];
        Transform child = getCardIdTransform(para.getUser(), para.getCard());
        if (child != null)
        {
            Debug.Log("文字====" + para.getText());
            Text addText = Instantiate(userAttackText, rootTransform);
            addText.transform.position = child.position;
            addText.text = para.getText();
            addText.fontSize = 40;
            addText.color = Color.green;
            Vector3 localPos = addText.transform.localPosition;
            moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + 50, localPos.z));
            iTween.ScaleTo(child.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 0.5f);
            yield return new WaitForSeconds(0.51f);
            Destroy(addText.gameObject);
            iTween.ScaleTo(child.gameObject, new Vector3(0.7f, 0.7f, 0.7f), 0.1f);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }
    public void refactoring(params System.Object[] obj)
    {
        IRefactoringPara para = (IRefactoringPara)obj[0];
        if (para.getUser().isNpc()){
            FightPokerMgr.Instance.reDealHandPoker(para.getUser());
        }else {
            //refactoringGameObject.GetComponent<Refactoring>().setFactoringNum(para);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void reHandPoker(params System.Object[] obj) {
        IReHandPokerPara para = (IReHandPokerPara)obj[0];
        FightPokerMgr.Instance.reDealHandPoker(para.getUser(), para.getSuit());
    }
    
    public void clearHandPoker(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        List<Transform> child = new List<Transform>();
        Transform transform = user.isNpc() ? npcPokers : userPokers;
        for (int i = 0; i < transform.childCount; i++)
        {
            child.Add(transform.GetChild(i));
        }
        for (int i = 0; i < child.Count; i++)
        {
            Destroy(child[i].gameObject);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void gameNextRound(params System.Object[] obj)
    {
        StartCoroutine(gameNextRoundHandle(obj));
    }
    private IEnumerator gameNextRoundHandle(params System.Object[] obj) {
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
        for (int i = 0; i < userPokers.childCount; i++)
        {
            child.Add(userPokers.GetChild(i));
        }
        for (int i = 0; i < npcPokers.childCount; i++)
        {
            child.Add(npcPokers.GetChild(i));
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
        FightPokerMgr.Instance.clear();
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void onShowTips(params System.Object[] obj)
    {
        GameObject tipsViewObject = Instantiate(tipsView);
        tipsViewObject.name = "tipsView";
        tipsViewObject.transform.parent = tipsView.transform.parent;
        tipsViewObject.transform.position = tipsView.transform.position;
        tipsViewObject.SetActive(true);
        tipsViewObject.GetComponent<TipsView>().setText((string)obj[0]);
    }

    private void moveTo(GameObject gameObject, Vector3 position) {
        iTween.MoveTo(gameObject, iTween.Hash("position", position, "time", 0.5f, "isLocal", true, "easeType", iTween.EaseType.linear));
    }

    private void delayMessageComplete() {
        GameMessage.Instance.setHandleMessageComplete();
    }
}
