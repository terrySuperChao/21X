using System.Collections.Generic;

public class BaseEffectHandleMgr : Singleton<BaseEffectHandleMgr>
{
    

    private List<IBaseEffectHandle> _baseEffectHandle = new List<IBaseEffectHandle> {
        new BaseEffect2001(),
        new BaseEffect2002(),
        new BaseEffect2003(),
        new BaseEffect2004(),
        new BaseEffect2005(),
        new BaseEffect2011(),
        new BaseEffect2012(),
        new BaseEffect2013(),
        new BaseEffect2014(),
        new BaseEffect2015(),
        new BaseEffect2021(),
        new BaseEffect2022(),
        new BaseEffect2023(),
        new BaseEffect2024(),
        new BaseEffect2025(),
        new BaseEffect2031(),
        new BaseEffect2032(),
        new BaseEffect2033(),
        new BaseEffect2034(),
        new BaseEffect2035(),
    };

    

    public IBaseEffectHandle getBaseEffectHandle(ITriggerHandlePara para)
    {
        return getBaseEffectHandle(para.getAssembleCard().getBaseEffect().getId());
    }

    public IBaseEffectHandle getBaseEffectHandle(int id)
    {
        return _baseEffectHandle.Find(handle => handle.getId() == id);
    }
}
