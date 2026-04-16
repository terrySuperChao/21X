using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Text cardName;
    public Text cardDesc;
    public GameObject starImage;
    public GameObject selectImage;

    private ICard _card;
    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.SELECTCARD, this.selectCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.SELECTCARD, this.selectCard);
    }

    public void loadCard(ICard card) {
        _card = card;
        selectImage.SetActive(false);
        cardDesc.text = card.getDescript();
        cardName.text = card.getName();
        starImage.SetActive(card.getLevel() == 2);
    }

    public void onClick() {
        EventDispatcher.Instance.emit(GameConst.SELECTCARD, new SelectCardPara(null,_card,this.transform.position));
    }

    private void selectCard(params System.Object[] obj)
    {
        ISelectCardPara para = (ISelectCardPara)obj[0];
        selectImage.SetActive(_card == para.getCard());
    }

    public bool isSelected() {
        return selectImage.activeSelf;
    }

    public ICard getCard() {
        return _card;
    }

    public void showNameText(bool active) {
        cardName.gameObject.SetActive(active);
    }
}   
