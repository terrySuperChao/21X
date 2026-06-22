using System.Collections.Generic;

public class AdvancedEffectHandleMgr : Singleton<AdvancedEffectHandleMgr>
{
    public const int advancedEffectId3001 = 3001;
    public const int advancedEffectId3002 = 3002;
    public const int advancedEffectId3003 = 3003;
    public const int advancedEffectId3004 = 3004;
    public const int advancedEffectId3005 = 3005;
    public const int advancedEffectId3006 = 3006;
    public const int advancedEffectId3007 = 3007;
    public const int advancedEffectId3008 = 3008;
    public const int advancedEffectId3009 = 3009;
    public const int advancedEffectId3010 = 3010;
    public const int advancedEffectId3011 = 3011;
    public const int advancedEffectId3012 = 3012;
    public const int advancedEffectId3013 = 3013;
    public const int advancedEffectId3014 = 3014;
    public const int advancedEffectId3015 = 3015;
    public const int advancedEffectId3016 = 3016;
    public const int advancedEffectId3017 = 3017;
    public const int advancedEffectId3018 = 3018;
    public const int advancedEffectId3019 = 3019;
    public const int advancedEffectId3020 = 3020;
    public const int advancedEffectId3021 = 3021;
    public const int advancedEffectId3022 = 3022;
    public const int advancedEffectId3023 = 3023;
    public const int advancedEffectId3031 = 3031;
    public const int advancedEffectId3032 = 3032;
    public const int advancedEffectId3033 = 3033;
    public const int advancedEffectId3801 = 3801;
    public const int advancedEffectId3802 = 3802;
    public const int advancedEffectId3901 = 3901;
    public const int advancedEffectId3902 = 3902;
    public const int advancedEffectId3903 = 3903;
    public const int advancedEffectId3904 = 3904;

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
