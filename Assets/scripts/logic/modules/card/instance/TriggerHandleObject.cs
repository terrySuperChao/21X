public abstract class TriggerHandleObject : ITriggerHandle
{
    public TriggerEvent getTrigger() {
        return this._getTrigger();
    }

    //战斗开始
    public bool battleStartHandle(ITriggerHandlePara para) {
        return this._battleEndHandle(para);
    }
    //回合开始
    public bool turnStartHandle(ITriggerHandlePara para) {
        return this._turnStartHandle(para);
    }
    //初始发牌完成后
    public bool postInitialDrawHandle(ITriggerHandlePara para) {
        return this._postInitialDrawHandle(para);
    }
    //要牌后
    public bool postCardDrawHandle(ITriggerHandlePara para) {
        return this._postCardDrawHandle(para);
    }
    //停牌/最终点数确定后
    public bool postStandOrFinalScoreHandle(ITriggerHandlePara para) {
        return this._postStandOrFinalScoreHandle(para);
    }
    //牌局结果确定后
    public bool postBattleResultHandle(ITriggerHandlePara para) {
        return this._postBattleResultHandle(para);
    }
    //每次单花色属性转化后
    public bool postSuitAttributeConversionHandle(ITriggerHandlePara para) {
        return this._postSuitAttributeConversionHandle(para);
    }
    //每次单花色属性转化后
    public bool preActionHandle(ITriggerHandlePara para) {
        return this._preActionHandle(para);
    }
    //普通攻击结算后
    public bool postBasicAttackHandle(ITriggerHandlePara para) {
        return this._postBasicAttackHandle(para);
    }
    //主技能释放后
    public bool postMainSkillHandle(ITriggerHandlePara para) {
        return this._postMainSkillHandle(para);
    }
    //回合结束时
    public bool turnEndHandle(ITriggerHandlePara para)
    {
        return this._turnEndHandle(para);
    }
    //战斗结束/胜利结算
    public bool battleEndHandle(ITriggerHandlePara para) {
        return this._battleEndHandle(para);
    }
    //自定义事件（优先级全部为0）
    public bool customEventHandle(ITriggerHandlePara para) {
        return this._customEventHandle(para);
    }

    //对比逻辑中的数字
    protected bool compareLogic(string compareStr,float currentNum) {
        return GameUtils.compareNumber(compareStr, currentNum);
    }
    //正则表达式（保留小数点）
    protected string extractNumbersWithDecimal(string input)
    {
        return GameUtils.extractNumbersWithDecimal(input);
    }

    protected virtual TriggerEvent _getTrigger() { return 0; }
    //战斗开始
    protected virtual bool _battleStartHandle(ITriggerHandlePara para) { return false; }
    //回合开始
    protected virtual bool _turnStartHandle(ITriggerHandlePara para) { return false; }
    //初始发牌完成后
    protected virtual bool _postInitialDrawHandle(ITriggerHandlePara para) { return false; }
    //要牌后
    protected virtual bool _postCardDrawHandle(ITriggerHandlePara para) { return false; }
    //停牌/最终点数确定后
    protected virtual bool _postStandOrFinalScoreHandle(ITriggerHandlePara para) { return false; }
    //牌局结果确定后
    protected virtual bool _postBattleResultHandle(ITriggerHandlePara para) { return false; }
    //每次单花色属性转化后
    protected virtual bool _postSuitAttributeConversionHandle(ITriggerHandlePara para) { return false; }
    //每次单花色属性转化后
    protected virtual bool _preActionHandle(ITriggerHandlePara para) { return false; }
    //普通攻击结算后
    protected virtual bool _postBasicAttackHandle(ITriggerHandlePara para) { return false; }
    //主技能释放后
    protected virtual bool _postMainSkillHandle(ITriggerHandlePara para) { return false; }
    //回合结束时
    protected virtual bool _turnEndHandle(ITriggerHandlePara para) { return false; }
    //战斗结束/胜利结算
    protected virtual bool _battleEndHandle(ITriggerHandlePara para) { return false; }
    //自定义事件（优先级全部为0）
    protected virtual bool _customEventHandle(ITriggerHandlePara para) { return false; }
}
