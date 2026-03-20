//ÅÆ¶Ñ
using System.Collections.Generic;
using UnityEngine;

public class PokerPointMgr : Singleton<PokerPointMgr>
{
    public int getPokerPoint(List<IPoker> list)
    {
        List<int> values = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            values.Add(list[i].getValue());
        }
        return this.getPokerPoint(values);
    }

    public int getPokerPoint(List<int> list)
    {
        int point = 0;
        List<int> APokers = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            int rank = list[i] % 100;
            if (rank == 14)
            {
                APokers.Add(rank);
            }
            else if (rank == 10 ||
                     rank == 11 ||
                     rank == 12 ||
                     rank == 13)
            {
                point += 10;
            }
            else
            {
                point += rank;
            }
        }

        int remainPoint = 21 - point;
        for (int i = 0; i < APokers.Count; i++)
        {
            if (remainPoint >= 11 && remainPoint - 11 >= ((APokers.Count - 1) - i))
            {
                point += 11;
                remainPoint -= 11;
            }
            else
            {
                point += 1;
                remainPoint -= 1;
            }
        }
        return point;
    }

    public bool isBlackJack(List<IPoker> list)
    {
        int poker10 = 0;
        int pokerJ = 0;
        int pokerQ = 0;
        int pokerK = 0;
        int pokerA = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].getRank() == 10)
            {
                poker10++;
            }
            else if (list[i].getRank() == 11)
            {
                pokerJ++;
            }
            else if (list[i].getRank() == 12)
            {
                pokerQ++;
            }
            else if (list[i].getRank() == 13)
            {
                pokerK++;
            }
            else if (list[i].getRank() == 14)
            {
                pokerA++;
            }
        }

        int number = 0;
        if (pokerA == 1)
        {
            if (poker10 > 0) number++;
            if (pokerJ > 0) number++;
            if (pokerQ > 0) number++;
            if (pokerK > 0) number++;
            return number == 1;
        }
        else
        {
            return false;
        }

    }

    public bool isBlackJack(List<int> list)
    {
        int poker10 = 0;
        int pokerJ = 0;
        int pokerQ = 0;
        int pokerK = 0;
        int pokerA = 0;
        for (int i = 0; i < list.Count; i++)
        {
            int rank = list[i] % 100;
            if (rank == 10)
            {
                poker10++;
            }
            else if (rank == 11)
            {
                pokerJ++;
            }
            else if (rank == 12)
            {
                pokerQ++;
            }
            else if (rank == 13)
            {
                pokerK++;
            }
            else if (rank == 14)
            {
                pokerA++;
            }
        }

        int number = 0;
        if (pokerA == 1 && list.Count == 2)
        {
            if (poker10 > 0) number++;
            if (pokerJ > 0) number++;
            if (pokerQ > 0) number++;
            if (pokerK > 0) number++;
            return number == 1;
        }
        else
        {
            return false;
        }

    }
}
