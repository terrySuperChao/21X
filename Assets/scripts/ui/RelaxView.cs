using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class RelaxView : MonoBehaviour, IBaseView
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
        
    }

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onRelaxClick()
    {
        GameDataMgr.Instance.setPageIndex(PageIndex.BarrierView);
        GamePropertyMgr.Instance.save();
        UIMgr.Instance.showView("BarrierView");
    }

    public void onTrainClick()
    {

    }

    public void onPopClick()
    {
        UIMgr.Instance.showView("PopView");
    }
}
