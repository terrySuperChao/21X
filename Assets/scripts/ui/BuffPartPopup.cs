using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffPartPopup : MonoBehaviour
{
    public Text partName;
    private IUser _user;
    private Dictionary<BuffType, string> _buffDic = new Dictionary<BuffType, string>();
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

    public void setUser(IUser user)
    {
        this._user = user;
        this.initBuffDesc();
    }

    public void setBuffType(BuffType buffType) {
        string desc = "";
        if (this._buffDic.ContainsKey(buffType))
        {
            desc = this._buffDic[buffType];
        }

        float value = CardMgr.Instance.getBaseEffectValue(this._user, (BaseEffectType)buffType);
        if (buffType == BuffType.healOverTime)
        {
            //desc = string.Format(desc, this._user.getExtraInfo().getHealOverTimes().Count);
        }
        else if (buffType == BuffType.mpRegen) {
            //desc = string.Format(desc, this._user.getExtraInfo().getMpRegens().Count);
        }
        this.partName.text = GameUtils.formatDescription(desc, value);
    }

    private void initBuffDesc() {
        if (this._buffDic.Count != 0) return;
        this._buffDic.Add(BuffType.multATK, "普通攻击额外造成%s%的伤害");
        this._buffDic.Add(BuffType.reflectDMG,"攻击时反弹%s点伤害");
        this._buffDic.Add(BuffType.bonusArmor, "转化方块属性，额外获得 %s%的护甲");
        this._buffDic.Add(BuffType.temporaryArmor, "临时%s的护甲");
        this._buffDic.Add(BuffType.lifeSteal, "造成伤害的 %s% 转化为回血");
        this._buffDic.Add(BuffType.healOverTime, "接下来的<color=red>{0}</color>回合,回复%s点生命值");
        this._buffDic.Add(BuffType.healToMP, "转化红桃属性，治疗量的%s%额外转化为法力值");
        this._buffDic.Add(BuffType.skillDamageUp, "技能效果提升%s%");
        this._buffDic.Add(BuffType.mpRegen, "接下来的<color=red>{0}</color>回合,回复%s点法力");
        this._buffDic.Add(BuffType.addBleeding, "获得当前法力值的%s%的法力值");
    }
}   
