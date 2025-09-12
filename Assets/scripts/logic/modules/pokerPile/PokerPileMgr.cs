//ÅÆ¶Ñ
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PokerPileMgr : Singleton<PokerPileMgr>
{
    public readonly static int[] CARDS = {
        102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,    // ·½ 2 ~ A
        202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214,    // ºì
        302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314,    // ºÚ
        402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414     // Ã·
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
        //½»»»
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
        IPoker poker = null;
        if (_remainCards.Count > 0){
            poker = _remainCards[0];
            poker.setBack(false);
            _playedTableCards.Add(poker);
            _remainCards.RemoveAt(0);
        }
        return poker;
    }

    public int getRemainCards() {
        return _remainCards.Count;
    }
}
