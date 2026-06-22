using System.Collections.Generic;
public class HandPokerSuitObject:IHandPokerSuit
{
    private int _baseValue = 0;
    private PokerSuit _suit = PokerSuit.club;
    private List<IPoker> _pokers = new List<IPoker>();

    public HandPokerSuitObject(PokerSuit suit) {
        this._suit = suit;
    }

    public PokerSuit getSuit()
    {
        return this._suit;
    }

    public bool addPoker(IPoker poker, int baseValue) {
        if (this._suit == poker.getSuit()) {
            this._pokers.Add(poker);
            this._baseValue += baseValue;
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<IPoker> getPokers() {
        return this._pokers;
    }

    public int getBaseValue() {
        return this._baseValue;
    }

    public void clear() {
        this._baseValue = 0;
        this._pokers.Clear();
    }
}
