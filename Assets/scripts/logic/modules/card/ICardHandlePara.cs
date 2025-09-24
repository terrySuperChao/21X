public interface ICardHandlePara
{
    public void setCard(ICard card);
    public ICard getCard();
    public IUser getAttackUser();

    public IUser getDefenseUser();

    public IPoker getPoker();
    public void setPoker(IPoker poker);
}
