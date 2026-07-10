public class DiamondSettle: SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value, out float outValue) {
        return GameDefenseMgr.Instance.handle(para.getAttackUser(), value, out outValue);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.diamond; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker,float addValue) {
        GameDefenseMgr.Instance.execAdvancedEffectHandle(para, addValue);

        float bonusArmor = GameCardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.bonusArmor);
        if (bonusArmor > 0) {
            GameDefenseMgr.Instance.handle(para, addValue * bonusArmor);
        }
    }
}