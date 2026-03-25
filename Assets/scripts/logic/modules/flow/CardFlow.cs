//牌堆
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
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.addNewCardAfter);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.addNewCardAfter);
    }

    override
    protected void _handPokerAfter(IHandPokerAfterPara para)
    {
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.handPokerAfter);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.handPokerAfter);
    }


    override
    protected void _dealPokerAfter(IDealPokerAfterPara para)
    {
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), !para.getUser().isNpc(), CardHandleType.dealPokerAfter);
    }

    override
    protected bool _gameSettle(IGameSettlePara para) {
        int winIndex = para.getWinIndex();
        int lossIndex = winIndex == 0 ? 1 : 0;
        if (winIndex == -1) //平局
        {
            this._specialSettle.settle(null);
            return false;
        }
        
        List<IUser> users = para.getUsers();

        ICardHandlePara handlePara = new CardHandleParaObject();
        handlePara.setUser(users[winIndex]);
        handlePara.setAttackUser(users[winIndex]);
        handlePara.setDefenseUser(users[lossIndex]);
        handlePara.setRoundResult(new RoundResultObject());

        CardMgr.Instance.handle( handlePara, CardHandleType.roundBegin);

        //牌的结算
        this._pokerSettle.settle(handlePara);
        
        //特殊结算
        this._specialSettle.settle(handlePara);

        //攻击结算
        CardMgr.Instance.handle( handlePara, CardHandleType.roundAttackBegin);
        
        //结算
        this._attackSettle.settle(handlePara);

        CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackAfter);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundEnd);

        return handlePara.getAttackUser().getBlood() <= 0 ||
               handlePara.getDefenseUser().getBlood() <= 0;
    }
}