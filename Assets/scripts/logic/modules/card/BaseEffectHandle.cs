using System.Collections.Generic;

public class BaseEffectHandle
{
	private static List<IBaseEffectHandle> _baseEffectHandle = new List<IBaseEffectHandle> {
        new AddATKHandle(),
    };

    private static List<IBaseEffectHandle> _advancedEffecthandle = new List<IBaseEffectHandle> {
        
    };

    public static IBaseEffectHandle getBaseEffectHandle(ITriggerHandlePara para)
    {
        return _baseEffectHandle.Find(handle => handle.getActionGenre() == para.getAssembleCard().getBaseEffect().getActionGenre());
    }

    public static IBaseEffectHandle getAdvancedEffectHandle(ITriggerHandlePara para) {
        return _advancedEffecthandle.Find(handle => handle.getActionGenre() == para.getAssembleCard().getBaseEffect().getActionGenre());
    }
}
