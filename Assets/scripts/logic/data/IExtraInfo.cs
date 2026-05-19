using System;
using System.Collections.Generic;

public interface IExtraInfo
{
    public void setBuffAction(Action<BuffAction, BuffType> callback);
    public List<BuffType> getBuffs();
    //下次攻击额外造成伤害 不可叠加
    public void setMultATK(float value);
    public float getMultATK();
    public void clearMultATK();

    //固定增加 %s% 暴击率
    public void setAddCrit(float value);
    public float getAddCrit();

    //受到攻击时反弹 %s 点伤害
    public void setReflectDMG(float value);
    public float getReflectDMG();
    public void clearReflectDMG();

    //下次转化方块属性，额外获得的护甲 不可叠加
    public void setBonusArmor(float value);
    public float getBonusArmor();
    public void clearBonusArmor();

    //获得当前护甲 %s% 的临时护甲
    public void setTemporaryArmor(float value);
    public float getTemporaryArmor();
    public void clearTemporaryArmor();

    //下一次造成伤害的 %s% 转化为回血 不可叠加
    public void setLifeSteal(float value);
    public float getLifeSteal();
    public void clearLifeSteal();

    //接下来的2回合每回合回复 %s 点生命值
    public void setHealOverTime(float value);
    public float getHealOverTime();
    public List<float> getHealOverTimes();

    //下次转化红桃属性，治疗量的 %s% 额外转化为法力值,不可叠加
    public void setHealToMP(float value);
    public float getHealToMP();
    public void clearHealToMP();

    //回复 %s 点生命值，一场战斗仅生效一次
    public void setHealSuper(float value);
    public float getHealSuper();

    //下次技能效果提升 %s%，可叠加
    public void setSkillDamageUp(float value);
    public float getSkillDamageUp();
    public void clearSkillDamageUp();

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

    //将下次对手普通攻击的 50% 反弹给对手
    public void setReflectPercent(float value);
    public float getReflectPercent();
    public void clearReflectPercent();

    //获得 1个技能免疫的护盾，无法叠加
    public void setMagicImmunity(float value);
    public float getMagicImmunity();
    public void clearMagicImmunity();

    //每次获得护甲对对手造成 5 点伤害
    public void setArmorATK(float value);
    public float getArmorATK();

    //下回合免疫负面状态的伤害
    public void setImmunityDeBuff(float value);
    public float getImmunityDeBuff();
    public void clearImmunityDeBuff();

    //运行过程中数据 rt前缀
    //魔法值
    public float getRtMagicValue();
    public void setRtMagicValue(float value);
    public void clearRtMagicValue();

    //伤害=普通攻击+魔法攻击+直接扣血
    public float getRtHurtVaule();
    public void setRtHurtValue(float value);
    public void clearRtHurtValue();
}
