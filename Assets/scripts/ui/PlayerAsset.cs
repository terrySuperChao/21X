using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAsset : MonoBehaviour
{
    public Text bloodText;
    public Text attackText;
    public Text defenseText;
    public Text magicText;
    public Text pointText;
    public Text winsText;
    public Text winRateText;
    public Text tipsText;

    public GameObject tipsPanel;
    public GameObject headImage;

    public Transform pokers;
    public Transform cards;

    public GameObject pokerPrefab;
    public GameObject cartPartPrefab;
    public GameObject cardPartPopup;

    public Transform rootTransform;
    private List<Text> _texts = new List<Text>();
    private IUser _user;
    private Vector3 _pokerPos;

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.on(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.on(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.on(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.on(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.on(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.on(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.on(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.on(GameConst.GAMECLEAR, this.gameClear);

        this._texts.Add(this.winRateText);
        this._texts.Add(this.bloodText);
        this._texts.Add(this.attackText);
        this._texts.Add(this.defenseText);
        this._texts.Add(this.magicText);
        this._texts.Add(this.pointText);
        this._texts.Add(this.winRateText);
        this.tipsPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.DEALCARD, this.dealCard);
        EventDispatcher.Instance.off(GameConst.DEALPOKER, this.dealPoker);
        EventDispatcher.Instance.off(GameConst.STOPDEALPOKER, this.stopDealPoker);
        EventDispatcher.Instance.off(GameConst.TOTALPOKERPOINT, this.totalPokerPoint);
        EventDispatcher.Instance.off(GameConst.ADDPOKERVALUE, this.addPokerValue);
        EventDispatcher.Instance.off(GameConst.ADDCARDVALUE, this.addCardValue);
        EventDispatcher.Instance.off(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.off(GameConst.FLYFONT, this.flyFont);
        EventDispatcher.Instance.off(GameConst.GAMECLEAR, this.gameClear);
    }

    // Update is called once per frame


    public void initUserInfo(IUser user,Vector3 pokerPos)
    {
        this._user = user;
        this._pokerPos = pokerPos;
        this.initPokers();
        this.initCards();
        this.initAdjust();
        this.initUserValue();
        this.showUserState();
    }

    private void initUserValue()
    {
        for (ValueType i = ValueType.blood; i < ValueType.winRate; i++)
        {
            this.setUserInfo(i);
        }
    }

    private void initPokers() {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(this._user);
        foreach (var poker in pokers){
            this.addPoker(new DealPokerPara(this._user, poker));
        }
    }

    private void initCards() {
        List<ICard> cards = FightPokerMgr.Instance.getUserCards(this._user);
        foreach (var card in cards){
            this.addCard(card);
        }
    }

    private void initAdjust() {
        if (this._user.isNpc()) return;
        for (int i = 0; i < this.rootTransform.childCount; i++) {
            Transform item = this.rootTransform.GetChild(i);
            item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y * -1, item.transform.localPosition.z);
        }
    }

    private void addPoker(IDealPokerPara para) {
        GameObject pokerObject = Instantiate(this.pokerPrefab, this.pokers);
        pokerObject.GetComponent<Poker>().loadPokerRes(para.getPoker());
        pokerObject.transform.position = this._pokerPos;

        Vector3 pos = new Vector3(0, 0, 0);
        float count = this.pokers.childCount;
        
        float index = count - 1;
        float scalex = pokerObject.transform.localScale.x;
        float width = pokerObject.GetComponent<RectTransform>().rect.width * scalex;
        float maxWidth = this.pokers.gameObject.GetComponent<RectTransform>().rect.width;
        float offX = count <= 1 ? 120 : Math.Min((maxWidth - width * count) / (count - 1), 120);
        float startX = pos.x - index * (width * scalex + offX) / 2;
        
        for (int i = 0; i < count; i++)
        {
            Vector3 localPos = new Vector3(startX + (width * scalex + offX) * i, pos.y, pos.z);
            moveTo(this.pokers.GetChild(i).gameObject, localPos);
        }
    }
    public void addCard(ICard card){
        IPart part = GameStaticConfigMgr.Instance.getTriggerPartConfig().getTriggerPartId(card.getId());
        int index = getCardForTypeIndex(card);
        Transform cardChild = this.cards.GetChild(index); //
        GameObject cardObject = Instantiate(this.cartPartPrefab, this.cards);
        cardObject.GetComponent<CardPart>().loadPartImage(part);
        cardObject.GetComponent<CardPart>().setCard(card);
        cardObject.transform.position = cardChild.position;
        cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        cardChild.SetParent(null);
        Destroy(cardChild.gameObject);

        HoverPopup hover = cardObject.AddComponent<HoverPopup>();
        hover.popup = this.cardPartPopup.GetComponent<RectTransform>();
    }

    // ------》》》》 1发卡牌
    public void dealCard(params System.Object[] obj)
    {
        IDealCardPara para = (IDealCardPara)obj[0];
        if (para.getUser() == this._user) {
            this.addCard(para.getCard());
        }
    }

    // ------》》》》 1.1 选择卡牌

    // ------》》》》 2发扑克牌
    private void dealPoker(params System.Object[] obj)
    {
        IDealPokerPara para = (IDealPokerPara)obj[0];
        if (para.getUser() == this._user){
            this.addPoker(para);
            this.showUserState();
        }
    }

    // ------》》》》 3轮到操作者

    // ------》》》》 4 等待操作

    // ------》》》》 5 停牌
    private void stopDealPoker(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        if (user == this._user)
        {
            this.showUserState();
        }
    }


    // ------》》》》 6 统计点数
    private void totalPokerPoint(params System.Object[] obj)
    {
        ITotalHandPokerPointPara para = (ITotalHandPokerPointPara)obj[0];
        if (para.getUser() == this._user)
        {
            this.showUserState();
        }
    }

    public void showUserState() {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(this._user);
        int index = pokers.FindIndex(poker => poker.isBack());
        int point = FightPokerMgr.Instance.getUserHandPokerPoint(this._user, index != -1);
        bool isBack = FightPokerMgr.Instance.isUserHandPokerBlackJack(this._user);

        if (isBack)
        {
            this.tipsText.text = "Blackack";
            this.tipsText.color = new Color(255, 223, 0);
            this.tipsPanel.SetActive(true);

        }

       if (point > 21)
       {
            this.tipsText.text = "爆牌";
            this.tipsText.color = Color.red;
            this.tipsPanel.SetActive(true);
        }
         
        if (this._user.getState() == UserState.end)
        {
            this.tipsText.text = "停牌";
            this.tipsText.color = Color.red;
            this.tipsPanel.SetActive(true);
        }

        if (point < 21)
        {
            this.pointText.color = Color.black;
        }
        else if (point == 21)
        {
            this.pointText.color = new Color(255, 223, 0);
        }
        else
        {
            this.pointText.color = Color.red;
        }
        this.pointText.text = point.ToString();
    }

    public Vector3 getCardPosition(ICard card)
    {
        int index = this.getCardForTypeIndex(card);
        return this.cards.GetChild(index).position;
    }

    private int getCardForTypeIndex(ICard card) {
        int index = 0;
        for (int i = 0; i < this.cards.childCount; i++)
        {
            CardPart cardComp = this.cards.GetChild(i).GetComponent<CardPart>();
            if (cardComp != null && cardComp.getCard().getType() == card.getType())
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private Transform getPokerIdTransform(IPoker poker)
    {
        for (int i = 0; i < this.pokers.childCount; i++)
        {
            Poker pokerComp = this.pokers.GetChild(i).GetComponent<Poker>();
            if (pokerComp != null && pokerComp.getPoker().getId() == poker.getId())
            {
                return this.pokers.GetChild(i);
            }
        }
        return null;
    }

    private Transform getCardIdTransform(ICard card)
    {
        for (int i = 0; i < this.cards.childCount; i++)
        {
            Card cardComp = this.cards.GetChild(i).GetComponent<Card>();
            if (cardComp != null && cardComp.getCard().getId() == card.getId())
            {
                return this.cards.GetChild(i);
            }
        }
        return null;
    }

    public void addPokerValue(params System.Object[] obj)
    {
        IUIPokerPara para = (IUIPokerPara)obj[0];
        if (para.getUser() == this._user) { 
            StartCoroutine(addPokerValueHandle(obj));
        }
    }
    private IEnumerator addPokerValueHandle(params System.Object[] obj)
    {
        IUIPokerPara para = (IUIPokerPara)obj[0];
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        ValueType type = GameConst.SuitTransformValueType(suit);
        Text textChild = this._texts[(int)type];
        Transform pokerChild = this.getPokerIdTransform(para.getPoker());
        if (pokerChild == null || textChild == null)
        {
            yield return 0;
        }

        Text addText = Instantiate(textChild, rootTransform);
        addText.transform.position = textChild.transform.position;
        addText.text = para.getText();
        addText.color = Color.green;

        Vector3 localPos = addText.transform.localPosition;
        moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + 50, localPos.z));
        iTween.ScaleTo(pokerChild.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
        yield return new WaitForSeconds(0.51f);
        iTween.ScaleTo(pokerChild.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 0.1f);
        Destroy(addText.gameObject);
        textChild.text = this.getFinalContent(type, para.getFinalValue());
    }

    public void addCardValue(params System.Object[] obj)
    {
        IUICommonPara para = (IUICommonPara)obj[0];
        if (para.getUser() == this._user)
        {
            StartCoroutine(addCardValueHandle(obj));
        }
    }

    private IEnumerator addCardValueHandle(params System.Object[] obj)
    {
        IUICommonPara para = (IUICommonPara)obj[0];
        Text text = this._texts[(int)para.getValueType()];
        if (text == null){
            yield return 0;
        }

        bool bl = para.getValue() > 0;

        Text addText = Instantiate(text, rootTransform);
        addText.transform.position = text.transform.position;
        addText.text = ((bl ? "+" : "-") + para.getValue());
        addText.color = (bl ? Color.green : Color.red);
        Vector3 localPos = addText.transform.localPosition;
        this.moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + ((bl ? 1 : -1) * 50.0f), localPos.z));
        
        yield return new WaitForSeconds(0.51f);
        Destroy(addText.gameObject);
        text.text = getFinalContent(para.getValueType(), para.getFinalValue());
    }

    private void setUserInfo(ValueType type) {
        string value = "";
        switch (type)
        {
            case ValueType.defense: // 方
            value = this._user.getDefense().ToString();
                break;
            case ValueType.blood: // 红
            value = this._user.getBlood() + "/" + this._user.getMaxBlood();
                break;
            case ValueType.attack: // 黑
            value = this._user.getAttack().ToString();
                break;
            case ValueType.magic: // 梅
            value = this._user.getMagic() + "/ " + this._user.getMaxMagic();
                break;
            case ValueType.winRate: //赢率
            value = string.Format("{0:P1}", this._user.getWinRate());
                break;
            default:
                break;
        }

        Text text = this._texts[(int)(type)];
        if (text != null) {
            text.text = value;
        }
    }

    private string getFinalContent(ValueType type, float value)
    {
        float maxValue = -1;
        float finalValue = (float)Math.Round((value * 10 + 0.5) / 10, 1);
        switch (type)
        {
            case ValueType.defense: // 方
                break;
            case ValueType.blood: // 红
                maxValue = this._user.getMaxBlood();
                break;
            case ValueType.attack: // 黑
                break;
            case ValueType.magic: // 梅
                maxValue = this._user.getMaxMagic();
                break;
            default:
                break;
        }
        if (maxValue == -1){
            return finalValue.ToString();
        }else {
            return finalValue + "/" + maxValue;
        }
    }

    public void clearHandPoker(params System.Object[] obj)
    {
        IUser user = (IUser)obj[0];
        if (user == this._user)
        {
            while (this.pokers.childCount > 0)
            {
                Transform child = this.pokers.GetChild(0);
                child.SetParent(null); //
                GameObject.Destroy(child.gameObject); // 销毁子对象
            }
        }
    }

    public void gameClear(params System.Object[] obj)
    {
        StartCoroutine(gameClearHandle(obj));
    }
    private IEnumerator gameClearHandle(params System.Object[] obj) {
        yield return new WaitForSeconds(0.5f);
        this.tipsPanel.SetActive(false);
        this.tipsText.text = "爆牌！！";
        this.tipsText.color = Color.red;
        this.pointText.color = Color.white;
        this.initUserValue();

        for (int i = 0; i < this.pokers.childCount; i++)
        {
            this.pokers.GetChild(i).gameObject.GetComponent<Poker>().loadBackPoker();
            iTween.MoveTo(this.pokers.GetChild(i).gameObject, this._pokerPos, 0.5f);
            
        }
        yield return new WaitForSeconds(1.0f);

        while (this.pokers.childCount > 0)
        {
            Transform child = this.pokers.GetChild(0);
            child.SetParent(null); //
            GameObject.Destroy(child.gameObject); // 销毁子对象
        }
    }

    public void flyFont(params System.Object[] obj)
    {
        IUIFlyFontPara para = (IUIFlyFontPara)obj[0];
        if (para.getUser() == this._user) { 
            StartCoroutine(flyFontHandle(obj));
        }
    }

    private IEnumerator flyFontHandle(params System.Object[] obj)
    {
        IUIFlyFontPara para = (IUIFlyFontPara)obj[0];
        Transform child = getCardIdTransform(para.getCard());
        if (child != null)
        {
            yield return 0;
        }

        Debug.Log("文字====" + para.getText());
        Text addText = Instantiate(this.attackText, rootTransform);
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

    public Vector3 getHeadPosition() {
        return this.headImage.transform.position;
    }

    private void moveTo(GameObject gameObject, Vector3 position) {
        iTween.MoveTo(gameObject, iTween.Hash("position", position, "time", 0.5f, "isLocal", true, "easeType", iTween.EaseType.linear));
    }
}
