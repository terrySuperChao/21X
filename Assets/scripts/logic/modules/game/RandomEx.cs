using System;
using UnityEngine;

public class RandomEx
{
    /** 随机种子 */
    private long _seed  = 0;

    public RandomEx(int seed = 123456)
    {
        this._seed = seed;
    }

    /**
     * 使用 Hull-Dobell 算法的线性同余生成器构造伪随机数
     */
    private double _random()
    {
        this._seed = (this._seed * 9301 + 49297) % 233280;
        return this._seed / 233280.0;
    }

    /** 返回最小(包含)和最大(不包含)之间的整点伪随机数 */
    public int rangeInt(int min, int max){
        return (int)this.range(min, max);
    }

    /** 返回最小(包含)和最大(不包含)之间的浮点伪随机数 */
    public double range(int min = 0, int max = 1) {
        return (this._random() * (max - min) + min);
    }

    public int getSeed() {
        return (int)this._seed;
    }
}

