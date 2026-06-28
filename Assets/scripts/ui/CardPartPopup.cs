using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPartPopup : MonoBehaviour
{
    private IUser _user;
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

    public void setUser(IUser user) {
        this._user = user;
    }

    public void loadPartInfo(IPart partInfo) {
        if (partInfo != null)
        {
            this.partName.text = this.getDescription(partInfo, null);
        }
    }

    public void setAssembleCard(IAssembleCard assembleCard) {
        if (assembleCard == null) {
            return;
        }
        
        string str = "";
        IPart trigger = assembleCard.getTrigger();
        IPart baseEffect = assembleCard.getBaseEffect();
        IPart advanceEffect = assembleCard.getAdvancedEffect();
        if (trigger != null) {
            str += string.Format("{0}\n{1}\n", trigger.getName(), trigger.getDesc());
        }
        
        if (baseEffect != null) {
            str += string.Format("{0}\n{1}\n", baseEffect.getName(), this.getDescription(baseEffect,advanceEffect));
        }
        
        if (advanceEffect != null)
        {
            if (advanceEffect.getId() == 0) {
                List<IAssembleCard> cards = FightPokerMgr.Instance.getUserAssembleCards(this._user);
                int index = cards.FindIndex(card => card == assembleCard);
                switch (index)
                {
                    case 0:
                        str += string.Format("解锁：触发<color=red>{0}</color>/{1}次数\n",assembleCard.getTriggerNumber(), assembleCard.getUpgradeNumber());
                        break;
                    case 1:
                        str += string.Format("解锁：触发<color=red>blackJock</color>\n");
                        break;
                    case 2:
                        str += string.Format("解锁：触发<color=red>魔法技能</color>\n");
                        break;
                }
            }
            else
            {
                str += string.Format("{0}\n{1}\n", advanceEffect.getName(), advanceEffect.getDesc());
            }            
        }
        this.partName.text = str;
    }

    protected float getAddValue(IPart baseEffect,IPart advanceEffect)
    {
        List<float> addValues = null;
        if (advanceEffect != null && advanceEffect.getId() > 0)
        {
            addValues = baseEffect.getValueUpgrade();
        }
        else
        {
            addValues = baseEffect.getValueDefault();
        }

        if (addValues == null)
        {
            return 0;
        }
        else {
            return addValues[0];
        }   
    }

    protected string getDescription(IPart baseEffect, IPart advanceEffect)
    {
        float addValue = this.getAddValue(baseEffect, advanceEffect);
        string desc = baseEffect.getDesc();
        return GameUtils.formatDescription(desc, addValue);
    }
}   
