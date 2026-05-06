using System.Collections.Generic;

public class SpecialSettle : IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        if (para == null)
        {
            List<IUser> users = FightPokerMgr.Instance.getPlayers();
            para = new TriggerHandleParaObject();
            para.setRoundResult(new RoundResultObject());
            para.setAttackUser(users.Find(user => user.isNpc() == true));
            para.setDefenseUser(users.Find(user => user.isNpc() == false));
        }

        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();

        para.setUser(attackUser);
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        CardMgr.Instance.handle(para, CardHandleType.roundSpecialAttr);

        para.setUser(defenseUser);
        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);
        CardMgr.Instance.handle(para, CardHandleType.roundSpecialAttr);

        para.setUser(attackUser);
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
    }
}