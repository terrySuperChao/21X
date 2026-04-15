
public class AssembleCardObject : IAssembleCard
{
    private int _baseDataId = 0;
    private int _triggerId = 0;
    private int _level = 0;
    private int _triggerNumber = 0;
    private int _upgradeNumber = 0;

    public AssembleCardObject(int baseDataId, int triggerId, int level,int triggerNumber,int upgradeNumber) {
        this._baseDataId = baseDataId;
        this._triggerId = triggerId;
        this._level = level;
        this._triggerNumber = triggerNumber;
        this._upgradeNumber = upgradeNumber;
    }

    public int getBaseDataId() {
        return this._baseDataId;
    }
    public void setBaseDataId(int id) {
        this._baseDataId = id;
    }

    public int getTriggerId() {
        return this._triggerId;
    }
    public void setTriggerId(int id) {
        this._triggerId = id;
    }

    public int getLevel() { 
        return this._level;
    }
    public void setLevel(int value) { 
        this._level = value;
    }

    public int getTriggerNumber() {
        return this._triggerNumber;
    }
    public void setTriggerNumber(int number) {
        this._triggerNumber = number;
    }

    public void addTriggerNumber() { 
        this._triggerNumber++;
    }

    public int getUpgradeNumber() {
        return this._upgradeNumber;
    }
    public void setUpgradeNumber(int number) {
        this._upgradeNumber = number;
    }

}
