using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardPart : MonoBehaviour
{
    public GameObject starImage;
    public Button button;

    private ICard _card;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void onClick() {
        EventDispatcher.Instance.emit(GameConst.IMPRINT_SELECT_PART,this.gameObject);
    }

    public void setBtnEnable() {
        this.button.enabled = false;
    }
}   
