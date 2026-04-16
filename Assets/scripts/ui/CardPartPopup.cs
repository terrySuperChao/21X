using UnityEngine;
using UnityEngine.UI;

public class CardPartPopup : MonoBehaviour
{
    public Text partName;
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

    public void loadPartInfo(IPart partInfo) {
        this.partName.text = partInfo.getDesc();
    }
}   
