using System;
using UnityEngine;
using UnityEngine.UI;

public class PopView : MonoBehaviour, IBaseView
{
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onContinueClick() {
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    public void onSettingClick() {
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    public void onHomeClick() {
        GameDataMgr.Instance.setGameState(GameState.idle);
        GamePropertyMgr.Instance.save();
        UIMgr.Instance.closeView(this.gameObject.name);
        UIMgr.Instance.showView("EntryView");
    }

    public void onSaveClick() {
        UIMgr.Instance.closeView(this.gameObject.name);
    }
}
