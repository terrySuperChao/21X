using System.Collections;
using UnityEngine;

public class LoadingView : MonoBehaviour,IBaseView
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
        StartCoroutine(gotoLobbyView());
    }


    private IEnumerator gotoLobbyView()
    {
        yield return new WaitForSeconds(1.0f);
        if (GameDataMgr.Instance.getGameState() == GameState.idle)
        {
            UIMgr.Instance.showView("EntryView");
        }
        else {
            PageIndex pageIndex = GameDataMgr.Instance.getPageIndex();
            if (pageIndex == PageIndex.EntryView)
            {
                UIMgr.Instance.showView("EntryView");
            }
            else if (pageIndex == PageIndex.LobbyView)
            {
                UIMgr.Instance.showView("LobbyView");
            }
            else if (pageIndex == PageIndex.BarrierView)
            {
                UIMgr.Instance.showView("BarrierView");
            }
            else if (pageIndex == PageIndex.RelaxView) {
                UIMgr.Instance.showView("RelaxView");
            }
            else if (pageIndex == PageIndex.GameView)
            {
                UIMgr.Instance.showView("GameView");
            }
        }
    }

        // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
