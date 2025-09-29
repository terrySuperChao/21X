using System.Diagnostics;

public abstract class CardHandleObject:ICardHandle
{
    public void addValueHandle(ICardHandlePara para) {
        this._addValueHandle(para);
    }

    public void penetrateHanle(ICardHandlePara para)
    {
        this._penetrateHanle(para);
    }

    protected virtual void _addValueHandle(ICardHandlePara para) {
        
    }

    protected virtual void _penetrateHanle(ICardHandlePara para)
    {

    }
}
