using System;
using System.Collections.Generic;
using Pb;
using UnityEngine;
using UnityEngine.UI;

public class BuffPartPopup : MonoBehaviour
{
    public Text partName;
    private IUser _user;
    private Dictionary<BaseEffectType, string> _buffDic = new Dictionary<BaseEffectType, string>();
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

    public void setBuffType(BaseEffectType buffType) {
        string desc = "";
        if (this._buffDic.ContainsKey(buffType))
        {
            desc = this._buffDic[buffType];
        }

        float value = 0;
        if (buffType == BaseEffectType.healOverTime)
        {
            float level = 2;
            IBaseEffectData data = this._user.getExtraInfo().getBaseEffectData(GameCardConst.baseEffectId2023);
            if (data.isState())
            {
                IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
                level = baseEffectValue1.getMaxValue() - baseEffectValue1.getValue();
            }
            desc = string.Format(desc, level);
            value = GameEffectMgr.Instance.getBaseEffectValue(this._user, buffType);
        }
        else if (buffType == BaseEffectType.mpRegen)
        {
            float level = 3;
            IBaseEffectData data = this._user.getExtraInfo().getBaseEffectData(GameCardConst.baseEffectId2033);
            if (data.isState())
            {
                IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
                level = baseEffectValue1.getMaxValue() - baseEffectValue1.getValue();
            }
            desc = string.Format(desc, level);
            value = GameEffectMgr.Instance.getBaseEffectValue(this._user, buffType);
        }
        else if (buffType == BaseEffectType.rtCountAttack)
        {
            value = GameRunTimeMgr.Instance.getRunTimeCountAttack(this._user);
        }
        else {
            value = GameEffectMgr.Instance.getBaseEffectValue(this._user, buffType);
        }
        this.partName.text = GameUtils.formatDescription(desc, value);
    }

    private void initBuffDesc() {
        if (this._buffDic.Count != 0) return;
        this._buffDic.Add(BaseEffectType.addCrit, "固定增加%s%暴击率");
        this._buffDic.Add(BaseEffectType.multATK, "普通攻击额外造成%s%的伤害");
        this._buffDic.Add(BaseEffectType.reflectDMG,"攻击时反弹%s点伤害");
        this._buffDic.Add(BaseEffectType.bonusArmor, "转化方块属性，额外获得 %s%的护甲");
        this._buffDic.Add(BaseEffectType.temporaryArmor, "临时%s的护甲");
        this._buffDic.Add(BaseEffectType.lifeSteal, "造成伤害的 %s% 转化为回血");
        this._buffDic.Add(BaseEffectType.healOverTime, "接下来的<color=red>{0}</color>回合,回复%s点生命值");
        this._buffDic.Add(BaseEffectType.healToMP, "转化红桃属性，治疗量的%s%额外转化为法力值");
        this._buffDic.Add(BaseEffectType.skillDamageUp, "技能效果提升%s%");
        this._buffDic.Add(BaseEffectType.mpRegen, "接下来的<color=red>{0}</color>回合,回复%s点法力");
        this._buffDic.Add(BaseEffectType.addBleeding, "获得当前法力值的%s%的法力值");
        this._buffDic.Add(BaseEffectType.rtCountAttack, "每累计获得攻击力%s点");
    }
}   
