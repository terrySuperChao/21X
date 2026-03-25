//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;

public class BasePokerPile
{
    private int _id = 1;
    private List<IPoker> _remainCards = new List<IPoker>();

    public void init(RepeatedField<int> pokers)
    {
        this._id = 1;
        this._remainCards.Clear();
        if (pokers != null && pokers.Count > 0)
        {
            for (int i = 0; i < pokers.Count; i++)
            {
                this._remainCards.Add(this.createPoker(pokers[i]));
            }
        }
        else
        {
            this.shuffle();
        }
    }

    public virtual List<IPoker> preShuffle(List<IPoker> allPoker) { 
        return new List<IPoker>();
    }

    public void shuffle() {
        List<IPoker> allPoker = new List<IPoker>();
        for (int i = 0; i < GameConst.CARDS.Length; i++)
        {
            allPoker.Add(this.createPoker(GameConst.CARDS[i]));
        }

        List<IPoker> prePoker = this.preShuffle(allPoker);
        for (int i = allPoker.Count-1; i > 0 ; i--){
            int j = RandomMgr.Instance.getRangeInt(0, i + 1);
            (allPoker[i], allPoker[j]) = (allPoker[j], allPoker[i]);
        }
        this._remainCards.Clear();
        this._remainCards.AddRange(prePoker);
        this._remainCards.AddRange(allPoker);
    }

    public IPoker getPoker(int index)
    {
        IPoker poker = null;
        if (_remainCards.Count > 0)
        {
            poker = _remainCards[index];
            poker.setBack(false);
            _remainCards.RemoveAt(index);
        }
        return poker;
    }

    public List<IPoker> getRemainCards()
    {
        return this._remainCards;
    }


    public IPoker createPoker(int value)
    {
        IPoker poker = new PokerObject();
        poker.setId(this._id++);
        poker.setValue(value);
        return poker;
    }
}
