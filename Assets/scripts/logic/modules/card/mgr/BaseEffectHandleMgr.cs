using System.Collections.Generic;

public class BaseEffectHandleMgr : Singleton<BaseEffectHandleMgr>
{
    public const int baseEffectId2001 = 2001;
    public const int baseEffectId2002 = 2002;
    public const int baseEffectId2003 = 2003;
    public const int baseEffectId2004 = 2004;
    public const int baseEffectId2005 = 2005;
    public const int baseEffectId2006 = 2006;
    public const int baseEffectId2011 = 2011;
    public const int baseEffectId2012 = 2012;
    public const int baseEffectId2013 = 2013;
    public const int baseEffectId2014 = 2014;
    public const int baseEffectId2015 = 2015;
    public const int baseEffectId2021 = 2021;
    public const int baseEffectId2022 = 2022;
    public const int baseEffectId2023 = 2023;
    public const int baseEffectId2024 = 2024;
    public const int baseEffectId2025 = 2025;
    public const int baseEffectId2031 = 2031;
    public const int baseEffectId2032 = 2032;
    public const int baseEffectId2033 = 2033;
    public const int baseEffectId2034 = 2034;
    public const int baseEffectId2035 = 2035;

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
