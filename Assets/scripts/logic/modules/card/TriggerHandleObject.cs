using System;
using System.Text.RegularExpressions;

public abstract class TriggerHandleObject : ITriggerHandle
{
    public TriggerEvent getTrigger() {
        return this._getTrigger();
    }

    public bool initPokerBeforeHandle(ITriggerHandlePara para) {
        return this._initPokerBeforeHandle(para);
    }

    public bool dealPokerBeforeHandle(ITriggerHandlePara para) {
        return this._dealPokerBeforeHandle(para);
    }

    public bool dealPokerAfterHandle(ITriggerHandlePara para) {
        return this._dealPokerAfterHandle(para);
    }

    public bool stopPokerAfterHandle(ITriggerHandlePara para) {
        return this._stopPokerAfterHandle(para);
    }

    public bool settlementBeforeHandle(ITriggerHandlePara para) {
        return this._settlementBeforeHandle(para);
    }

    public bool transformAttributeHandle(ITriggerHandlePara para) {
        return this._transformAttributeHandle(para);
    }

    public bool roundAttackBeforeHandle(ITriggerHandlePara para) {
        return this._roundAttackBeforeHandle(para);
    }

    public bool normalAttackAfterHandle(ITriggerHandlePara para) {
        return this._normalAttackAfterHandle(para);
    }

    public bool magicAttackAfterHandle(ITriggerHandlePara para){
        return this._magicAttackAfterHandle(para);
    }

    public bool roundAttackAfterHandle(ITriggerHandlePara para) {
        return this._roundAttackAfterHandle(para);
    }

    public bool roundOtherHandle(ITriggerHandlePara para) {
        return this._roundOtherHandle(para);
    }

    //对比逻辑中的数字
    protected bool compareLogic(string compareStr,float currentNum) {
        string targetStr = this.extractNumbersWithDecimal(compareStr);
        int index = compareStr.IndexOf(targetStr);
        if (index == -1) return false;

        bool success = false;
        float targetNum = float.Parse(targetStr);
        string symbol = compareStr.Substring(0, index).Trim();
        UnityEngine.Debug.Log(string.Format("targetNum={0},currentNum={1}", targetNum, currentNum));
        switch (symbol)
        {
            case ">":
                if (currentNum > targetNum)
                {
                    success = true;
                }
                break;
            case ">=":
                if (currentNum >= targetNum)
                {
                    success = true;
                }
                break;
            case "=":
                if (currentNum == targetNum)
                {
                    success = true;
                }
                break;
            case "<=":
                if (currentNum <= targetNum)
                {
                    success = true;
                }
                break;
            case "<":
                if (currentNum < targetNum)
                {
                    success = true;
                }
                break;
            default:
                break;
        }
        return success;
    }
    //正则表达式（保留小数点）
    protected string extractNumbersWithDecimal(string input)
    {
        return Regex.Replace(input, @"[^\d.]", "");
    }

    protected virtual TriggerEvent _getTrigger() { return 0; }

    protected virtual bool _initPokerBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _dealPokerBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _dealPokerAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _stopPokerAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _settlementBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _transformAttributeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAttackBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _normalAttackAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _magicAttackAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAttackAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundOtherHandle(ITriggerHandlePara para) { return false; }
}
