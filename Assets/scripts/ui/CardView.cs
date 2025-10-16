using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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

    public GameObject selectCard;
    public GameObject cardPrefab;

    public Transform npcCards;
    public Transform userCards;
    public GameObject refactoringGameObject;
    public GameObject tipsView;

    private CardFlow _gameFlow = new CardFlow();

    // Start is called before the first frame update
    void Start()
    {
        updateUserInfo();
        setBtnInteractable(false);

        PlayPokerMgr.Instance.setGameFlow(_gameFlow);
        PlayPokerMgr.Instance.startPlayPoker();

        EventDispatcher.Instance.on(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.on(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.on(GameConst.PLAYERACTION, this.playerAction);
        EventDispatcher.Instance.on(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.on(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.on(GameConst.GAMEOVER, this.gameOver);
        EventDispatcher.Instance.on(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.on(GameConst.SELECTFINSIHCARD, this.selectFinishCard);
        EventDispatcher.Instance.on(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.on(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.on(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.on(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.on(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.on(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.on(GameConst.CLEARHEADPOKER, this.clearHandPoker);
        EventDispatcher.Instance.on(GameConst.GAMENEXTROUND, this.gameNextRound);
        EventDispatcher.Instance.on(GameConst.SHOWTIPS, this.onShowTips);
        StartCoroutine(dealPokerAfterAction());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.off(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.off(GameConst.PLAYERACTION, this.playerAction);
        EventDispatcher.Instance.off(GameConst.SHUFFLEPOKER, this.shufflePoker);
        EventDispatcher.Instance.off(GameConst.GAMESETTLE, this.gameSettle);
        EventDispatcher.Instance.off(GameConst.GAMEOVER, this.gameOver);
        EventDispatcher.Instance.off(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.off(GameConst.SELECTFINSIHCARD, this.selectFinishCard);
        EventDispatcher.Instance.off(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.off(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.off(GameConst.COMMONATTACK, this.commonAttack);
        EventDispatcher.Instance.off(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.off(GameConst.REFACTORING, this.refactoring);
        EventDispatcher.Instance.off(GameConst.REHANDPOKER, this.reHandPoker);
        EventDispatcher.Instance.off(GameConst.CLEARHEADPOKER, this.clearHandPoker);
        EventDispatcher.Instance.off(GameConst.GAMENEXTROUND, this.gameNextRound);
        EventDispatcher.Instance.off(GameConst.SHOWTIPS, this.onShowTips);
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
                userBloodText.text = user.getBlood() + "/" + user.getMaxBlood();
                userAttackText.text = user.getAttack().ToString();
                userDefenseText.text = user.getDefense().ToString();
                userMagicText.text = user.getMagic() + "/" + user.getMaxMagic();
            }
        }
    }

    private IEnumerator dealPokerAfterAction() {
        yield return new WaitForSeconds(1.0f);
        updateUserInfo();
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void addPoker(IUser user, IPoker poker, int point, Transform parent, Text text) {
        GameObject pokerObject = Instantiate(pokerPrefab, parent);
        pokerObject.GetComponent<Poker>().loadPokerRes(poker);
        pokerObject.transform.position = pokerPrefab.transform.position;

        Vector3 pos = new Vector3(0, 0, 0);
        float count = parent.childCount;
        float index = count - 1;
        float scalex = pokerObject.transform.localScale.x;
        float width = pokerObject.GetComponent<RectTransform>().rect.width * scalex;
        float maxWidth = parent.gameObject.GetComponent<RectTransform>().rect.width;
        float offX = count <= 1 ? 120 : Math.Min((maxWidth - width * count) / (count - 1), 120);
        float startX = pos.x - index * (width * scalex + offX) / 2;
        
        for (int i = 0; i < count; i++)
        {
            Vector3 localPos = new Vector3(startX + (width * scalex + offX) * i, pos.y, pos.z);
            moveTo(parent.GetChild(i).gameObject, localPos);
        }

        if (point < 21) {
            text.color = Color.black;
        } else if (point == 21) {
            text.color = new Color(255, 223, 0);
        } else {
            text.color = Color.red;
        }
        text.text = point.ToString();
        Debug.Log("point===" + point);
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
        refactoringGameObject.SetActive(false);
        setBtnInteractable(false);
        PlayPokerMgr.Instance.dealPokerAction();
    }

    public void onStopPokerClick() {
        refactoringGameObject.SetActive(false);
        setBtnInteractable(false);
        PlayPokerMgr.Instance.stopDealPokerAction();
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
            transform = npcPokers;
            text = npcPointText;
        }
        else {
            transform = userPokers;
            text = userPointText;
        }
        addPoker(user, poker, point, transform, text);

        if (!user.isNpc()) {
            //获取真实的分数
            if (point == 21)
            {
                if (HandPokerMgr.Instance.isBlackJack(user)) {
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
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void stopDealPoker(params System.Object[] obj) {
        IUser user = (IUser)obj[0];
        GameObject panel = user.isNpc() ? npcTipsPanel : userTipsPanel;

        panel.SetActive(true);
        Text text = panel.GetComponentInChildren<Text>();
        text.text = "停牌";
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void playerAction(params System.Object[] obj) {
        StartCoroutine(playerActionHandle(obj));
    }

    private IEnumerator playerActionHandle(params System.Object[] obj) {
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            setBtnInteractable(false);
            yield return new WaitForSeconds(RandomMgr.Instance.getRangeInt(1, 3));
            int number = HandPokerMgr.Instance.getHandPokerPoint(user, false);
            if (number >= 17)
            {
                PlayPokerMgr.Instance.stopDealPokerAction();
            }
            else
            {
                PlayPokerMgr.Instance.dealPokerAction();
            }
        }
        else {
            setBtnInteractable(true);
            //用户自行操作
            yield return new WaitForSeconds(0.1f);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }

    private void gameSettle(params System.Object[] obj) {
        StartCoroutine(gameSettleHandle(obj));
    }

    private IEnumerator gameSettleHandle(params System.Object[] obj) {
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

        if (user == null) {
            resultText.text = "本回合平局";
        }
        else {
            if (user.isNpc()) {
                resultText.text = "本回合NPC获胜";
            }
            else {
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
        EventDispatcher.Instance.emit("returnToLobby");
    }

    private void setBtnInteractable(bool able) {
        stopPokerBtn.interactable = dealPokerBtn.interactable = able;
    }

    private void shufflePoker(params System.Object[] obj) {
        StartCoroutine(shufflePokerHandle(obj));
    }

    private IEnumerator shufflePokerHandle(params System.Object[] obj) {
        yield return new WaitForSeconds(0.1f);
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void dealCard(params System.Object[] obj) {
        IUser user = (IUser)obj[0];

        if (user == null) {
            GameMessage.Instance.setHandleMessageComplete();
            return;
        }

        List<ICard> cards = (List<ICard>)obj[1];
        if (cards == null) {
            GameMessage.Instance.setHandleMessageComplete();
            return;
        }

        if (user.isNpc())
        {
            ICard card = null;
            int rd = RandomMgr.Instance.getRangeInt(0, cards.Count);
            List<ICard> list = CardMgr.Instance.getCards(user);
            if (list != null) {
                for (int i = 0; i < list.Count; i++) {
                    for (int j = 0; j < cards.Count; j++) {
                        if (list[i].getType() == cards[j].getType()) {
                            card = cards[j];
                            break;
                        }
                    }
                    if (card != null) {
                        break;
                    }
                }
            }
            if (card == null) {
                card = cards[rd];
            }
            bool success = CardMgr.Instance.addCard(user, card);
            if (success) { 
                int index = getCardForTypeIndex(card, npcCards);

                GameObject cardObject = Instantiate(cardPrefab, npcCards);
                cardObject.GetComponent<Card>().loadCard(card);
                cardObject.transform.position = npcCards.GetChild(index).position;
                cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                Destroy(npcCards.GetChild(index).gameObject);

                _gameFlow.cardHandleTypeHandle(PlayPokerMgr.Instance.getPlayers(), false, CardHandleType.addNewCardAfter);
            }
            GameMessage.Instance.setHandleMessageComplete();
        }
        else {
            selectCard.SetActive(true);
            selectCard.GetComponent<SelectCardView>().initCards(user, cards);
        }
    }

    public void selectFinishCard(params System.Object[] obj)
    {
        StartCoroutine(selectFinishCardHandle(obj));
    }

    private IEnumerator selectFinishCardHandle(params System.Object[] obj) {
        IUser user = (IUser)obj[0];
        ICard card = (ICard)obj[1];
        selectCard.SetActive(false);
        
        if (card != null) {
            bool success = CardMgr.Instance.addCard(user, card);
            if (success)
            {
                GameObject cardObject = Instantiate(cardPrefab, rootTransform);
                cardObject.GetComponent<Card>().loadCard(card);
                cardObject.GetComponent<Card>().showNameText(false);
                cardObject.transform.position = (Vector3)obj[2];

                int index = getCardForTypeIndex(card, userCards);
                iTween.MoveTo(cardObject, userCards.GetChild(index).position, 0.5f);
                yield return new WaitForSeconds(0.6f);
                Destroy(userCards.GetChild(index).gameObject);
                cardObject.GetComponent<Card>().showNameText(true);
                cardObject.transform.SetParent(userCards);
                cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

                _gameFlow.cardHandleTypeHandle(PlayPokerMgr.Instance.getPlayers(), false, CardHandleType.addNewCardAfter);
            }
        }
        yield return new WaitForSeconds(0.1f);
        GameMessage.Instance.setHandleMessageComplete();
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
                addText.text = "-" + para.getValue();
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
    private string getFinalContent(IUser user, ValueType type, float finalValue)
    {
        float maxValue = -1;
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
        if (para.getValueType() == ValueType.magic)
        {
            Text text = getText(para.getUser(), ValueType.magic);
            text.text = para.getUser().getMagic() + "/" + para.getUser().getMaxMagic();
        }
        else
        {
            Text text = getText(para.getUser(), ValueType.attack);
            text.text = para.getUser().getAttack().ToString();
        }

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
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            int number = HandPokerMgr.Instance.getHandPokerPoint(user, false);
            if (number < 21) {
                if (RandomMgr.Instance.getRangeInt(0, 100) <= 30)
                {
                    _gameFlow.reDealHandPoker(PlayPokerMgr.Instance.getPlayers(), true,0);
                }
            }
        }
        else {
            refactoringGameObject.GetComponent<Refactoring>().setFactoringNum((int)obj[1]);
        }
        GameMessage.Instance.setHandleMessageComplete();
    }
    public void reHandPoker(params System.Object[] obj) {
        _gameFlow.reDealHandPoker(PlayPokerMgr.Instance.getPlayers(), false, (int)obj[0]);
    }
    
    public void clearHandPoker(params System.Object[] obj)
    {
        List<Transform> child = new List<Transform>();
        IUser user = (IUser)obj[0];
        if (user.isNpc())
        {
            for (int i = 0; i < npcPokers.childCount; i++)
            {
                child.Add(npcPokers.GetChild(i));
            }
        }
        else
        {
            for (int i = 0; i < userPokers.childCount; i++)
            {
                child.Add(userPokers.GetChild(i));
            }
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
        
        PokerPileMgr.Instance.shuffle();
        HandPokerMgr.Instance.resetHandPoker();
        PlayPokerMgr.Instance.startPlayPoker();
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
}
