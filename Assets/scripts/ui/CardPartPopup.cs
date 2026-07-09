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
            this.partName.text = this.getDescription(partInfo);
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

            IBaseEffectData data = this._user.getExtraInfo().getBaseEffectData(baseEffect.getId());
            IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.addLevel);
            if (baseEffectValue != null && baseEffectValue.getValue() > 0) {
                str += string.Format("当前{0}/{1}层\n", baseEffectValue.getValue(), baseEffectValue.getMaxValue());
            }

            if (data.getId() == GameCardConst.baseEffectId2025) {
                if (data.isState()) {
                    str += "<color=red>首次已触发</color>\n";
                }
            }
        }

        if (assembleCard.getAdvancedEffectId() <= 0)
        {
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
        
        this.partName.text = str;
    }

    protected List<float> getAddValue(IPart baseEffect,IPart advanceEffect)
    {
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
        List<float> values = this.getAddValue(baseEffect, advanceEffect);
        string desc = baseEffect.getDesc();
        return GameUtils.formatDescription(desc, values);
    }

    protected string getDescription(IPart baseEffect)
    {
        List<float> valueDefault = baseEffect.getValueDefault();
        string desc = baseEffect.getDesc();
        string descDefault = GameUtils.formatDescription(desc, valueDefault);
        if (baseEffect.getTargetPart() == TargetPart.baseEffect)
        {
            List<float> valueUpgrade = baseEffect.getValueUpgrade();
            string descUpgrade = GameUtils.formatDescription(desc, valueUpgrade);
            return descDefault + "\n<color=red>升级后效果：</color>\n" + descUpgrade;
        }
        else {
            return descDefault;
        }
    }
}   
