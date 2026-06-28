
using System.Collections.Generic;

public class BaseEffectHandleObject:IBaseEffectHandle
{
    public int getId() {
        return this._getId();
    }
    
    public void handle(ITriggerHandlePara para) {
        UnityEngine.Debug.Log("开始处理 Id:" + this.getId()+"逻辑");
        IUIFlyFontPara uiPara = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), this.getDescription(para));
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
        this._handle(para);
        UnityEngine.Debug.Log("处理结束 Id:" + this.getId());
    }

    public void effect(IBaseEffectHandlePara para) {
        this._effect(para);
    }

    public float getAdvancedValue(ITriggerHandlePara para) {
        return this._getAdvancedValue(para);
    }

    protected float getAddValue(ITriggerHandlePara para,int index = 0) {
        bool isUpgrade = para.getAssembleCard().getAdvancedEffect().getId() > 0;
        List<float> addValues = isUpgrade ? para.getAssembleCard().getBaseEffect().getValueUpgrade()
                                          : para.getAssembleCard().getBaseEffect().getValueDefault();
   
        float value = 0;
        if (addValues == null || addValues.Count == 0)
        {
            value = 0;
        }else {
            if (index < 0){
                index = 0;
            }else if(addValues.Count <= index) {
                index = addValues.Count - 1;
            }
            value = addValues[index];
        }
        
        if (isUpgrade)
        {       
            IBaseEffectHandle handle = AdvancedEffectHandleMgr.Instance.getAdvancedEffectHandle(para.getAssembleCard().getAdvancedEffectId());
            if (handle != null) {
                value *= 1 + handle.getAdvancedValue(para);
            }
        }
        
        return value;
    }

    protected string getDescription(ITriggerHandlePara para) {
        float addValue = this.getAddValue(para);
        string desc = para.getAssembleCard().getBaseEffect().getDesc();
        return GameUtils.formatDescription(desc, addValue);
    }

    protected virtual int _getId() { return 0; }
    protected virtual void _handle(ITriggerHandlePara para) { }
    protected virtual void _effect(IBaseEffectHandlePara para) { }
    protected virtual float _getAdvancedValue(ITriggerHandlePara para) { return 0; }
}
