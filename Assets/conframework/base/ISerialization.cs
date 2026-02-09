public interface ISerialization
{
    /**
     * 序列化
     */
    public object serialized();
    /**
     * 反序列化
     */
    public void deserialized(object data);
}
