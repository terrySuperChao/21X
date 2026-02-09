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
        UIMgr.Instance.showView("LobbyView");
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
