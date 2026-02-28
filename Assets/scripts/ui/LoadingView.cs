using System;
using System.Collections;
using System.Drawing;
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

        string pageName = "";
        if (GameDataMgr.Instance.getGameState() == GameState.idle)
        {
            pageName = Enum.GetName(typeof(PageIndex), PageIndex.EntryView);
        }
        else {
            pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        }
        UIMgr.Instance.showView(pageName);
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
