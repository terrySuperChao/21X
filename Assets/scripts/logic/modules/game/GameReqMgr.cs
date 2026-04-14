using Pb;
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
        for (int i = 0; i < typeList.Count; i++)
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
        GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_NEWPOKER, poker);

        BarrierState state = BarrierDataMgr.Instance.getMatchPoint() >= 21 ? BarrierState.stopPoker : BarrierState.dealPoker;
        BarrierDataMgr.Instance.setState(state);
        GamePropertyMgr.Instance.save();
    }

    public void requestRefreshPoker(BarrierDealType type) {
        int count = 0;
        if (type == BarrierDealType.npc) {
            count = BarrierDataMgr.Instance.setRefreshNpcPokerNum();
        } else {
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
        int point = BarrierDataMgr.Instance.getMatchPointA();
        if (point <= 0) return;

        PokerSuit suit = (PokerSuit)((point - point % 100) / 100);
        if (suit == PokerSuit.club) {
            ShopDataMgr.Instance.initEntry();
        }
        else if (suit == PokerSuit.spade) {
            FightDataMgr.Instance.initEntry();
            //初始化
            foreach (var item in ImprintDataMgr.Instance.getNpcAssembleCard())
            {
                FightDataMgr.Instance.addCardId(FightDealType.npc, item.getTriggerId());
            }

            foreach (var item in ImprintDataMgr.Instance.getPlayerAssembleCard())
            {
                FightDataMgr.Instance.addCardId(FightDealType.player, item.getTriggerId());
            }
            //初始化资源
            this.requestInitPlayerInfo();
        }

        PageIndex pageIndex = GameConst.PAGEINDEX_SUIT[(int)suit];
        BarrierDataMgr.Instance.clearMatch();
        BarrierDataMgr.Instance.addBarrierId();
        BarrierDataMgr.Instance.setState(BarrierState.fillPoker);
        GameDataMgr.Instance.setPageIndex(pageIndex);
        GamePropertyMgr.Instance.save();
        GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_EXIT);
    }

    public void requestImprint(int obj) {
        ImprintDataMgr.Instance.setAssembleObject(obj);
        GameDataMgr.Instance.setPageIndex(PageIndex.ImprintView);
        GamePropertyMgr.Instance.save();
        GameMessage.Instance.addMsg(GameConst.BARRIERVIEW_EXIT);
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
        if (PlayerDataMgr.Instance.getHP() == PlayerDataMgr.Instance.getMaxHP()) {
            GameDataMgr.Instance.setPageIndex(PageIndex.BarrierView);
        }
        PlayerDataMgr.Instance.addHP(value);
        GamePropertyMgr.Instance.save();
        GameMessage.Instance.addMsg(GameConst.UPDATE_PLAYER_INFO);
        GameMessage.Instance.addMsg(GameConst.RELAXVIEW_RELAX);
    }

    public void requestExitPage()
    {
        GameDataMgr.Instance.setPageIndex(PageIndex.BarrierView);
        GamePropertyMgr.Instance.save();
        GameMessage.Instance.addMsg(GameConst.EXIT_PAGE);
    }

    public void requestReturnHome() {
        GameDataMgr.Instance.setGameState(GameState.idle);
        GamePropertyMgr.Instance.save();
    }

    public void requestSaveSetting(string language) {
        SettingDataMgr.Instance.setLanguage(language);
        SettingDataMgr.Instance.saveSetting();
        LangMgr.Instance.setCurLanguage(SettingDataMgr.Instance.getLanguage());
        GamePropertyMgr.Instance.save();
    }

    public void requestResetSetting()
    {
        SettingDataMgr.Instance.resetSetting();
        LangMgr.Instance.setCurLanguage(SettingDataMgr.Instance.getLanguage());
        GamePropertyMgr.Instance.save();
    }

    public void requestSaveFile() {
        GamePropertyMgr.Instance.save();
    }

    public void requestPurchaseGoods(int id, int price) {
        if (PlayerDataMgr.Instance.getMoney() >= price) {
            bool success = ShopDataMgr.Instance.purchaseGoods(id);
            if (success)
            {
                PlayerDataMgr.Instance.addMoney(-price);
                GamePropertyMgr.Instance.save();
                GameMessage.Instance.addMsg(GameConst.UPDATE_PLAYER_INFO);
                GameMessage.Instance.addMsg(GameConst.SHOPVIEW_PURCHASE);
            }
        }
    }

    public void requestSellHP() {
        if (PlayerDataMgr.Instance.getHP() > 10) {
            PlayerDataMgr.Instance.addHP(-10);
            PlayerDataMgr.Instance.addMoney(50);
            GamePropertyMgr.Instance.save();
            GameMessage.Instance.addMsg(GameConst.UPDATE_PLAYER_INFO);
        }
    }

    public void requestRefreshShop() {
        ShopDataMgr.Instance.refreshShop();
        GameMessage.Instance.addMsg(GameConst.SHOPVIEW_REFRESH);
    }
    public void requestAddCard(bool isOk, IUser user, ICard card, Vector3 position) {
        bool success = FightPokerMgr.Instance.addUserCard(isOk, user, card);
        if (success)
        {
            EventDispatcher.Instance.emit(GameConst.OKSELECTCARD, new SelectCardPara(user, card, position));
        }
        else
        {
            EventDispatcher.Instance.emit(GameConst.CANCELSELECTCARD);
        }
    }
    public void requestNpcOperator(IUser user)
    {
        FightFlowState state;
        if (FightPokerMgr.Instance.getUserHandPokerPoint(user, false) >= 17)
        {
            state = FightFlowState.stopDealPoker;
        }
        else
        {
            state = FightFlowState.dealPoker;
        }
        this.requestSaveFightFlowState(state);
    }

    public void requestSaveFightFlowState(FightFlowState state) {
        //保存下一个状态
        FightPokerMgr.Instance.setFlowState(state);
        GamePropertyMgr.Instance.save();

        //下一个流程
        FightPokerMgr.Instance.runFlow();
    }

    

    //保存状态
    public void requestSavePlayerInfo() {
        AssetInfo playerInfo = FightDataMgr.Instance.getAssetInfo(FightDealType.player);
        PlayerDataMgr.Instance.setHP(playerInfo.Hp);
        PlayerDataMgr.Instance.setMaxHP(playerInfo.MaxHP);
        PlayerDataMgr.Instance.setMagic(playerInfo.Magic);
        PlayerDataMgr.Instance.setMaxMagic(playerInfo.MaxMagic);
    }
    private void requestInitPlayerInfo() {
        AssetInfo playerInfo = FightDataMgr.Instance.getAssetInfo(FightDealType.player);
        playerInfo.Hp = PlayerDataMgr.Instance.getHP();
        playerInfo.MaxHP = PlayerDataMgr.Instance.getMaxHP();
        playerInfo.Magic = PlayerDataMgr.Instance.getMagic();
        playerInfo.MaxMagic = PlayerDataMgr.Instance.getMaxMagic();
    }
}
