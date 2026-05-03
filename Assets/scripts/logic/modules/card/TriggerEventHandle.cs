using System.Collections.Generic;

public class TriggerEventHandle
{
	private static List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        new TransformAttributeHandle(),
    };

    public static ITriggerHandle getTriggerEventHandle(int trigger)
    {
        return _handle.Find(handle => (int)handle.getTrigger() == trigger);
    }
}
