public interface IBaseEffectHandle
{
    public int getId();
    public void handle(ITriggerHandlePara para);
    public void effect(IBaseEffectHandlePara para);
    public float getAdvancedValue(ITriggerHandlePara para);
}
