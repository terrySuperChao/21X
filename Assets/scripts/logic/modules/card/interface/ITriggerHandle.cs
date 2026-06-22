public interface ITriggerHandle
{
    public TriggerEvent getTrigger();
    //战斗开始
    public bool battleStartHandle(ITriggerHandlePara para);
    //回合开始
    public bool turnStartHandle(ITriggerHandlePara para);
    //初始发牌完成后
    public bool postInitialDrawHandle(ITriggerHandlePara para);
    //要牌后
    public bool postCardDrawHandle(ITriggerHandlePara para);         
    //停牌/最终点数确定后
    public bool postStandOrFinalScoreHandle(ITriggerHandlePara para);
    //牌局结果确定后
    public bool postBattleResultHandle(ITriggerHandlePara para);                                                     
    //每次单花色属性转化后
    public bool postSuitAttributeConversionHandle(ITriggerHandlePara para);
    //每次单花色属性转化后
    public bool preActionHandle(ITriggerHandlePara para);
    //普通攻击结算后
    public bool postBasicAttackHandle(ITriggerHandlePara para);
    //主技能释放后
    public bool postMainSkillHandle(ITriggerHandlePara para);
    //回合结束时
    public bool turnEndHandle(ITriggerHandlePara para);
    //战斗结束/胜利结算
    public bool battleEndHandle(ITriggerHandlePara para);
    //自定义事件（优先级全部为0）
    public bool customEventHandle(ITriggerHandlePara para);
}
