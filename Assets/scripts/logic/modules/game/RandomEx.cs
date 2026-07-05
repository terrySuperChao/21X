public class RandomEx
{
    /** ������� */
    private long _seed  = 0;

    public RandomEx(int seed = 123456)
    {
        this._seed = seed;
    }

    /**
     * ʹ�� Hull-Dobell �㷨������ͬ������������α�����
     */
    private double _random()
    {
        this._seed = (this._seed * 9301 + 49297) % 233280;
        return this._seed / 233280.0;
    }

    /** ������С(����)�����(������)֮�������α����� */
    public int rangeInt(int min, int max){
        return (int)this.range(min, max);
    }

    /** ������С(����)�����(������)֮��ĸ���α����� */
    public double range(int min = 0, int max = 1) {
        return (this._random() * (max - min) + min);
    }

    public int getSeed() {
        return (int)this._seed;
    }
}

