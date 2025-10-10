public interface ICardHandle
{
    //要牌
    public void dealPokerHandle(ICardHandlePara para);
    //回合前
    public void roundBeginHandle(ICardHandlePara para);
    //回合添加值
    public void roundAddValueHandle(ICardHandlePara para);
    //回合添加魔法值
    public void roundAddMagicHandle(ICardHandlePara para);
    //回合攻击前
    public void roundAttackBeginHandle(ICardHandlePara para);
    //攻击
    public void roundAttackHandle(ICardHandlePara para);
    //魔法攻击
    public void roundMagicAttackHandle(ICardHandlePara para);
    //扣护甲
    public void roundSubDefenseHandle(ICardHandlePara para);
    //扣血
    public void roundSubBloodHandle(ICardHandlePara para);
    //回合攻击后
    public void roundAttackAfterHandle(ICardHandlePara para);
    //回合结束
    public void roundEndHandle(ICardHandlePara para);
}
