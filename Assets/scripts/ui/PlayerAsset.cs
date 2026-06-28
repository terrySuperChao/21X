using System;
using System.Collections;
using System.Collections.Generic;
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
    public Transform buffs;

    public GameObject pokerPrefab;
    public GameObject cartPartPrefab;
    public GameObject buffPartPrefab;
    public GameObject cardPartPopup;
    public GameObject buffPartPopup;

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
        EventDispatcher.Instance.on(GameConst.ADDBUFFTYPE, this.addBuffType);
        EventDispatcher.Instance.on(GameConst.REMOVEBUFFTYPE, this.removeBuffType);
        EventDispatcher.Instance.on(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.on(GameConst.COMMONATTACK, this.commonAttack);
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
        EventDispatcher.Instance.off(GameConst.ADDBUFFTYPE, this.addBuffType);
        EventDispatcher.Instance.off(GameConst.REMOVEBUFFTYPE, this.removeBuffType);
        EventDispatcher.Instance.off(GameConst.CLEARHANDPOKER, this.clearHandPoker);
        EventDispatcher.Instance.off(GameConst.COMMONATTACK, this.commonAttack);
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
        this.initBuffs();
        this.initAdjust();
        this.initUserValue();
        this.showUserState();
    }

    private void initUserValue()
    {
        for (ValueType i = ValueType.blood; i < ValueType.maxMagic; i++)
        {
            this.setUserInfo(i);
        }
    }

    private void initPokers() {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(this._user);
        foreach (var poker in pokers){
            this.addPoker(new DealPokerPara(this._user, poker,0));
        }
    }

    private void initCards() {
        List<IAssembleCard> cards = FightPokerMgr.Instance.getUserAssembleCards(this._user);
        foreach (var card in cards){
            this.addCard(card);
        }
    }

    private void initBuffs() {
        List<BaseEffectType> buffs = FightPokerMgr.Instance.getUserBuff(this._user);
        foreach (var buff in buffs)
        {
            this.addBuff(buff);
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
    public void addCard(IAssembleCard card){
        Transform cardChild = this.cards.GetChild(0); //
        GameObject cardObject = Instantiate(this.cartPartPrefab, this.cards);
        cardObject.GetComponent<CardPart>().loadPartImage(card.getTrigger());
        cardObject.GetComponent<CardPart>().setAssembleCard(card);
        cardObject.GetComponent<CardPart>().setUser(this._user);
        cardObject.transform.position = cardChild.position;
        cardObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        cardChild.SetParent(null);
        Destroy(cardChild.gameObject);

        HoverCardPopup hover = cardObject.AddComponent<HoverCardPopup>();
        hover.popup = this.cardPartPopup.GetComponent<RectTransform>();
    }

    private void addBuff(BaseEffectType buffType) {
        GameObject buffObject = Instantiate(this.buffPartPrefab, this.buffs);
        buffObject.GetComponent<BuffPart>().setUser(this._user);
        buffObject.GetComponent<BuffPart>().setBuffType(buffType);
        
        HoverBuffPopup hover = buffObject.AddComponent<HoverBuffPopup>();
        hover.popup = this.buffPartPopup.GetComponent<RectTransform>();
    }

    private void removeBuff(BaseEffectType buffType) {
        for (int i = 0; i < this.buffs.childCount; i++) {
            if (this.buffs.GetChild(i).GetComponent<BuffPart>().getBuffType() == buffType) {
                GameObject buffObject = this.buffs.GetChild(i).gameObject;
                HoverBuffPopup hover = buffObject.AddComponent<HoverBuffPopup>();
                hover.popup = null;
                Destroy(buffObject);
                break;
            }
        }
    }
    

    // ------》》》》 1发卡牌
    public void dealCard(params System.Object[] obj)
    {
        /*
        IDealCardPara para = (IDealCardPara)obj[0];
        if (para.getUser() == this._user) {
            this.addCard(para.getCard());
        }*/
    }

    // ------》》》》 1.1 选择卡牌

    // ------》》》》 2发扑克牌
    private void dealPoker(params System.Object[] obj)
    {
        IDealPokerPara para = (IDealPokerPara)obj[0];
        if (para.getUser() == this._user){
            StartCoroutine(dealPokerHandle(para));
        }
    }

    private IEnumerator dealPokerHandle(IDealPokerPara para)
    {
        this.addPoker(para);
        yield return new WaitForSeconds(0.5f);
        this.showUserState(para.getPoint());
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

    public void showUserState(int currentPoint = 0) {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(this._user);
        int index = pokers.FindIndex(poker => poker.isBack());
        bool isBlackJack = FightPokerMgr.Instance.isUserHandPokerBlackJack(this._user);
        int point = currentPoint > 0 ? currentPoint :FightPokerMgr.Instance.getUserHandPokerPoint(this._user, index != -1);

        if (isBlackJack)
        {
            this.tipsText.text = "BlackJack";
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

    public Vector3 getCardPosition(IAssembleCard card)
    {
        int index = this.getCardForTypeIndex(card);
        return this.cards.GetChild(index).position;
    }

    private int getCardForTypeIndex(IAssembleCard card) {
        int index = 0;
        for (int i = 0; i < this.cards.childCount; i++)
        {
            CardPart cardComp = this.cards.GetChild(i).GetComponent<CardPart>();
            if (cardComp != null && cardComp.getAssembleCard().getTriggerId() == card.getTriggerId())
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

    private Transform getCardIdTransform(IAssembleCard card)
    {
        if (card == null) {
            return null;
        }
        for (int i = 0; i < this.cards.childCount; i++)
        {
            CardPart cardComp = this.cards.GetChild(i).GetComponent<CardPart>();
            if (cardComp != null && cardComp.getAssembleCard().getTriggerId() == card.getTriggerId())
            {
                return this.cards.GetChild(i);
            }
        }
        return null;
    }

    private Transform getBuffTransform(BaseEffectType buffType)
    {
        for (int i = 0; i < this.buffs.childCount; i++)
        {
            BuffPart buffComp = this.buffs.GetChild(i).GetComponent<BuffPart>();
            if (buffComp != null && buffComp.getBuffType() == buffType)
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
        List<IPoker> pokers = para.getPokers();
        if (pokers.Count == 0) {
            yield return 0;
        }

        for (int i = 0; i < pokers.Count; i++) {
            Transform pokerChild = this.getPokerIdTransform(pokers[i]);
            if (pokerChild != null){
                iTween.ScaleTo(pokerChild.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.5f);   
            }
        }

        yield return new WaitForSeconds(0.51f);

        for (int i = 0; i < pokers.Count; i++)
        {
            Transform pokerChild = this.getPokerIdTransform(pokers[i]);
            if (pokerChild != null){
                iTween.ScaleTo(pokerChild.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 0.1f);
            }
        }

        PokerSuit suit = pokers[0].getSuit();
        ValueType valueType = GameUtils.SuitTransformValueType(suit);
        int indexType = (int)valueType;
        if (indexType < this._texts.Count)
        {
            Text textChild = this._texts[indexType];
            float addValue = GameUtils.getNumberDigits(para.getValue());
            Text addText = Instantiate(textChild, rootTransform);
            addText.transform.position = textChild.transform.position;
            addText.text = "+" + addValue;
            addText.color = Color.green;

            Vector3 localPos = addText.transform.localPosition;
            moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + 50, localPos.z));
            yield return new WaitForSeconds(0.51f);
            Destroy(addText.gameObject);
            textChild.text = this.getFinalContent(valueType, para.getFinalValue());
        }
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

        float addValue = GameUtils.getNumberDigits(para.getValue());
        bool bl = addValue > 0;

        Text addText = Instantiate(text, rootTransform);
        addText.transform.position = text.transform.position;
        addText.text = ((bl ? "+" : "") + addValue);
        addText.color = (bl ? Color.green : Color.red);
        Vector3 localPos = addText.transform.localPosition;
        this.moveTo(addText.gameObject, new Vector3(localPos.x, localPos.y + ((bl ? 1 : -1) * 50.0f), localPos.z));
        
        yield return new WaitForSeconds(0.51f);
        Destroy(addText.gameObject);
        text.text = getFinalContent(para.getValueType(), para.getFinalValue());
    }

    public void addBuffType(params System.Object[] obj)
    {
        IUIBuffPara para = (IUIBuffPara)obj[0];
        if (para.getUser() == this._user)
        {
            this.addBuff(para.getBuffType());
        }
    }

    public void removeBuffType(params System.Object[] obj) {
        IUIBuffPara para = (IUIBuffPara)obj[0];
        if (para.getUser() == this._user)
        {
            this.removeBuff(para.getBuffType());
        }
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
            value = this._user.getMagic() + "/" + this._user.getMaxMagic();
                break;
            default:
                break;
        }

        Text text = this._texts[(int)(type)];
        if (text != null) {
            text.text = value;
        }
    }

    private string getFinalContent(ValueType type, float finalValue)
    {
        float maxValue = -1;
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
            case ValueType.maxMagic: // 梅
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
        this.pointText.text = "0";
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
        Transform child = this.getCardIdTransform(para.getAssembleCard());
        if (child == null) {
            child = this.getBuffTransform(para.getBuffType());
        }
        if (child == null){
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

    public void commonAttack(params System.Object[] obj)
    {
        StartCoroutine(commonAttackHandle(obj));
        
    }

    private IEnumerator commonAttackHandle(params System.Object[] obj)
    {
        yield return new WaitForSeconds(1.1f);
        IUICommonPara para = (IUICommonPara)obj[0];
        int indexType = (int)para.getValueType();
        if (this._texts.Count <= indexType || indexType < 0)
        {
            yield return 0;
        }
        Text textChild = this._texts[indexType];
        textChild.text = getFinalContent(para.getValueType(), para.getFinalValue());
    }
}
