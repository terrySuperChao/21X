//牌堆
using System;
using System.Collections.Generic;
using System.Reflection;

public class PokerPileMgr : Singleton<PokerPileMgr>
{
    public readonly static int[] CARDS = {
        102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,    // 方 2 ~ A
        202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214,    // 红
        302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314,    // 黑
        402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414     // 梅
    };

    private List<IPoker> _remainCards = new List<IPoker>();
    private List<IPoker> _playedTableCards = new List<IPoker>();
    public void init() {
        initDefautPokers();
    }

    private void initDefautPokers() {
        for (int i = 0; i < CARDS.Length; i++){
            IPoker poker = new PokerObject();
            poker.setId(i);
            poker.setValue(CARDS[i]);
            _remainCards.Add(poker);
        }
    }

    public void shuffle() {
        //交换
        Random rd = new Random();

        _remainCards.AddRange(_playedTableCards);
        _playedTableCards.Clear();

        for (int i = _remainCards.Count-1; i > 0 ; i--){
            int j = (int)Math.Floor(rd.NextDouble()*(i+1));
            (_remainCards[i], _remainCards[j]) = (_remainCards[j], _remainCards[i]);
        }
    }

    public IPoker dealPoker()
    {
        return getPoker(0);
    }

    public int getRemainCards() {
        return _remainCards.Count;
    }

    //发指定花色的牌
    public IPoker dealSuitPoker(int suit) {
        IPoker poker = null;
        if (_remainCards.Count > 0)
        {
            int index = 0;
            for (int i = 0; i < _remainCards.Count; i++) {
                if (_remainCards[i].getSuit() == suit) {
                    index = i;
                    break;
                }
            }
            poker = getPoker(index);
        }
        return poker;
    }

    private IPoker getPoker(int index) {
        IPoker poker = null;
        if (_remainCards.Count > 0)
        {
            poker = _remainCards[index];
            poker.setBack(false);
            _playedTableCards.Add(poker);
            _remainCards.RemoveAt(index);
        }
        return poker;
    }
}
