using System;
using System.Collections.Generic;

public class RandomMgr : Singleton<RandomMgr>
{
    private Random _rd = new Random();

    /**
      * 返回随机整数, 默认范围[min, max)
      * @param type 种子的类型
      * @param min 最小值
      * @param max 最大值, 不包括此值
      * @returns 随机的整数
     */
    public int getRangeInt(int min, int max) {
        return _rd.Next(min, max);
    }
}
