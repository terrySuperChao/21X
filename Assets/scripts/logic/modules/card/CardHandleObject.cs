using System;
using System.Diagnostics;

public abstract class CardHandleObject:ICardHandle
{
    public void addValueHandle(ICardHandlePara para) {
        this._addValueHandle(para);
    }

    public void addRoundValueHanle(ICardHandlePara para)
    {
        this._addRoundValueHanle(para);
    }

    protected virtual void _addValueHandle(ICardHandlePara para) {
        
    }

    protected virtual void _addRoundValueHanle(ICardHandlePara para)
    {

    }

    //保留一位小数
    protected float getNumberDigits(float number) {
        return (float)Math.Round(number, 1);
    }
}
