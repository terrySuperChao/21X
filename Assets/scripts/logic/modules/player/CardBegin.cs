using System.Collections.Generic;

public class CardBegin : IGameBegin
{
    public void gameBegin(IGameBeginPara para)
    {
        if (CardMgr.Instance.getRound() % 2 == 1) {
            List<IUser> users = para.getUsers();
            for (int i = 0; i < users.Count; i++)
            {
                List<ICard> cards = CardMgr.Instance.getRandomCard(users[i]);
                if (cards.Count > 0) {
                    GameMessage.Instance.addMsg(GameConst.DEALCARD, users[i], cards);
                }
            }
        }
        CardMgr.Instance.addRound();
    }
}
