using System.Collections.Generic;

public interface IExtraInfo
{
    //下次攻击额外造成伤害
    public void setMultATK(float value);
    public float getMultATK();

    //固定增加 %s% 暴击率
    public void setAddCrit(float value);
    public float getAddCrit();

    //受到攻击时反弹 %s 点伤害
    public void setReflectDMG(float value);
    public float getReflectDMG();

    //下次转化方块属性，额外获得的护甲
    public void setBonusArmor(float value);
    public float getBonusArmor();

    //获得当前护甲 %s% 的临时护甲
    public void setTemporaryArmor(float value);
    public float getTemporaryArmor();
    public void clearTemporaryArmor();

    //下一次造成伤害的 %s% 转化为回血
    public void setLifeSteal(float value);
    public float getLifeSteal();

    //接下来的2回合每回合回复 %s 点生命值
    public void setHealOverTime(float value);
    public float getHealOverTime();
}
