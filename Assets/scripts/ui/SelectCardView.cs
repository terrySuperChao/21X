using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardView : MonoBehaviour
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

    public void initCards(IUser user,List<ICard> cards)
    {
        _user = user;
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
        EventDispatcher.Instance.emit(GameConst.SELECTFINSIHCARD, _user, null, _position);
    }

    public void okClick() {
        EventDispatcher.Instance.emit(GameConst.SELECTFINSIHCARD, _user, _card, _position);
    }

    private void selectCard(params System.Object[] obj)
    {
        _card = (ICard)obj[0];
        _position = (Vector3)obj[1];
        onBtn.interactable = true;
    }
}
