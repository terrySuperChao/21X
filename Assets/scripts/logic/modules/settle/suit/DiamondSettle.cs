public class DiamondSettle: SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value) {
        return GameDefenseMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), value, false);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.diamond; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker,float addValue) {
        addValue *= CardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.bonusArmor);
        if (addValue > 0) {
            GameDefenseMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue);
        }
    }
}