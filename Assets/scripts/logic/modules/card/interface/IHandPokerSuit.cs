using System.Collections.Generic;
public interface IHandPokerSuit
{
    public bool addPoker(IPoker poker, int baseValue);
    public List<IPoker> getPokers();
    public int getBaseValue();
    public PokerSuit getSuit();
    public void clear();
}
