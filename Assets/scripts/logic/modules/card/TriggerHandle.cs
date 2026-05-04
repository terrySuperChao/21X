using System.Collections.Generic;

public class TriggerHandle
{
	private static List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        new TransformAttributeHandle(),
    };

    public static ITriggerHandle getTriggerHandle(ITriggerHandlePara para)
    {
        return _handle.Find(handle => (int)handle.getTrigger() == para.getAssembleCard().getTrigger().getTriggerEvent());
    }
}
