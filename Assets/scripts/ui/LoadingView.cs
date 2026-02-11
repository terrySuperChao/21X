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
        if (GamePropertyMgr.Instance.getGameData().GameState == (int)GameState.idle)
        {
            UIMgr.Instance.showView("EntryView");
        }
        else {
            PageIndex pageIndex = (PageIndex)GamePropertyMgr.Instance.getGameData().PageIndex;
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
