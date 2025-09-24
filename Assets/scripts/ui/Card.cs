using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        EventDispatcher.Instance.on(GameConst.SELECTCARD, this.selectCard);
    }

    public void loadCard(ICard card) {
        _card = card;
        selectImage.SetActive(false);
        cardDesc.text = card.getDescript();
        cardName.text = card.getName();
        starImage.SetActive(card.getLevel() == 2);
    }

    public void onClick() {
        EventDispatcher.Instance.emit(GameConst.SELECTCARD, _card,this.transform.position);
    }

    private void selectCard(params System.Object[] obj)
    {
        selectImage.SetActive(_card == obj[0]);
    }

    public bool isSelected() {
        return selectImage.activeSelf;
    }

    public ICard getCard() {
        return _card;
    }
}   
