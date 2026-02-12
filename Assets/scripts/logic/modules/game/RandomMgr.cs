
using Pb;
using System.Collections.Generic;

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
        data.InitSeed = this._rd.seed;
    }

    public void init(int initSeed) { 
        this._rd = new RandomEx(initSeed);
    }
    /**
      * 返回随机整数, 默认范围[min, max)
      * @param type 种子的类型
      * @param min 最小值
      * @param max 最大值, 不包括此值
      * @returns 随机的整数
     */
    public int getRangeInt(int min, int max) {
        return this._rd.rangeInt(min, max);
    }
}
