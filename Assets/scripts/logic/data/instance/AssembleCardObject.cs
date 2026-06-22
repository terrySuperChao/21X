
public class AssembleCardObject : IAssembleCard
{
    private IPart _baseEffect = null;
    private IPart _advancedEffect = null;
    private IPart _trigger = null;
    private int _triggerNumber = 0;
    private int _upgradeNumber = 0;

    public AssembleCardObject(int triggerId, int baseEffectId, int advancedEffectId,int triggerNumber,int upgradeNumber) {
        this._triggerNumber = triggerNumber;
        this._upgradeNumber = upgradeNumber;
        this.setTriggerId(triggerId);
        this.setBaseEffectId(baseEffectId);
        this.setAdvancedEffectId(advancedEffectId);
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
        return this._advancedEffect.getId();
    }
    public IPart getAdvancedEffect()
    {
        return this._advancedEffect;
    }
    public void setAdvancedEffectId(int id)
    {
        this._advancedEffect = GameStaticConfigMgr.Instance.getAdvancedEffectConfig().getAdvancedEffectInfoId(id);
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
