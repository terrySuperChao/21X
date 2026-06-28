public class DiamondSettle: SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, IUser defenseUser, float value) {
        return GameDefenseMgr.Instance.handle(attackUser, defenseUser, value, false);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.diamond; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _settle(ITriggerHandlePara para, IHandPokerSuit handPoker) {
        float bonusArmor = CardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.bonusArmor);
        float addValue = bonusArmor * handPoker.getBaseValue();
        GameDefenseMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue);
    }
}