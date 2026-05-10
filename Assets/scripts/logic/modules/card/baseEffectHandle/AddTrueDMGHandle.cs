//获得 2/4 点真实伤害
public class AddTrueDMGHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_True_DMG";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddTrueDMGHandle=========>>");
        float addValue = this.getAddValue(para);

        IUser defenseUser = para.getDefenseUser();
        float blood = defenseUser.getBlood();
        if (blood >= addValue) {
            blood -= addValue;
        }else {
            blood = 0;
        }
        defenseUser.setBlood(blood);
        para.getRoundResult(para.getAttackUser()).addHurtValue(addValue);

        string desc = para.getAssembleCard().getBaseEffect().getDesc();
        string text = desc.Replace("%s", addValue.ToString());

        IUIFlyFontPara uiPara = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), text);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.blood, -addValue, defenseUser.getBlood());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
       
        if (blood <= 0) {
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.fightOver);
            GameMessage.Instance.addMsg(GameConst.GAMESETTLE, para.getUser());
            GameMessage.Instance.addMsg(GameConst.GAMEOVER);
        }
    }
}
