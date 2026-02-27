using System.Collections.Generic;
using UnityEngine;

public class GameReqMgr : Singleton<GameReqMgr>
{
    public void requestNewGame(int roleId)
    {
        GameDataMgr.Instance.newGame();
        GameDataMgr.Instance.setGameState(GameState.playing);
        GameDataMgr.Instance.setPageIndex(PageIndex.BarrierView);
        PlayerDataMgr.Instance.setRoleId(roleId);
        PlayerDataMgr.Instance.setMoney(ConfigMgr.INIT_MONEY_VALUE);
        PlayerDataMgr.Instance.setHP(ConfigMgr.INIT_BLOOD_VALUE);
        PlayerDataMgr.Instance.setMaxHP(ConfigMgr.INIT_BLOOD_VALUE);
        PlayerDataMgr.Instance.setMaxMagic(ConfigMgr.INIT_MAGIC_VALUE);
        GamePropertyMgr.Instance.save();
    }

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

    public void requestRelax() {
        int point = BarrierDataMgr.Instance.getFinalPoint();
        int value = 0;
        if (point < 15)
        {
            value = 10;
        }
        else if (point >= 15 && point <= 19)
        {
            value = 20;
        }
        else if (point >= 20 && point <= 21)
        {
            value = 30;
        }
        else {
            value = RandomMgr.Instance.getRangeInt(1, 16);
        }
        if (BarrierDataMgr.Instance.getBlackjack() == 1)
        {
            PlayerDataMgr.Instance.addMaxHP(10);
        }
        PlayerDataMgr.Instance.addHP(value);
        GameDataMgr.Instance.setPageIndex(PageIndex.BarrierView);
        GamePropertyMgr.Instance.save();

        bool isFinish = PlayerDataMgr.Instance.getHP() == PlayerDataMgr.Instance.getMaxHP();
        GameMessage.Instance.addMsg(GameConst.RELAXVIEW_RELAX,isFinish);
    }
}
