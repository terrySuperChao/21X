//
using System.Collections.Generic;

public class CardFlow : GameFlowObject
{
    private IAttackSettle _pokerSettle = null;
    private IAttackSettle _attackSettle = null;
    private IAttackSettle _specialSettle = null;

    public  CardFlow()
    {
        this._pokerSettle = new PokerSettle();
        this._attackSettle = new AttackSettle();
        this._specialSettle = new SpecialSettle();
    }

    override
    protected void _gameBegin(IGameBeginPara para)
    {

    }

    override
    protected void _addCardAfter(IAddCardAfterPara para)
    {
        this.execCardHandle(para.getUsers(), false, CardHandleType.addNewCardAfter);
        this.execCardHandle(para.getUsers(), true, CardHandleType.addNewCardAfter);
    }

    override
    protected void _handPokerAfter(IHandPokerAfterPara para)
    {
        this.execCardHandle(para.getUsers(), false, CardHandleType.handPokerAfter);
        this.execCardHandle(para.getUsers(), true, CardHandleType.handPokerAfter);
    }


    override
    protected void _dealPokerAfter(IDealPokerAfterPara para)
    {
        this.execCardHandle(para.getUsers(), !para.getUser().isNpc(), CardHandleType.dealPokerAfter);
    }

    override
    protected void _stopPokerAfter(IStopPokerAfterPara para)
    {
        this.execCardHandle(para.getUsers(), para.getUser().isNpc(), CardHandleType.stopPokerAfter);
    }

    override
    protected bool _gameSettle(IGameSettlePara para) {
        ITriggerHandlePara handlePara = new TriggerHandleParaObject();
        handlePara.setGameSettlePara(para);
        handlePara.setRoundResult(new RoundResultObject());

        //npc
        this.setTriggerHandleParaUser(handlePara, 0);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundBeginBefore);

        //player
        this.setTriggerHandleParaUser(handlePara, 1);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundBeginBefore);

        int winIndex = -1;//para.getWinIndex();
        if (winIndex == -1) //平局
        {
            this._specialSettle.settle(null);
            return false;
        }

        //wins
        this.setTriggerHandleParaUser(handlePara, winIndex);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundBegin);

        //牌结算
        this._pokerSettle.settle(handlePara);
        
        //特殊结算
        this._specialSettle.settle(handlePara);

        //卡牌结算
        CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackBefore);
        
        //攻击
        this._attackSettle.settle(handlePara);

        CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackAfter);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundEnd);

        return handlePara.getAttackUser().getBlood() <= 0 ||
               handlePara.getDefenseUser().getBlood() <= 0;
    }

    //winIndex:0 npc 1:player
    private void setTriggerHandleParaUser(ITriggerHandlePara handlePara,int winIndex) {
        List<IUser> users = handlePara.getGameSettlePara().getUsers();
        IUser npc = users.Find(user => user.isNpc() == true);
        IUser player = users.Find(user => user.isNpc() == false);

        IUser attackUser = winIndex == 0 ? npc : player;
        IUser defenseUser = winIndex == 0 ? player : npc;
        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
    }

    private void execCardHandle(List<IUser> users,bool isNpc, CardHandleType type) {
        ITriggerHandlePara handlePara = new TriggerHandleParaObject();
        handlePara.setGameSettlePara(new GameSettlePara(users, -1, false));
        handlePara.setRoundResult(new RoundResultObject());

        this.setTriggerHandleParaUser(handlePara, isNpc ? 0 : 1);
        CardMgr.Instance.handle(handlePara, type);
    }
}