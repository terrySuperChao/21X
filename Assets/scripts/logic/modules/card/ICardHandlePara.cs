using System;
public interface ICardHandlePara
{
    public IUser getUser();
    public void setUser(IUser user);
    public void setCard(ICard card);
    public ICard getCard();
    public void setAttackUser(IUser user);

    public IUser getAttackUser();

    public void setDefenseUser(IUser user);
    public IUser getDefenseUser();

    public IPoker getPoker();
    public void setPoker(IPoker poker);

    public float getBaseValue();

    public void setBaseValue(float baseValue);

    public IRoundResult getRoundResult();

    public void setRoundResult(IRoundResult value);
}
