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
        if (GamePropertyMgr.Instance.getGameData().GameState == (int)GameState.idle) {
            if (GamePropertyMgr.Instance.getGameData().PageIndex == (int)PageIndex.EntryView) {
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
        UIMgr.Instance.showView("LobbyView");
    }

    public void onContinueClick() {
        GamePropertyMgr.Instance.getGameData().GameState = (int)GameState.playing;
        GamePropertyMgr.Instance.save();

        PageIndex pageIndex = (PageIndex)GamePropertyMgr.Instance.getGameData().PageIndex;
        if (pageIndex == PageIndex.LobbyView)
        {
            UIMgr.Instance.showView("LobbyView");
        }
        else if (pageIndex == PageIndex.BarrierView)
        {
            UIMgr.Instance.showView("BarrierView");
        }
    }
}
