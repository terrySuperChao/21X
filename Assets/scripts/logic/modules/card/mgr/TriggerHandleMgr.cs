using System.Collections.Generic;

public class TriggerHandleMgr : Singleton<TriggerHandleMgr>
{
    private List<ITriggerHandle> _triggerHandle = new List<ITriggerHandle> {
        new TriggerEffect1001(),
        new TriggerEffect1002(),
        new TriggerEffect1003(),
        new TriggerEffect1004(),
        new TriggerEffect1005(),

        new TriggerEffect1011(),
        new TriggerEffect1012(),
        new TriggerEffect1013(),
        new TriggerEffect1014(),
        new TriggerEffect1015(),

        new TriggerEffect1021(),
        new TriggerEffect1022(),
        new TriggerEffect1023(),
        new TriggerEffect1024(),
        new TriggerEffect1025(),

        new TriggerEffect1031(),
        new TriggerEffect1032(),
        new TriggerEffect1033(),
        new TriggerEffect1034(),
        new TriggerEffect1035(),

        new TriggerEffect1051(),
    };

    public ITriggerHandle getTriggerHandle(int id)
    {
        return this._triggerHandle.Find(handle => handle.getId() == id);
    }
}
