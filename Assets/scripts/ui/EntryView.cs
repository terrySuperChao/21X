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
        UIMgr.Instance.showView("stateView");
    }

    public void onFightClick() {
        EventDispatcher.Instance.emit("startGame", GameMode.Fight);
    }

    public void onCardClick()
    {
        EventDispatcher.Instance.emit("startGame", GameMode.Card);
    }

    public void onCloseClick() { 
        
    }
}
