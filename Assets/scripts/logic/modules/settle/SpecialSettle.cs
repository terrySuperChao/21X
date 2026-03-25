using System.Collections.Generic;

public class SpecialSettle : IAttackSettle
{
    public void settle(ICardHandlePara handlePara) {
        //Æ½¾Ö
        if (handlePara == null)
        {
            List<IUser> users = FightPokerMgr.Instance.getPlayers();
            handlePara = new CardHandleParaObject();
            handlePara.setRoundResult(new RoundResultObject());
            handlePara.setAttackUser(users.Find(user => user.isNpc() == true));
            handlePara.setDefenseUser(users.Find(user => user.isNpc() == false));
        }

        IUser attackUser = handlePara.getAttackUser();
        IUser defenseUser = handlePara.getDefenseUser();

        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundSpecialAttr);

        handlePara.setUser(defenseUser);
        handlePara.setAttackUser(defenseUser);
        handlePara.setDefenseUser(attackUser);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundSpecialAttr);

        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
    }
}