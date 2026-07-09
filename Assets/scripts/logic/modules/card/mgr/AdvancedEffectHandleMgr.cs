using System.Collections.Generic;

public class AdvancedEffectHandleMgr : Singleton<AdvancedEffectHandleMgr>
{
    

    private List<IBaseEffectHandle> _advancedEffecthandle = new List<IBaseEffectHandle> {
        new AdvancedEffect3001(),
        new AdvancedEffect3002(),
        new AdvancedEffect3003(),
        new AdvancedEffect3011(),
        new AdvancedEffect3021(),
        new AdvancedEffect3031(),
        new AdvancedEffect3021(),
        new AdvancedEffect3031(),
        new AdvancedEffect3021(),
        new AdvancedEffect3022(),
        new AdvancedEffect3023(),
        new AdvancedEffect3031(),
        new AdvancedEffect3032(),
        new AdvancedEffect3033(),
        new AdvancedEffect3801(),
        new AdvancedEffect3802(),
        new AdvancedEffect3901(),
        new AdvancedEffect3902(),
        new AdvancedEffect3903(),
        new AdvancedEffect3904(),
    };

    public IBaseEffectHandle getAdvancedEffectHandle(ITriggerHandlePara para)
    {
        return getAdvancedEffectHandle(para.getAssembleCard().getBaseEffect().getId());
    }

    public IBaseEffectHandle getAdvancedEffectHandle(int id)
    {
        return _advancedEffecthandle.Find(handle => handle.getId() == id);
    }

}
