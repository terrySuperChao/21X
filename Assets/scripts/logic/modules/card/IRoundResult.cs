public interface IRoundResult
{
    //
    public void setUser(IUser user);
    public IUser getUser();

    //
    public float getPenetrateValue();
    public void setPenetrateValue(float value);

    //
    public float getSaveMagicValue();
    public void setSaveMagicValue(float value);

    //
    public float getSaveAttackValue();
    public void setSaveAttackValue(float value);

    //
    public float getReflectValue();
    public void setReflectValue(float value);

    //
    public float getAttributeMult();
    public void setAttributeMult(float value);

    public float getMagicValue();
    public void addMagicValue(float value);

    //伤害=普通攻击+魔法攻击+直接扣血
    public float getHurtVaule();
    public void addHurtValue(float value);
    //
    public void reset();
}
