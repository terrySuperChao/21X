using System.Collections.Generic;
using UnityEngine;

public class BarrierReqMgr : Singleton<BarrierReqMgr>
{
    public void requestNewBarrier() {
        List<BarrierDealType> typeList = new List<BarrierDealType> { BarrierDealType.npc, BarrierDealType.player };
        for (int i = 0; i < typeList.Count; i++) {
            BarrierDealType type = typeList[i];
            for (int j = 0; j < 3; j++)
            {
                BarrierDealPoker poker = BarrierDataMgr.Instance.dealPoker(type);
                GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_NEWPOKER, poker);
            }
        }
        
        BarrierDataMgr.Instance.setState(BarrierState.dragPoker);
        GamePropertyMgr.Instance.save();
    }

    public void requestFillPoker()
    {
        List<BarrierDealType> typeList = new List<BarrierDealType> { BarrierDealType.npc, BarrierDealType.player };
        for(int i=0;i< typeList.Count; i++)
        {
            BarrierDealType type = typeList[i];
            BarrierDealPoker poker = BarrierDataMgr.Instance.dealPoker(type);
            GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_NEWPOKER, poker);
        }
        BarrierDataMgr.Instance.setState(BarrierState.dragPoker);
        GamePropertyMgr.Instance.save();
    }

    public void requestDealPoker() {
        BarrierDealPoker poker = BarrierDataMgr.Instance.dealPoker(BarrierDealType.other);
        GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_NEWPOKER,poker);

        BarrierState state = BarrierDataMgr.Instance.getMatchPoint() >= 21 ? BarrierState.stopPoker : BarrierState.dealPoker;
        BarrierDataMgr.Instance.setState(state);
        GamePropertyMgr.Instance.save();
    }

    public void requestRefreshPoker(BarrierDealType type) {
        int count = 0;
        if (type == BarrierDealType.npc){
            count = BarrierDataMgr.Instance.setRefreshNpcPokerNum();
        }else{
            count = BarrierDataMgr.Instance.setRefreshPlayerPokerNum();
        }
        for (int i = 0; i < count; i++)
        {
            BarrierDealPoker poker = BarrierDataMgr.Instance.dealPoker(type);
            GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_NEWPOKER, poker);
        }
        GamePropertyMgr.Instance.save();
    }

    public void requestStopPoker() {
        BarrierDataMgr.Instance.setState(BarrierState.stopPoker);
        GamePropertyMgr.Instance.save();
    }

    public void requestSurePoker() {
        string viewName = "";
        PageIndex pageIndex = 0;
        List<IPoker> npcList = BarrierDataMgr.Instance.getPokers(BarrierDealType.npc);
        int index = npcList.FindIndex(p => p.getValue() == BarrierDataMgr.Instance.getMatchPointA());
        
        if (index == 0)
        {
            viewName = "GameView";
            pageIndex = PageIndex.GameView;
        }
        else if (index == 1)
        {
            viewName = "RelaxView";
            pageIndex = PageIndex.RelaxView;
        }
        else if (index == 2)
        {

        }
          
        if (pageIndex == 0) return;

        BarrierDataMgr.Instance.clearMatch();
        BarrierDataMgr.Instance.addBarrierId();
        BarrierDataMgr.Instance.setState(BarrierState.fillPoker);
        GameDataMgr.Instance.setPageIndex(pageIndex);
        GamePropertyMgr.Instance.save();
        GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_SUREPOKER, viewName);
    }

    public void requestMatchPoker(int matchPointA, int matchPointB, int pokerPosX, int pokerPosY)
    {
        BarrierDataMgr.Instance.setState(BarrierState.matchPoker);
        BarrierDataMgr.Instance.setMatchPoker(matchPointA, matchPointB, pokerPosX, pokerPosY);
        GamePropertyMgr.Instance.save();
    }

    public void requestUnMatchPoker() {
        BarrierDataMgr.Instance.setState(BarrierState.dragPoker);
        BarrierDataMgr.Instance.setMatchPoker(0, 0, 0, 0);
        GamePropertyMgr.Instance.save();
    }
}
