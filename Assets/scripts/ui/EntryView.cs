using System;
using UnityEngine;
using UnityEngine.UI;

public class EntryView : MonoBehaviour,IBaseView
{
    public GameObject version;
    public GameObject continueGame;
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
        if (GameDataMgr.Instance.getGameState() == GameState.idle) {
            if (GameDataMgr.Instance.getPageIndex() == PageIndex.EntryView) {
                this.continueGame.SetActive(false);
            }
        }
        this.version.GetComponent<Text>().text = GameVersion.version;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onNewGameClick() {
        string pageName = Enum.GetName(typeof(PageIndex), PageIndex.LobbyView);
        UIMgr.Instance.showView(pageName);
    }

    public void onContinueClick() {
        GameDataMgr.Instance.setGameState(GameState.playing);
        GamePropertyMgr.Instance.save();

        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    public void onSettingViewClick() {
        UIMgr.Instance.showView("SettingView");
    }
}
