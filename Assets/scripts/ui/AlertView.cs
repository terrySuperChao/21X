using System;
using UnityEngine;
using UnityEngine.UI;

public class AlertView : MonoBehaviour, IBaseView
{
    public GameObject content;
    private Action _okAction;
    private Action _cancelAction;
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

    public void setAlert(string content, Action okAction, Action cancelAction) { 
        
        this.content.GetComponent<Text>().text = content;
        this._okAction = okAction;
        this._cancelAction = cancelAction;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onCloseClick() {
        if (this._cancelAction != null)
        {
            this._cancelAction();
        }
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    public void onOkClick() {
        if (this._okAction != null) { 
            this._okAction();
        }
        UIMgr.Instance.closeView(this.gameObject.name);
    }
}
