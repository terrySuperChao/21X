//削减目标 4/7 点护甲
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

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.defense, -addValue, defenseUser.getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
