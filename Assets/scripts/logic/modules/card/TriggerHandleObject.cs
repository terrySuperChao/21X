using System;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;

public abstract class TriggerHandleObject : ITriggerHandle
{
    public TriggerEvent getTrigger() {
        return this._getTrigger();
    }

    public bool addNewCardAfterHandle(ITriggerHandlePara para)
    {
        return this._addNewCardAfterHandle(para);
    }

    public bool handPokerAfterHandle(ITriggerHandlePara para)
    {
        return this._handPokerAfterHandle(para);
    }

    public bool dealPokerAfterHandle(ITriggerHandlePara para)
    {
        return this._dealPokerAfterHandle(para);
    }

    public bool roundBeginHandle(ITriggerHandlePara para)
    {
        return this._roundBeginHandle(para);
    }
  
    public bool roundAddValueBeforeHandle(ITriggerHandlePara para)
    {
        return this._roundAddValueBeforeHandle(para);
    }

    public bool roundAddValueHandle(ITriggerHandlePara para)
    {
        return this._roundAddValueHandle(para);
    }

    public bool roundAddMagicHandle(ITriggerHandlePara para)
    {
        return this._roundAddMagicHandle(para);
    }

    public bool roundSpecialAttrHandle(ITriggerHandlePara para)
    {
        return this._roundSpecialAttrHandle(para);
    }
    
    public bool roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        return this._roundAttackBeforeHandle(para);
    }

    public bool roundAttackHandle(ITriggerHandlePara para)
    {
        return this._roundAttackHandle(para);
    }

    public bool roundMagicAttackHandle(ITriggerHandlePara para)
    {
        return this._roundMagicAttackHandle(para);
    }

    public bool roundSubDefenseHandle(ITriggerHandlePara para)
    {
        return this._roundSubDefenseHandle(para);
    }

    public bool roundSubBloodHandle(ITriggerHandlePara para)
    {
        return this._roundSubBloodHandle(para);
    }

    public bool roundAttackAfterHandle(ITriggerHandlePara para)
    {
        return this._roundAttackAfterHandle(para);
    }
    
    public bool roundEndHandle(ITriggerHandlePara para)
    {
        return this._roundEndHandle(para);
    }

    protected float getNumberDigits(float number)
    {
        return (float)Math.Round((number * 10 + 0.5) / 10, 1);
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
    protected virtual bool _addNewCardAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _handPokerAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _dealPokerAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundBeginHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAddValueBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAddValueHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAddMagicHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundSpecialAttrHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAttackBeforeHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAttackHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundMagicAttackHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundSubDefenseHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundSubBloodHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundAttackAfterHandle(ITriggerHandlePara para) { return false; }
    protected virtual bool _roundEndHandle(ITriggerHandlePara para) { return false; }

    
}
