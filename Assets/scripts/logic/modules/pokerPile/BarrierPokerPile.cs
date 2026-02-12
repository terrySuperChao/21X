//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using System.Collections.Generic;

public class BarrierPokerPile
{
    private List<IPoker> _remainCards = new List<IPoker>();
    private List<IPoker> _playedTableCards = new List<IPoker>();

    public void init(RepeatedField<int> pokers)
    {
        if (pokers != null && pokers.Count > 0)
        {
            for (int i = 0; i < pokers.Count; i++)
            {
                _remainCards.Add(this.createPoker(pokers[i]));
            }
        }
        else
        {
            this.initDefautPokers();
            this.shuffle();
        }
    }

    private void initDefautPokers() {
        for (int i = 0; i < GameConst.CARDS.Length; i++) {
            _remainCards.Add(this.createPoker(GameConst.CARDS[i]));
        }
    }

    public void shuffle()
    {
        List<IPoker> randomList = new List<IPoker>();
        randomList.AddRange(_remainCards);
        randomList.AddRange(_playedTableCards);

        List<IPoker> publicList = new List<IPoker>(); //¹«¹²ÅÆ
        while (true)
        {
            PokerSuit suit = PokerSuit.spade;
            int j = RandomMgr.Instance.getRangeInt(0, 750);
            if (j <= 400) //ºÚÌÒ
            {
                suit = PokerSuit.spade;
            }
            else if (j <= 500)//ºìÌÒ
            {
                suit = PokerSuit.heart;
            }
            else if (j <= 600)//Ã·»¨
            {
                suit = PokerSuit.club;
            }
            else if (j <= 750)//·½¿é
            {
                suit = PokerSuit.diamond;
            }

            List<IPoker> temp = new List< IPoker >();
            for (int i = 0; i < randomList.Count; i++)
            {
                if (suit == (PokerSuit)randomList[i].getSuit())
                {
                    temp.Add(randomList[i]);
                }
            }
            if (temp.Count > 0) {
                int m = RandomMgr.Instance.getRangeInt(0, temp.Count);
                publicList.Add(temp[m]);
                randomList.Remove(temp[m]);
            }

            if (randomList.Count == 0 || publicList.Count == 3) {
                break;
            }
        }

        for (int i = randomList.Count - 1; i > 0; i--)
        {
            int j = RandomMgr.Instance.getRangeInt(0, 1) * (i + 1);
            (randomList[i], randomList[j]) = (randomList[j], randomList[i]);
        }

        _remainCards.Clear();
        _playedTableCards.Clear();
        _remainCards.AddRange(publicList);
        _remainCards.AddRange(randomList);
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

    public List<IPoker> getRemainCards() { 
        return this._remainCards;
    }

    public IPoker createPoker(int value) {
        IPoker poker = new PokerObject();
        poker.setId(0);
        poker.setValue(value);
        return poker;
    }
}
