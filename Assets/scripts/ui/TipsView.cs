
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TipsView : MonoBehaviour,IBaseView
{

    public Text text;

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

    private void OnDestroy()
    {
        
    }

    public void setAlert(string content, Action okAction, Action cancelAction)
    {

        this.setText(content);
    }

    public void setText(string str) {
        StartCoroutine(textAction(str));
    }

    private IEnumerator textAction(string str)
    {
        text.text = str;
        iTween.MoveBy(this.gameObject, new Vector3(0, 300, 0), 1f);
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}   
