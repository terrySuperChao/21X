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

    public float getBaseValue();
    public void setBaseValue(float baseValue);

    public IRoundResult getRoundResult();
    public void setRoundResult(IRoundResult value);

    public IAssembleCard getAssembleCard();
    public void setAssembleCard(IAssembleCard card);

    public void setBlackJock(bool isBlackJock);
    public bool isBlackJock();

    public void setMagicAttack(bool isMagicAttack);
    public bool isMagicAttack();
}
