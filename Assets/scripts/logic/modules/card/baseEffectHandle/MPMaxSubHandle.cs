//本局技能释放所需 MP 减少 %s
public class MPMaxSubHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "MP_Max_Sub";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddAMPHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setMpMaxSub(addValue);
    }
}
