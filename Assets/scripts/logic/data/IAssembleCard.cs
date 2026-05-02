public interface IAssembleCard
{
    public int getBaseEffectId();
    public void setBaseEffectId(int id);

    public int getAdvancedEffectId();
    public void setAdvancedEffectId(int id);

    public int getTriggerId();
    public void setTriggerId(int id);

    public int getLevel();
    public void setLevel(int value);

    public int getTriggerNumber();

    public void addTriggerNumber();
    public void setTriggerNumber(int number);

    public int getUpgradeNumber();
    public void setUpgradeNumber(int number);
}
