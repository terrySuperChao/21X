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
        if (partInfo != null)
        {
            this.partName.text = this.getDescription(partInfo, null);
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
            str += string.Format("{0}\n{1}\n", baseEffect.getName(), this.getDescription(baseEffect,advanceEffect));
        }

        if (advanceEffect != null)
        {
            str += string.Format("{0}\n{1}\n", advanceEffect.getName(), advanceEffect.getDesc());
        }

        this.partName.text = str;
    }

    protected float getAddValue(IPart baseEffect,IPart advanceEffect)
    {
        //
        if (advanceEffect != null && advanceEffect.getId() > 0)
        {
            return baseEffect.getValueUpgrade();
        }
        else
        {
            return baseEffect.getValueDefault();
        }
    }

    protected string getDescription(IPart baseEffect, IPart advanceEffect)
    {
        float addValue = this.getAddValue(baseEffect, advanceEffect);
        string desc = baseEffect.getDesc();
        return GameUtils.formatDescription(desc, addValue);
    }
}   
