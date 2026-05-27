using System.Collections.Generic;

public class PokerSettle: IAttackSettle
{
    private ISuitSettle _suitSettle;
    private List<IHandPokerSuit> _handPokerSuits = new List<IHandPokerSuit>();
    public PokerSettle() {
        this._suitSettle = new SpadeSettle();
        this._suitSettle.setNextSuitSettle(new HeartSettle())
                        .setNextSuitSettle(new ClubSettle())
                        .setNextSuitSettle(new DiamondSettle());

        this._handPokerSuits.Add(new HandPokerSuitObject(PokerSuit.spade));
        this._handPokerSuits.Add(new HandPokerSuitObject(PokerSuit.heart));
        this._handPokerSuits.Add(new HandPokerSuitObject(PokerSuit.club));
        this._handPokerSuits.Add(new HandPokerSuitObject(PokerSuit.diamond));
    }

    public void settle(ITriggerHandlePara handlePara) {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(handlePara.getAttackUser());
        List<int> values = PokerPointMgr.Instance.getPokerValue(pokers);
        for (int i = 0; i < this._handPokerSuits.Count; i++)
        {
            this._handPokerSuits[i].clear();
        }

        for (int i = 0; i < pokers.Count; i++)
        {
            for (int j = 0; j < this._handPokerSuits.Count; j++)
            {
                if (this._handPokerSuits[j].addPoker(pokers[i], values[i]))
                {
                    break;
                }
            }
        }

        for (int i = 0; i < this._handPokerSuits.Count; i++)
        {
            if (this._handPokerSuits[i].getBaseValue() > 0) {
                this._suitSettle.settle(handlePara, this._handPokerSuits[i]);
            }
        }
    }
}