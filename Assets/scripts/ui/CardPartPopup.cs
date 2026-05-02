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
        if (partInfo == null)
            return;

        string desc = partInfo.getDesc();
        if (desc.IndexOf("%s") == -1)
        {
            this.partName.text = desc;
        }
        else {
            this.partName.text = desc.Replace("%s", partInfo.getValueDefault().ToString());
        }
    }
}   
