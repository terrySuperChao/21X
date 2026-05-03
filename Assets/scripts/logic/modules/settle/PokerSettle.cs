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
    }
}