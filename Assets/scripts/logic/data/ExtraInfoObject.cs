using System;
using System.Collections.Generic;

public class ExtraInfoObject : IExtraInfo
{
    //基础
    private float _multATK = 0;
    private float _addCrit = 0;
    private float _reflectDMG = 0;
    private float _bonusArmor = 0;
    private float _temporaryArmor = 0;
    private float _lifeSteal = 0;
    private float _healToMP = 0;
    private float _healSuper = 0;
    private float _skillDamageUp = 0;
    private float _mpMaxSub = 0;
    //进阶
    private float _addBleeding = 0;
    private float _doubleProc = 0;
    private float _ignoreArmor = 0;
    private float _execute = 0;
    private float _retainATK = 0;
    private float _reflectPercent = 0;
    private float _magicImmunity = 0;
    private float _armorATK = 0;
    private float _immunityDeBuff = 0;
    private float _freezeArmor = 0;
    //运行时
    private float _rtMagicValue = 0;
    private float _rtHurtValue = 0;
    private float _rtFreezeArmorValue = 0;
    private List<float> _healOverTime = new List<float>();
    private List<float> _mpRegen = new List<float>();
    private List<BuffType> _buffTypes = new List<BuffType>();
    private Action<BuffAction, BuffType> _callback = null;
    public void setBuffAction(Action<BuffAction, BuffType> callback) {
        this._callback = callback;
    }

    public List<BuffType> getBuffs() {
        return this._buffTypes;
    }

    public void setMultATK(float value) {
        this._multATK = value;
        this._multATK = this._multATK < 0 ? 0 : this._multATK;
        this.addBuffType(BuffType.multATK, value);
    }

    public float getMultATK() {
        return this._multATK;
    }

    public void clearMultATK() {
        this._multATK = 0;
        this.removeBuffType(BuffType.multATK);
    }

    public void setAddCrit(float value) {
        this._addCrit += value;
        this._addCrit = this._addCrit < 0 ? 0 : this._addCrit;
    }

    public float getAddCrit() {
        return this._addCrit;
    }

    public void setReflectDMG(float value) {
        this._reflectDMG += value;
        this._reflectDMG = this._reflectDMG < 0 ? 0 : this._reflectDMG;
        this.addBuffType(BuffType.reflectDMG,value);
    }

    public float getReflectDMG() {
        return this._reflectDMG;
    }
    public void clearReflectDMG() {
        this._reflectDMG = 0;
    }

    public void setBonusArmor(float value) {
        this._bonusArmor = value;
        this._bonusArmor = this._bonusArmor < 0 ? 0 : this._bonusArmor;
        this.addBuffType(BuffType.multATK, value);
    }
    public float getBonusArmor() {
        return this._bonusArmor;
    }
    public void clearBonusArmor() {
        this._bonusArmor = 0;
        this.removeBuffType(BuffType.multATK);
    }

    public void setTemporaryArmor(float value) {
        this._temporaryArmor += value;
        this._temporaryArmor = this._temporaryArmor < 0 ? 0 : this._temporaryArmor;
        this.addBuffType(BuffType.temporaryArmor,value);
    }
    public float getTemporaryArmor() {
        return this._temporaryArmor;
    }
    public void clearTemporaryArmor() {
        this._temporaryArmor = 0;
        this.removeBuffType(BuffType.temporaryArmor);
    }

    public void setLifeSteal(float value) {
        this._lifeSteal = value;
        this._lifeSteal = this._lifeSteal < 0 ? 0 : this._lifeSteal;
        this.addBuffType(BuffType.lifeSteal, value);
    }
    public float getLifeSteal() {
        return this._lifeSteal;
    }
    public void clearLifeSteal() {
        this._lifeSteal = 0;
        this.removeBuffType(BuffType.lifeSteal);
    }

    public void setHealOverTime(float value) {
        this._healOverTime.Add(value);
    }
    public float getHealOverTime() {
        float value = 0;
        if (this._healOverTime.Count > 0) {
            value = this._healOverTime[0];
            this._healOverTime.RemoveAt(0);
        }
        return value;
    }

    public List<float> getHealOverTimes() {
        return this._healOverTime;
    }

    public void setHealToMP(float value) {
        this._healToMP = value;
        this._healToMP = this._healToMP < 0 ? 0 : this._healToMP;
        this.addBuffType(BuffType.healToMP,value);
    }
    public float getHealToMP() {
        return this._healToMP;
    }
    public void clearHealToMP() {
        this._healToMP = 0;
        this.removeBuffType(BuffType.healToMP);
    }

    public void setHealSuper(float value) {
        this._healSuper = value;
    }
    public float getHealSuper() {
        return this._healSuper;
    }

    public void setSkillDamageUp(float value) {
        this._skillDamageUp += value;
        this._skillDamageUp = this._skillDamageUp < 0 ? 0 : this._skillDamageUp;
        this.addBuffType(BuffType.skillDamageUp, value);
    }
    public float getSkillDamageUp() {
        return this._skillDamageUp;
    }
    public void clearSkillDamageUp() {
        this._skillDamageUp = 0;
        this.removeBuffType(BuffType.skillDamageUp);
    }

    public void setMpRegen(float value) {
        this._mpRegen.Add(value);
        this.addBuffType(BuffType.mpRegen, value);
    }
    public float getMpRegen() {
        float value = 0;
        if (this._mpRegen.Count > 0)
        {
            value = this._mpRegen[0];
            this._mpRegen.RemoveAt(0);
        }
        else {
            this.removeBuffType(BuffType.mpRegen);
        }
        return value;
    }
    public List<float> getMpRegens() {
        return this._mpRegen;
    }

    public void setMpMaxSub(float value) {
        this._mpMaxSub += value;
        this._mpMaxSub = this._mpMaxSub < 0 ? 0 : this._mpMaxSub;
    }
    public float getMpMaxSub() {
        return this._mpMaxSub;
    }

    public void setAddBleeding(float value) {
        this._addBleeding += value;
        this._addBleeding = this._addBleeding < 0 ? 0 : this._addBleeding;
        if (this._addBleeding > 0)
        {
            this.addBuffType(BuffType.addBleeding, value);
        }
        else {
            this.removeBuffType(BuffType.addBleeding);
        }
    }
    public float getAddBleeding() {
        return this._addBleeding;
    }

    public void setDoubleProc(float value) {
        this._doubleProc = value;
        this._doubleProc = this._doubleProc < 0 ? 0 : this._doubleProc;
        this.addBuffType(BuffType.doubleProc, value);
    }
    public float getDoubleProc() {
        return this._doubleProc;
    }
    public void clearDoubleProc() {
        this._doubleProc = 0;
        this.removeBuffType(BuffType.doubleProc);
    }

    public void setIgnoreArmor(float value) {
        this._ignoreArmor = value;
        this._ignoreArmor = this._ignoreArmor < 0 ? 0 : this._ignoreArmor;
        this.addBuffType(BuffType.ignoreArmor,value);
    }
    public float getIgnoreArmor() {
        return this._ignoreArmor;
    }
    public void clearIgnoreArmor() {
        this._ignoreArmor = 0;
        this.removeBuffType(BuffType.ignoreArmor);
    }

    public void setExecute(float value) {
        this._execute = value;
        this._execute = this._execute < 0 ? 0 : this._execute;
        this.addBuffType(BuffType.execute, value);
    }
    public float getExecute() {
        return this._execute;
    }
    public void clearExecute() {
        this._execute = 0;
        this.removeBuffType(BuffType.execute);
    }

    public void setRetainATK(float value) {
        this._retainATK = value;
        this._retainATK = this._retainATK < 0 ? 0 : this._retainATK;
        this.addBuffType(BuffType.retainATK,value);
    }
    public float getRetainATK() {
        return this._retainATK;
    }
    public void clearRetainATK() {
        this._retainATK = 0;
        this.removeBuffType(BuffType.retainATK);
    }

    public void setReflectPercent(float value) {
        this._reflectPercent = value;
        this._reflectPercent = this._reflectPercent < 0 ? 0 : this._reflectPercent;
        this.addBuffType(BuffType.reflectPercent,value);
    }
    public float getReflectPercent() {
        return this._reflectPercent;
    }
    public void clearReflectPercent() {
        this._reflectPercent = 0;
        this.removeBuffType(BuffType.reflectPercent);
    }

    public void setMagicImmunity(float value) {
        this._magicImmunity = value;
        this.addBuffType(BuffType.magicImmunity, value);
    }
    public float getMagicImmunity() {
        return this._magicImmunity;
    }
    public void clearMagicImmunity() {
        this._magicImmunity = 0;
        this.removeBuffType(BuffType.magicImmunity);
    }

    public void setArmorATK(float value) {
        this._armorATK = value;
        this._armorATK = this._armorATK < 0 ? 0 : this._armorATK;
        this.addBuffType(BuffType.armorATK, value);
    }
    public float getArmorATK() {
        return this._armorATK;
    }
    public void clearArmorATK() {
        this._armorATK = 0;
        this.removeBuffType(BuffType.armorATK);
    }

    public void setImmunityDeBuff(float value) {
        this._immunityDeBuff = value;
        this._immunityDeBuff = this._immunityDeBuff < 0 ? 0 : this._immunityDeBuff;
        this.addBuffType(BuffType.immunityDeBuff, value);
    }
    public float getImmunityDeBuff() {
        return this._immunityDeBuff;
    }
    public void clearImmunityDeBuff() {
        this._immunityDeBuff = 0;
        this.removeBuffType(BuffType.immunityDeBuff);
    }

    public void setFreezeArmor(float value)
    {
        this._freezeArmor = value;
        this._freezeArmor = this._freezeArmor < 0 ? 0 : this._freezeArmor;
        this.addBuffType(BuffType.freezeArmor, value);
    }
    public float getFreezeArmor()
    {
        return this._freezeArmor;
    }
    public void clearFreezeArmor()
    {
        this._freezeArmor = 0;
        this.removeBuffType(BuffType.freezeArmor);
    }

    public void setRtMagicValue(float value)
    {
        this._rtMagicValue += value;
        this._rtMagicValue = this._rtMagicValue < 0 ? 0 : this._rtMagicValue;
        this.addBuffType(BuffType.rtMagicValue, value);
    }
    public float getRtMagicValue()
    {
        return this._rtMagicValue;
    }
    public void clearRtMagicValue() {
        this._rtMagicValue = 0;
        this.removeBuffType(BuffType.rtMagicValue);
    }

    public void setRtHurtValue(float value)
    {
        this._rtHurtValue += value;
        this._rtHurtValue = this._rtHurtValue < 0 ? 0 : this._rtHurtValue;
        this.addBuffType(BuffType.rtHurtValue, value);
    }
    public float getRtHurtVaule()
    {
        return this._rtHurtValue;
    }
    public void clearRtHurtValue() {
        this._rtHurtValue = 0;
        this.removeBuffType(BuffType.rtHurtValue);
    }

    public void setRtFreezeArmorValue(float value)
    {
        this._rtFreezeArmorValue = value;
        this._rtFreezeArmorValue = this._rtFreezeArmorValue < 0 ? 0 : this._rtFreezeArmorValue;
        this.addBuffType(BuffType.rtFreezeArmorValue, value);
    }
    public float getRtFreezeArmorValue()
    {
        return this._rtFreezeArmorValue;
    }
    public void clearRtFreezeArmorValue() {
        this._rtFreezeArmorValue = 0;
        this.removeBuffType(BuffType.rtFreezeArmorValue);
    }

    private void addBuffType(BuffType type,float value) {
        if (value <= 0) return;

        int index = this._buffTypes.FindIndex(buffType => buffType == type);
        if (index == -1) {
            this._buffTypes.Add(type);
            this._callback?.Invoke(BuffAction.add, type);
        }
    }

    private void removeBuffType(BuffType type) {
        int index = this._buffTypes.FindIndex(buffType => buffType == type);
        if(index != -1)
        {
            this._buffTypes.RemoveAt(index);
            this._callback?.Invoke(BuffAction.remove, type);
        }
    }
}
