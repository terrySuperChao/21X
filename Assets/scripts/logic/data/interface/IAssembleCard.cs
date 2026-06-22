public interface IAssembleCard
{
    public int getBaseEffectId();
    public IPart getBaseEffect();
    public void setBaseEffectId(int id);

    public int getAdvancedEffectId();
    public IPart getAdvancedEffect();
    public void setAdvancedEffectId(int id);

    public int getTriggerId();
    public IPart getTrigger();
    public void setTriggerId(int id);

    public int getTriggerNumber();
    public void addTriggerNumber();
    public void setTriggerNumber(int number);

    public int getUpgradeNumber();
    public void setUpgradeNumber(int number);
}
