public interface IUICommonAttackPara
{
    public IUser getAttackUser();

    public IUser getDefenseUser();

    public float getAttack();
    public float getBlood();
    public float getDefense();
    public float getFinalBlood();
    public float getFinalDefense();

    public bool isMagicAttack();
}
