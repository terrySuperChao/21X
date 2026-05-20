using System.Collections.Generic;

public class PokerSettle: IAttackSettle
{
    private ISuitSettle _suitSettle;
    public PokerSettle() {
        this._suitSettle = new SpadeSettle();
        this._suitSettle.setNextSuitSettle(new HeartSettle())
                        .setNextSuitSettle(new ClubSettle())
                        .setNextSuitSettle(new DiamondSettle());
    }

    public void settle(ITriggerHandlePara handlePara) {
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(handlePara.getAttackUser());
        List<int> values = PokerPointMgr.Instance.getPokerValue(pokers);
        for (int i = 0; i < pokers.Count; i++)
        {
            this._suitSettle.settle(handlePara, pokers[i], values[i]);
        }

        List<PokerSuit> suits = new List<PokerSuit>(){
             PokerSuit.spade,
             PokerSuit.heart,
             PokerSuit.club,
             PokerSuit.diamond,
        };
        for (int i = 0; i < suits.Count; i++) {
            int index = pokers.FindIndex(poker => poker.getSuit() == suits[i]);
            if (index != -1) {
                handlePara.setPokerSuit(suits[i]);
                CardMgr.Instance.handle(handlePara, TriggerEvent.transformAttribute);
            }
        }
    }
}