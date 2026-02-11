using UnityEngine;
using UnityEngine.UI;

public class BarrierView : MonoBehaviour,IBaseView
{
    public GameObject money;
    public GameObject diamond;
    public GameObject hp;
    public GameObject magic;
    public GameObject attack;
    public GameObject defense;

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

    public void onDealPokerClick() {
        
    }

    public void onReDealPokerClick1() { 

    }

    public void onReDealPokerClick2() { 

    }

    public void onStopPokerClick() {
        
    }

    public void onCardClick()
    {
        
    }

    public void onSureClick() { 

    }

    public void onSearchClick() { 

    }

    public void onPopClick() {
        UIMgr.Instance.showView("PopView");
    }
}
