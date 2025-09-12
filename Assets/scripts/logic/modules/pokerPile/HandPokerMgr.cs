//ÅÆ¶Ñ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPokerMgr : Singleton<HandPokerMgr>
{
    private Dictionary<string,List<IPoker>> _handPokerDic = new Dictionary<string, List<IPoker>>();

    public void init() {
       
    }

    public void addHandPoker(IUser user,IPoker poker) {
        if (user == null || poker == null) {
            return;
        }
        if (_handPokerDic.ContainsKey(user.getUserId()))
        {
            _handPokerDic[user.getUserId()].Add(poker);
        }
        else {
            _handPokerDic[user.getUserId()] = new List<IPoker> { poker };
        }
    }

    public List<IPoker> getHandPoker(IUser user) {
        if (user == null || !_handPokerDic.ContainsKey(user.getUserId())) {
            return null;
        }
        else {
            return _handPokerDic[user.getUserId()];
        }       
    }

    public void clearHandPoker(IUser user) {
        if (user == null)
        {
            return;
        }
        else
        {
            _handPokerDic.Remove(user.getUserId());
        }
    }

    public void resetHandPoker()
    {
        _handPokerDic.Clear();
    }

    public int getHandPokerPoint(IUser user,bool isFilter) {
        if (user == null || !_handPokerDic.ContainsKey(user.getUserId())){
            return 0;
        }else{
            int point = 0;
            List<IPoker> list = _handPokerDic[user.getUserId()];
            List<IPoker> APokers = new List<IPoker>();
            for (int i = 0; i < list.Count; i++) {
                if (isFilter && list[i].isBack()) continue;
                if (list[i].getRank() == 14)
                {
                    APokers.Add(list[i]);
                }
                else if (list[i].getRank() == 10 ||
                    list[i].getRank() == 11 ||
                    list[i].getRank() == 12 ||
                    list[i].getRank() == 13)
                {
                    point += 10;
                }
                else {
                    point += list[i].getRank();
                }
            }

            int remainPoint = 21 - point;
            for (int i = 0; i < APokers.Count; i++) {
                if (remainPoint >= 11 && remainPoint - 11 >= ((APokers.Count - 1) - i))
                {
                    point += 11;
                    remainPoint -= 11;
                }
                else {
                    point += 1;
                    remainPoint -= 1;
                }
            }
            return point;
        }
    }

    public bool isBlackJack(IUser user) {
        if (!_handPokerDic.ContainsKey(user.getUserId())) 
            return false;
       
        int poker10 = 0;
        int pokerJ = 0;
        int pokerQ = 0;
        int pokerK = 0;
        int pokerA = 0;
        List<IPoker> list = _handPokerDic[user.getUserId()];
        for (int i = 0; i < list.Count; i++) {
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
            if(poker10 > 0) number++;
            if(pokerJ > 0) number++;
            if(pokerQ > 0) number++;
            if(pokerK > 0) number++;
            return number == 1;
        }
        else {
            return false;
        }
        
    }
}
