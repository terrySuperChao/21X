using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectPartView : MonoBehaviour, IBaseView
{
    public Transform content;
    public Button onBtn;
    public Button cancelBtn;
    private IPart _part;
    private Vector3 _position = new Vector3(0,0,0);
    private ICandidacyPartPara _para;

    void Start()
    {
        EventDispatcher.Instance.on(GameConst.SELECTPART, this.selectPart);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.SELECTPART, this.selectPart);
    }
    private void initParts()
    {
        for (int i = 0; i < this._para.getParts().Count; i++) {
            this.content.GetChild(i).gameObject.SetActive(true);
            this.content.GetChild(i).GetComponent<SelectPart>().loadPart(this._para.getParts()[i]);
        }
        onBtn.interactable = false;
    }

    public void cancelClick() {
        GameReqMgr.Instance.requestUpgradePart(false, this._para.getTriggerId(), this._para.getUser(), this._part, this._position);
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    public void okClick() {
        GameReqMgr.Instance.requestUpgradePart(true, this._para.getTriggerId(),this._para.getUser(), this._part, this._position);
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    private void selectPart(params System.Object[] obj)
    {
        ISelectPartPara selecPart = (SelectPartPara)obj[0];
        _part = selecPart.getPart();
        _position = selecPart.getPosition();
        onBtn.interactable = true;
    }

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

    public void setAlert(object content, Action okAction, Action cancelAction)
    {
        this._para = (ICandidacyPartPara)content;
        this.initParts();
    }
}
