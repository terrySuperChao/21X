using UnityEngine;
using UnityEngine.UI;

public class LanguageCom : MonoBehaviour
{
    public string textkey;
    void Start()
    {
        Text text = this.GetComponent<Text>();
        if (text != null) {
            text.text = LangMgr.Instance.getText(this.textkey);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

}   
