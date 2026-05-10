public class SubArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Sub_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("SubArmorHandle=========>>");
        float addValue = this.getAddValue(para);

        IUser defenseUser = para.getDefenseUser();
        float defense = defenseUser.getDefense();
        if (defense >= addValue) {
            defense -= addValue;
        }else {
            defense = 0;
        }
        defenseUser.setDefense(defense);

        string desc = para.getAssembleCard().getBaseEffect().getDesc();
        string text = desc.Replace("%s", addValue.ToString());

        IUIFlyFontPara uiPara = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), text);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.defense, -addValue, defenseUser.getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
