using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectPartView : MonoBehaviour, IBaseView
{
    public Transform content;
    public Button onBtn;
    public Button refreshBtn;
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
        //选择第一个
        this.content.GetChild(0).GetComponent<SelectPart>().onClick();
    }

    public void refreshClick() {
        GameReqMgr.Instance.requestRefreshPart(this._para.getUser(),this._para.getAssembleCard());
    }

    public void okClick() {
        GameReqMgr.Instance.requestUpgradePart(this._para.getUser(),this._para.getAssembleCard(), this._part, this._position);
        UIMgr.Instance.closeView(this.gameObject.name);
    }

    private void selectPart(params System.Object[] obj)
    {
        ISelectPartPara selecPart = (ISelectPartPara)obj[0];
        this._part = selecPart.getPart();
        this._position = selecPart.getPosition();
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
        Invoke("initParts", 0.1f);
    }
}
