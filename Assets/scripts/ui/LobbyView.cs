using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour,IBaseView
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
}
