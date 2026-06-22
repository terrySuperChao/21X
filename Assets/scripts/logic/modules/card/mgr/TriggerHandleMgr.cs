using System.Collections.Generic;

public class TriggerHandleMgr : Singleton<TriggerHandleMgr>
{
    private List<ITriggerHandle> _handle = new List<ITriggerHandle> {
        new CustomEventHandle(),
        new PostBasicAttackHandle(),
        new PostBattleResultHandle(),
        new PostCardDrawHandle(),
        new PostMainSkillHandle(),
        new PostStandOrFinalScore(),
        new PostSuitAttributeConversionHandle(),
        new PreActionHandle(),
        new TurnEndHandle(),
    };

    public List<ITriggerHandle> getTriggerHandle(ITriggerHandlePara para)
    {
        return _handle;
    }
}
