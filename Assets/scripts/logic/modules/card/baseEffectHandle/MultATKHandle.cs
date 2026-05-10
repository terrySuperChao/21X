//下次攻击额外造成 20%/50% 的伤害
public class MultATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Mult_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddATKHandle=========>>");
        float addValue = this.getAddValue(para);
        
        string desc = para.getAssembleCard().getBaseEffect().getDesc();
        string text = desc.Replace("%s", addValue.ToString());

        IUIFlyFontPara uiPara = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), text);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
    }
}
