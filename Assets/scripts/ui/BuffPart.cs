using UnityEngine;
using UnityEngine.UI;

public class BuffPart : MonoBehaviour
{
    public Text partName;
    public GameObject partImage;
    private string _buff;
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

    public void setBuff(string buff) {
        this._buff = buff;
    }

    public string getBuff() {
        return this._buff;
    }
}   
