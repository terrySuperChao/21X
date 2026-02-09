using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GamePropertyMgr.Instance.init();
        UIMgr.Instance.init(this.gameObject, "config/UIConfig");
        LangMgr.Instance.init("config/language");
        UIMgr.Instance.showView("LoadingView");
        PokerPileMgr.Instance.init();
        HandPokerMgr.Instance.init();
    }
}
