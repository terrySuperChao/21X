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
    public List<float> getHealOverTimes();

    //下次转化红桃属性，治疗量的 %s% 额外转化为法力值
    public void setHealToMP(float value);
    public float getHealToMP();

    //回复 %s 点生命值，一场战斗仅生效一次
    public void setHealSuper(float value);
    public float getHealSuper();

    //下次技能效果提升 %s%，可叠加
    public void setSkillDamageUp(float value);
    public float getSkillDamageUp();

    //接下来的3回合每回合回复 %s 点法力
    public void setMpRegen(float value);
    public float getMpRegen();
    public List<float> getMpRegens();

    //本局技能释放所需 MP 减少 %s
    public void setMpMaxSub(float value);
    public float getMpMaxSub();

    //下次普通攻击使敌方获得 3 层流血状态
    public void setAddBleeding(float value);
    public float getAddBleeding();

    //下次普通攻击连续触发两次
    public void setDoubleProc(float value);
    public float getDoubleProc();
    public void clearDoubleProc();

    //下次普通攻击无视对手护甲
    public void setIgnoreArmor(float value);
    public float getIgnoreArmor();
    public void clearIgnoreArmor();

    //下次普通攻击时若对手血量低于 15% 直接处决
    public void setExecute(float value);
    public float getExecute();

    //若下次普通攻击暴击，则保留50%的攻击力
    public void setRetainATK(float value);
    public float getRetainATK();
    public void clearRetainATK();
}
