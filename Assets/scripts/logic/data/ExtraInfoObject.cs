using System.Collections.Generic;
public class ExtraInfoObject : IExtraInfo
{
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
    private float _addBleeding = 0;
    private List<float> _healOverTime = new List<float>();
    private List<float> _mpRegen = new List<float>();
    public void setMultATK(float value) {
        this._multATK += value;
        this._multATK = this._multATK < 0 ? 0 : this._multATK;
    }

    public float getMultATK() {
        return this._multATK;
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
    }

    public float getReflectDMG() {
        return this._reflectDMG;
    }

    public void setBonusArmor(float value) {
        this._bonusArmor += value;
        this._bonusArmor = this._bonusArmor < 0 ? 0 : this._bonusArmor;
    }
    public float getBonusArmor() {
        return this._bonusArmor;
    }

    public void setTemporaryArmor(float value) {
        this._temporaryArmor += value;
        this._temporaryArmor = this._temporaryArmor < 0 ? 0 : this._temporaryArmor;
    }
    public float getTemporaryArmor() {
        return this._temporaryArmor;
    }
    public void clearTemporaryArmor() {
        this._temporaryArmor = 0;
    }

    public void setLifeSteal(float value) {
        this._lifeSteal += value;
        this._lifeSteal = this._lifeSteal < 0 ? 0 : this._lifeSteal;
    }
    public float getLifeSteal() {
        return this._lifeSteal;
    }

    public void setHealOverTime(float value) {
        this._healOverTime.Add(value);
    }
    public float getHealOverTime() {
        float value = 0;
        if (this._healOverTime.Count > 0){
            value = this._healOverTime[0];
            this._healOverTime.RemoveAt(0);
        }
        return value;
    }

    public List<float> getHealOverTimes() {
        return this._healOverTime;
    }

    public void setHealToMP(float value) {
        this._healToMP += value;
        this._healToMP = this._healToMP < 0 ? 0 : this._healToMP;
    }
    public float getHealToMP() {
        return this._healToMP;
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
    }
    public float getSkillDamageUp() {
        return this._skillDamageUp;
    }

    public void setMpRegen(float value) {
        this._mpRegen.Add(value);
    }
    public float getMpRegen() {
        float value = 0;
        if (this._mpRegen.Count > 0)
        {
            value = this._mpRegen[0];
            this._mpRegen.RemoveAt(0);
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
    }
    public float getAddBleeding() {
        return this._addBleeding;
    }
}
