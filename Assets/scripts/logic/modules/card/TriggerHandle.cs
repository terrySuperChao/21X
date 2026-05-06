using System.Collections.Generic;

public class TriggerHandle
{
    private static List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        new SettlementPointHandle(),
        new SettlementIncludePokerHandle(),
        new SettlementAbortionHandle(),
        new TransformAttributeHandle(),
        new TransformAttributeRatioHandle(),
    };

    public static List<ITriggerHandle> getTriggerHandle(ITriggerHandlePara para)
    {
        return _handle.FindAll(handle => (int)handle.getTrigger() == para.getAssembleCard().getTrigger().getTriggerEvent());
    }
}
