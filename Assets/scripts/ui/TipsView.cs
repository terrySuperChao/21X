
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TipsView : MonoBehaviour
{

    public Text text;
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

    public void setText(string str) {
        StartCoroutine(textAction(str));
    }

    private IEnumerator textAction(string str)
    {
        text.text = str;
        iTween.MoveBy(this.gameObject, new Vector3(0, 100, 0), 1f);
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}   
