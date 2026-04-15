public interface IAssembleCard
{
    public int getBaseDataId();
    public void setBaseDataId(int id);

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
