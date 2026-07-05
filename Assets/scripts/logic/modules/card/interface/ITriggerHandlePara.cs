public interface ITriggerHandlePara
{
    public void setAttackUser(IUser user);
    public IUser getAttackUser();

    public void setDefenseUser(IUser user);
    public IUser getDefenseUser();

    public void setPokerSuit(PokerSuit pokerSuit);
    public PokerSuit getPokerSuit();

    public void setMagicAttack(bool isMagicAttack);
    public bool isMagicAttack();

    public void setGameSettlePara(IGameSettlePara para);
    public IGameSettlePara getGameSettlePara();

    public IAssembleCard getAssembleCard();
    public void setAssembleCard(IAssembleCard card);

    public void reset();
}
