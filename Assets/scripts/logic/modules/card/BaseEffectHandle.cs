using System.Collections.Generic;

public class BaseEffectHandle
{
	private static List<IBaseEffectHandle> _baseEffectHandle = new List<IBaseEffectHandle> {
        new AddAMPHandle(),
        new AddArmorHandle(),
        new AddATKHandle(),
        new AddCritHandle(),
        new AddHealHandle(),
        new AddMPPerHandle(),
        new AddTrueDMGHandle(),
        new ArmorToATKHandle(),
        new BonusArmorHandle(),
        new HealOverTimeHandle(),
        new HealSuperHandle(),
        new HealToMPHandle(),
        new LifeStealHandle(),
        new MPMaxSubHandle(),
        new MPRegenHandle(),
        new MultATKHandle(),
        new ReflectDMGHandle(),
        new SkillDamageUpHandle(),
        new SubArmorHandle(),
        new TemporaryArmorHandle(),
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
