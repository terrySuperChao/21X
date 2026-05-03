
public class AssembleCardObject : IAssembleCard
{
    private IPart _baseEffect = null;
    private int _advancedEffectId = 0;
    private IPart _trigger = null;
    private int _level = 0;
    private int _triggerNumber = 0;
    private int _upgradeNumber = 0;

    public AssembleCardObject(int baseEffectId, int triggerId, int level,int triggerNumber,int upgradeNumber) {
        this._level = level;
        this._triggerNumber = triggerNumber;
        this._upgradeNumber = upgradeNumber;
        this.setBaseEffectId(baseEffectId);
        this.setTriggerId(triggerId);
    }

    public int getBaseEffectId() {
        return this._baseEffect.getId();
    }
    public IPart getBaseEffect() {
        return this._baseEffect;
    }
    public void setBaseEffectId(int id) {
        this._baseEffect = GameStaticConfigMgr.Instance.getBaseEffectConfig().getBaseEffectId(id);
    }
    public int getAdvancedEffectId()
    {
        return this._advancedEffectId;
    }
    public void setAdvancedEffectId(int id)
    {
        this._advancedEffectId = id;
    }

    public IPart getTrigger() {
        return this._trigger;
    }
    public int getTriggerId() {
        return this._trigger.getId();
    }
    public void setTriggerId(int id) {
        this._trigger = GameStaticConfigMgr.Instance.getTriggerConfig().getTriggerId(id);
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
