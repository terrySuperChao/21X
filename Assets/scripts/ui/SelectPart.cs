using UnityEngine;
using UnityEngine.UI;

public class SelectPart : MonoBehaviour
{
    public Text partName;
    public GameObject selectImage;

    private IPart _part;
    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.SELECTPART, this.selectPart);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.SELECTPART, this.selectPart);
    }

    public void loadPart(IPart part)
    {
        this._part = part;
        this.selectImage.SetActive(false);
        this.partName.text = part.getName();
    }

    public void onClick()
    {
        EventDispatcher.Instance.emit(GameConst.SELECTPART, new SelectPartPara(null, this._part, this.transform.position));
    }

    private void selectPart(params System.Object[] obj)
    {
        ISelectCardPara para = (ISelectCardPara)obj[0];
        selectImage.SetActive(this._part == para.getCard());
    }

    public bool isSelected()
    {
        return selectImage.activeSelf;
    }

    public IPart getPart()
    {
        return this._part;
    }

    public void showNameText(bool active)
    {
        this.partName.gameObject.SetActive(active);
    }
}
