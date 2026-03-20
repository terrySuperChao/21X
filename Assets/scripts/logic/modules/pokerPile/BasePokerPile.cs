//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;

public class BasePokerPile
{
    private List<IPoker> _remainCards = new List<IPoker>();
    private List<IPoker> _playedTableCards = new List<IPoker>();

    public void init(RepeatedField<int> pokers)
    {
        this._remainCards.Clear();
        this._playedTableCards.Clear();
        if (pokers != null && pokers.Count > 0)
        {
            for (int i = 0; i < pokers.Count; i++)
            {
                this._remainCards.Add(this.createPoker(pokers[i]));
            }
        }
        else
        {
            this.initDefautPokers();
            this.shuffle();
        }
    }

    private void initDefautPokers() {
        for (int i = 0; i < GameConst.CARDS.Length; i++)
        {
            _remainCards.Add(this.createPoker(GameConst.CARDS[i]));
        }
    }

    public virtual List<IPoker> preShuffle(List<IPoker> allPoker) { 
        return new List<IPoker>();
    }

    public void shuffle() {
        List<IPoker> allPoker = new List<IPoker>();
        allPoker.AddRange(_remainCards);
        allPoker.AddRange(_playedTableCards);

        List<IPoker> prePoker = this.preShuffle(allPoker);
        for (int i = allPoker.Count-1; i > 0 ; i--){
            int j = RandomMgr.Instance.getRangeInt(0, i + 1);
            (allPoker[i], allPoker[j]) = (allPoker[j], allPoker[i]);
        }

        _remainCards.Clear();
        _playedTableCards.Clear();
        _remainCards.AddRange(prePoker);
        _remainCards.AddRange(allPoker);
    }

    public IPoker getPoker(int index)
    {
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

    public List<IPoker> getRemainCards()
    {
        return this._remainCards;
    }

    public List<IPoker> getPlayedTableCards()
    {
        return this._playedTableCards;
    }

    public IPoker createPoker(int value)
    {
        IPoker poker = new PokerObject();
        poker.setId(0);
        poker.setValue(value);
        return poker;
    }
}
