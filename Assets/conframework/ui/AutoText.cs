using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class AutoText : MonoBehaviour
{
    public int maxWidth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setText(string str)
    {
        Text text = this.gameObject.GetComponent<Text>();
        text.text = str;

        float width = text.preferredWidth < maxWidth ? text.preferredWidth : maxWidth;
        text.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, text.preferredHeight);
    }
}
