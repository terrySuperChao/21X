using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Refactoring : MonoBehaviour
{
    public Text resultText;
    public Transform btn1;
    public Transform btn2;
    public Transform btn3;
    public Transform btn4;
    public Transform btn5;
    public Button okBtn;
    public GameObject suitNode;
    private int _selectIndex = -1;
    private int _refactoringNum = 0;
    private IUser _user = null;
    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.HIDE_REFACTORING, this.runRefactoring);
        EventDispatcher.Instance.on(GameConst.HIDE_REFACTORING, this.hideRefactoring);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.HIDE_REFACTORING, this.runRefactoring);
        EventDispatcher.Instance.off(GameConst.HIDE_REFACTORING, this.hideRefactoring);
    }

    public void okClick() {
        if (--this._refactoringNum <= 0)
        {
            this.gameObject.SetActive(false);
        }
        else {
            StartCoroutine(delayAction());
        }
        resultText.text = this._refactoringNum.ToString();
        EventDispatcher.Instance.emit(GameConst.REHANDPOKER, new ReHandPokerPara(this._user,this._selectIndex));
    }

    private IEnumerator delayAction()
    {
        okBtn.interactable = false;
        yield return new WaitForSeconds(2.0f);
        okBtn.interactable = true;
    }

    public void selectClick1() {
        _selectIndex = 0;
        btn1.localScale = new Vector3(0.5f, 0.5f, 1f);
        btn2.localScale = btn3.localScale = btn4.localScale = btn5.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    public void selectClick2()
    {
        _selectIndex = 1;
        btn2.localScale = new Vector3(0.5f, 0.5f, 1f);
        btn1.localScale = btn3.localScale = btn4.localScale = btn5.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    public void selectClick3()
    {
        _selectIndex = 2;
        btn3.localScale = new Vector3(0.5f, 0.5f, 1f);
        btn1.localScale = btn2.localScale = btn4.localScale = btn5.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    public void selectClick4()
    {
        _selectIndex = 3;
        btn4.localScale = new Vector3(0.5f, 0.5f, 1f);
        btn1.localScale = btn2.localScale = btn3.localScale = btn5.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    public void selectClick5()
    {
        _selectIndex = 4;
        btn5.localScale = new Vector3(0.5f, 0.5f, 1f);
        btn1.localScale = btn2.localScale = btn3.localScale = btn4.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    private void runRefactoring(params System.Object[] obj)
    {
        IRefactoringPara para = (IRefactoringPara)obj[0];
        this._user = para.getUser();
        this._refactoringNum = para.getNumber();
        this.resultText.text = this._refactoringNum.ToString();
        this.gameObject.SetActive(true);
        this.suitNode.SetActive(this._refactoringNum == 2);
    }

    private void hideRefactoring(params System.Object[] obj) { 
        this.gameObject.SetActive(false);
    }
}
