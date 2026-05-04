
public class BaseEffectHandleObject:IBaseEffectHandle
{
    public string getActionGenre() {
        return this._getActionGenre();
    }

    public void handle(ITriggerHandlePara para) {
        this._handle(para);
    }
    protected float getAddValue(ITriggerHandlePara para) {
        //
        if (para.getAssembleCard().getAdvancedEffect().getId() > 0){
            return para.getAssembleCard().getBaseEffect().getValueUpgrade();
        }
        else {
            return para.getAssembleCard().getBaseEffect().getValueDefault();
        }
    }
    protected virtual string _getActionGenre() { return ""; }
    protected virtual void _handle(ITriggerHandlePara para) { }
}
