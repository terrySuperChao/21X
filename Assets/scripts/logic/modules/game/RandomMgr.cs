using Pb;
public class RandomMgr : Singleton<RandomMgr>
{
    private RandomEx _rd = null;

    public void deserialized(GameData data)
    {
        int initSeed = data.InitSeed;
        this._rd = new RandomEx(initSeed);
    }

    public void serialized(GameData data)
    {
        data.InitSeed = this._rd.getSeed();
    }

    public void init(int initSeed) { 
        this._rd = new RandomEx(initSeed);
    }
    /**
      * �����������, Ĭ�Ϸ�Χ[min, max)
      * @param type ���ӵ�����
      * @param min ��Сֵ
      * @param max ���ֵ, ��������ֵ
      * @returns ���������
     */
    public int getRangeInt(int min, int max) {
        return this._rd.rangeInt(min, max);
    }
}
