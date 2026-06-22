public interface IBaseEffectHandlePara
{
    public void setAttackUser(IUser user);
    public IUser getAttackUser();

    public void setDefenseUser(IUser user);
    public IUser getDefenseUser();

    public void setEffectType(AdvancedEffectType type);
    public AdvancedEffectType getEffectType();

    public void setExtralValue(float value);
    public float getExtralValue();
}
