using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardView : MonoBehaviour, IBaseView
{
    public Transform content;
    public Button onBtn;
    public Button cancelBtn;
    private ICard _card;
    private IUser _user;
    private Vector3 _position = new Vector3(0,0,0);

    void Start()
    {
        EventDispatcher.Instance.on(GameConst.SELECTCARD, this.selectCard);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.SELECTCARD, this.selectCard);
    }
    private void initCards(List<ICard> cards)
    {

        for (int i = 0; i < 3; i++)
        {
            content.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < cards.Count; i++) {
            content.GetChild(i).gameObject.SetActive(true);
            content.GetChild(i).GetComponent<Card>().loadCard(cards[i]);
        }
        onBtn.interactable = false;
    }

    public void cancelClick() {
        GameReqMgr.Instance.requestAddCard(false,_user, _card, _position);
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    public void okClick() {
        List<ICard> list = FightPokerMgr.Instance.getUserCards(_user);
        int index = list.FindIndex(card => card.getType() == this._card.getType());
        if (index != -1 && list.Count == 3)
        {
            UIMgr.Instance.showTips("TipsView", "¿¨²ÛÒÑÂú");
        }
        else {
            GameReqMgr.Instance.requestAddCard(true,_user, _card, _position);
            UIMgr.Instance.closeView(this.gameObject.name);
        }
    }

    private void selectCard(params System.Object[] obj)
    {
        SelectCardPara selecCard = (SelectCardPara)obj[0];
        _card = selecCard.getCard();
        _position = selecCard.getPosition();
        onBtn.interactable = true;
    }

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

    }

    public void setAlert(object content, Action okAction, Action cancelAction)
    {
        ICandidacyCardPara para = (ICandidacyCardPara)content;
        this._user = para.getUser();
        this.initCards(para.getCards());
    }
}
