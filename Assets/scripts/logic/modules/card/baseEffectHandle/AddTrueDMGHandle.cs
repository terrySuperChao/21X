//获得 2/4 点真实伤害
public class AddTrueDMGHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_True_DMG";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddTrueDMGHandle=========>>");
        
        float addValue = this.getAddValue(para);
        para.getRoundResult(para.getAttackUser()).addHurtValue(addValue);
        GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), addValue);
    }
}
