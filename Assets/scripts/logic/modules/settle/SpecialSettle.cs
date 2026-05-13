using System.Collections.Generic;

public class SpecialSettle : IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.removeTemporaryArmor(para.getAttackUser());
        this.removeTemporaryArmor(para.getDefenseUser());
        //
        para.reset();
    }

    private void removeTemporaryArmor(IUser user) {
        float addValue = user.getExtraInfo().getTemporaryArmor();
        if (addValue <= 0) {
            return;
        }
        user.addDefense(-addValue);

        IUICommonPara attackPara = new UICommonParaObject(user, ValueType.defense, -addValue, user.getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}