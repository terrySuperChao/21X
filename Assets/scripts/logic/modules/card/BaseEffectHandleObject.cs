
public class BaseEffectHandleObject:IBaseEffectHandle
{
    public string getActionGenre() {
        return this._getActionGenre();
    }

    public void handle(ITriggerHandlePara para) {
        IUIFlyFontPara uiPara = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), this.getDescription(para));
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
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

    protected string getDescription(ITriggerHandlePara para) {
        float addValue = this.getAddValue(para);
        string desc = para.getAssembleCard().getBaseEffect().getDesc();
        int index1 = desc.IndexOf("%s");
        if (index1 > -1)
        {
            int index = desc.IndexOf("%s%");
            if (index > -1)
            {
                addValue *= 100;
            }
            return desc.Replace("%s", addValue.ToString());
        }
        else {
            return desc;
        }
    }

    protected virtual string _getActionGenre() { return ""; }
    protected virtual void _handle(ITriggerHandlePara para) { }
}
