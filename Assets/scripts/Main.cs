using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("repeatHandleMessage", 0.0f, 0.1f);
        LangMgr.Instance.init("config/language");
        GamePropertyMgr.Instance.init();
        GameStaticConfigMgr.Instance.init();
        UIMgr.Instance.init(this.gameObject, "config/UIConfig");
        UIMgr.Instance.showView("LoadingView");
        PokerPileMgr.Instance.init();
        HandPokerMgr.Instance.init();
    }

    private void repeatHandleMessage()
    {
        GameMessage.Instance.handleMessage();
    }
}
