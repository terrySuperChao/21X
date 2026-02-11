//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class BarrierPokerPileMgr : Singleton<BarrierPokerPileMgr>
{
    private List<IPoker> _remainCards = new List<IPoker>();
    private List<IPoker> _playedTableCards = new List<IPoker>();

    public void deserialized()
    {
        RepeatedField<int> pokers = GamePropertyMgr.Instance.getGameData().Barrier.PokerPile;
        if (pokers != null && pokers.Count > 0)
        {
            for (int i = 0; i < pokers.Count; i++)
            {
                IPoker poker = new PokerObject();
                poker.setId(i);
                poker.setValue(pokers[i]);
                _remainCards.Add(poker);
            }
        }
        else
        {
            this.initDefautPokers();
            this.shuffle();
        }
    }

    public void serialized()
    {
        GamePropertyMgr.Instance.getGameData().Barrier.PokerPile.Clear();
        for (int i = 0; i < this._remainCards.Count; i++)
        {
            GamePropertyMgr.Instance.getGameData().Barrier.PokerPile.Add(this._remainCards[i].getValue());
        }
    }

    private void initDefautPokers() {
        for (int i = 0; i < GameConst.CARDS.Length; i++) {
            IPoker poker = new PokerObject();
            poker.setId(i);
            poker.setValue(GameConst.CARDS[i]);
            _remainCards.Add(poker);
        }
    }

    public void shuffle() {
        _remainCards.AddRange(_playedTableCards);
        _playedTableCards.Clear();

        for (int i = _remainCards.Count-1; i > 0 ; i--){
            int j = RandomMgr.Instance.getRangeInt(0,1) * (i+1);
            (_remainCards[i], _remainCards[j]) = (_remainCards[j], _remainCards[i]);
        }
    }

    public IPoker dealPoker()
    {
        return getPoker(0);
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
