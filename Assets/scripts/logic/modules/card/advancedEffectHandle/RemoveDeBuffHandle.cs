//随机移除自身 1 个/层负面状态
public class RemoveDeBuffHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Remove_DeBuff";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("RemoveDeBuffHandle=========>>");
        float addValue = -1.0f;
        para.getAttackUser().getExtraInfo().setAddBleeding(addValue);
    }
}
