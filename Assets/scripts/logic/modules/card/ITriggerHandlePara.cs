using System.Collections.Generic;
public interface ITriggerHandlePara
{
    public IUser getUser();
    public void setUser(IUser user);

    public void setAttackUser(IUser user);
    public IUser getAttackUser();

    public void setDefenseUser(IUser user);
    public IUser getDefenseUser();

    public IPoker getPoker();
    public void setPoker(IPoker poker);

    public void setPokerSuit(PokerSuit pokerSuit);
    public PokerSuit getPokerSuit();

    public float getBaseValue();
    public void setBaseValue(float baseValue);

    public void setMagicAttack(bool isMagicAttack);
    public bool isMagicAttack();

    public void setGameSettlePara(IGameSettlePara para);
    public IGameSettlePara getGameSettlePara();

    public IRoundResult getRoundResult(IUser user);
    public void addRoundResult(IRoundResult value);

    public IAssembleCard getAssembleCard();
    public void setAssembleCard(IAssembleCard card);

    public void reset();
}
