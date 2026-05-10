using System.Collections.Generic;

public class TriggerHandle
{
    private static List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        new DealPokerAfterHandle(),
        new StopPokerAfterHandle(),
        new SettlementWinPointHandle(),
        new SettlementLossPointHandle(),
        new SettlementIncludePokerHandle(),
        new SettlementAbortionHandle(),
        new TransformAttributeHandle(),
        new TransformAttributeRatioHandle(),
        new AttackBeforeLossHandle(),
        new AttackBeforeWinHandle(),
        new NormalAttackAfterHandle(),
        new MagicAttackAfterHandle(),
        new SettlementAfterHandle(),
        new RoundOtherHandle(),
    };

    public static List<ITriggerHandle> getTriggerHandle(ITriggerHandlePara para)
    {
        return _handle.FindAll(handle => (int)handle.getTrigger() == para.getAssembleCard().getTrigger().getTriggerEvent());
    }
}
