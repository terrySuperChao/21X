using TMPro;
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
        {
            return;
        }
            

        string desc = partInfo.getDesc();
        if (desc.IndexOf("%s") == -1){
            this.partName.text = desc;
        }else {
            this.partName.text = desc.Replace("%s", partInfo.getValueDefault().ToString());
        }
    }

    public void setAssembleCard(IAssembleCard card) {
        if (card == null) {
            return;
        }

        string str = "";
        IPart trigger = card.getTrigger();
        IPart baseEffect = card.getBaseEffect();
        IPart advanceEffect = card.getAdvancedEffect();
        if (trigger != null) {
            str += string.Format("{0}\n{1}\n", trigger.getName(), trigger.getDesc());
        }

        if (baseEffect != null) {
            str += string.Format("{0}\n{1}\n", baseEffect.getName(), baseEffect.getDesc());
        }

        if (advanceEffect != null)
        {
            str += string.Format("{0}\n{1}\n", advanceEffect.getName(), advanceEffect.getDesc());
        }

        this.partName.text = str;
    }
}   
