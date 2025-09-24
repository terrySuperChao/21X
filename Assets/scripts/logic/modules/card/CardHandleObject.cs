using System.Diagnostics;

public abstract class CardHandleObject:ICardHandle
{
    public bool handle(ICardHandlePara para) {
        return this._handle(para);
    }

    protected virtual bool _handle(ICardHandlePara para) {
        return true;
    }
}
