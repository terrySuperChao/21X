//�ƶ�

using System.Collections.Generic;

public class TriggerEvent
{
    private static List<ICard> _config = new List<ICard> { 
        
	};
   
	private static List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        
    };

    public static ITriggerHandle getTriggerEventHandle(int trigger)
    {
        return _handle.Find(handle => handle.getTrigger() == trigger);
    }
}
