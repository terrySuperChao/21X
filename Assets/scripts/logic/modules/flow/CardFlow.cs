//
using System.Collections.Generic;

public class CardFlow : GameFlowObject
{
    private IAttackSettle _pokerSettle = null;
    private IAttackSettle _attackSettle = null;
    private IAttackSettle _specialSettle = null;
    private ITriggerHandlePara _handlePara = null;

    public  CardFlow()
    {
        this._pokerSettle = new PokerSettle();
        this._attackSettle = new AttackSettle();
        this._specialSettle = new SpecialSettle();
        this._handlePara = new TriggerHandleParaObject();
    }

    override
    protected void _gameBegin(IGameBeginPara para)
    {
        ITriggerHandlePara handlePara = this._handlePara;
        foreach (IUser user in para.getUsers()) {
            handlePara.addRoundResult(new RoundResultObject(user));
        }
        handlePara.setGameSettlePara(new GameSettlePara(para.getUsers(), -1, false));
    }

    override
    protected void _handPokerBefore(IHandPokerBeforePara para)
    {
        GameBloodMgr.Instance.addBloodHandle(para.getUsers());
        ITriggerHandlePara handlePara = this._handlePara;
        this.setTriggerHandleParaUser(handlePara,0);
        SwitchParaMgr.Instance.handle(handlePara, () =>{
            CardMgr.Instance.handle(handlePara, TriggerEvent.initPokerBefore);
        });
    }

    override
    protected void _handPokerAfter(IHandPokerAfterPara para)
    {
    }


    override
    protected void _dealPokerAfter(IDealPokerAfterPara para)
    {
        ITriggerHandlePara handlePara = this._handlePara;
        this.setTriggerHandleParaUser(handlePara, para.getUser().isNpc() ? 0 : 1);
        CardMgr.Instance.handle(handlePara, TriggerEvent.dealPokerAfter);
    }

    override
    protected void _stopPokerAfter(IStopPokerAfterPara para)
    {
        ITriggerHandlePara handlePara = this._handlePara;
        this.setTriggerHandleParaUser(handlePara, para.getUser().isNpc() ? 0 :1);
        CardMgr.Instance.handle(handlePara, TriggerEvent.stopPokerAfter);
    }

    override
    protected bool _gameSettle(IGameSettlePara para) {
        ITriggerHandlePara handlePara = this._handlePara;

        //关键数据
        handlePara.getGameSettlePara().setWinIndex(para.getWinIndex());
        handlePara.getGameSettlePara().setBlackJack(para.isBlackJack());

        //行动前
        this.setTriggerHandleParaUser(handlePara, 0);
        SwitchParaMgr.Instance.handle(handlePara, () =>{
            CardMgr.Instance.handle(handlePara, TriggerEvent.settlementBefore);
        });
        
        int winIndex = para.getWinIndex();
        if (winIndex != -1) //非平局
        {
            //wins
            this.setTriggerHandleParaUser(handlePara, winIndex);
            CardMgr.Instance.handle(handlePara, TriggerEvent.roundAttackBefore);

            //牌结算
            this._pokerSettle.settle(handlePara);

            //攻击
            this._attackSettle.settle(handlePara);
        }

        //行动后
        SwitchParaMgr.Instance.handle(handlePara, () => {
            CardMgr.Instance.handle(handlePara, TriggerEvent.roundAttackAfter);
        });

        this._specialSettle.settle(handlePara);
     
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
}