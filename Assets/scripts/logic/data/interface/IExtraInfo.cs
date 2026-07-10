using System;
using System.Collections.Generic;

public interface IExtraInfo
{
    //运行过程中数据 rt前缀
    //伤害=普通攻击+魔法攻击+直接扣血

    //消耗的护甲
    public void setRtFreezeArmorValue(float value);
    public float getRtFreezeArmorValue();    
    public void clearRtFreezeArmorValue();

    //添加的护甲值
    public void setRtAddDefenseValue(float value);
    public float getRtAddDefenseValue();

    public void setMagicAttack(bool isMagicAttack);
    public bool isMagicAttack();

    //效果
    public IBaseEffectData getBaseEffectData(int id);
    public List<IBaseEffectData> getBaseEffectDatas();
    public void addBaseEffectData(Pb.BaseEffectData value);
    public void setBaseEffectDataInstance(Func<int,IBaseEffectData> func);
}
