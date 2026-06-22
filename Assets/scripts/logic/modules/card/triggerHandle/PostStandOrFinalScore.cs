using System.Text.RegularExpressions;
using System.Collections.Generic;
using static UnityEngine.UIElements.UxmlAttributeDescription;
//总点数在 [17, 21] 之间
public class PostStandOrFinalScore : TriggerHandleObject
{
    protected override bool _postStandOrFinalScoreHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Stop Poker After Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "总点数在[,]之间";
        string input = Regex.Replace(logic, @"\d", "").Replace(" ", "");
        if (input.IndexOf(str) != 0)//替换数字与空格
        {
            return false;
        }

        MatchCollection matches = Regex.Matches(logic, @"\d+");
        if (matches.Count != 2)
        {
            return false;
        }

        List<int> points = new List<int>();
        foreach(Match m in matches) {
            points.Add(int.Parse(m.Value));
        }

        if (points.Count != 2) {
            return false;
        }

        FightDealType type = para.getAttackUser().isNpc() ? FightDealType.npc : FightDealType.player;
        List<IPoker> pokers = FightDataMgr.Instance.getPokers(type);
        int index = pokers.FindIndex(poker => poker.getSuit() == PokerSuit.diamond);
        if (index == -1) {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        if (points[0] <= point && points[1] >= point) {
            return true;
        }

        return false;
    }
}
