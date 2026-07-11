using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffPart : MonoBehaviour
{
    public Text partName;
    public GameObject partImage;
    private IUser _user;
    private BaseEffectType _buffType;
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

    public void setBuffType(BaseEffectType buffType) {
        this.initBuffDesc();
        this._buffType = buffType;
        string desc = "";
        if (this._buffDic.ContainsKey(buffType))
        {
            desc = this._buffDic[buffType];
        }
        this.partName.text = desc;
    }

    public BaseEffectType getBuffType() {
        return this._buffType;
    }

    public void setUser(IUser user) {
        this._user = user;
    }

    public IUser getUser() {
        return this._user;
    }

    private void initBuffDesc()
    {
        if (this._buffDic.Count != 0) return;
        this._buffDic.Add(BaseEffectType.addCrit, "暴击率");
        this._buffDic.Add(BaseEffectType.multATK, "额外伤害");
        this._buffDic.Add(BaseEffectType.reflectDMG, "反弹伤害");
        this._buffDic.Add(BaseEffectType.bonusArmor, "额外护甲");
        this._buffDic.Add(BaseEffectType.temporaryArmor, "临时护甲");
        this._buffDic.Add(BaseEffectType.lifeSteal, "转化回血");
        this._buffDic.Add(BaseEffectType.healOverTime, "回复生命值");
        this._buffDic.Add(BaseEffectType.healToMP, "转化法力值");
        this._buffDic.Add(BaseEffectType.skillDamageUp, "技能效果");
        this._buffDic.Add(BaseEffectType.mpRegen, "回复法力");
        this._buffDic.Add(BaseEffectType.addBleeding, "获得法力值");
        this._buffDic.Add(BaseEffectType.rtCountAttack, "累计攻击力");
    }
}   
