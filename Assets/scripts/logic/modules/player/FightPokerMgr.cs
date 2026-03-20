//牌堆
using Pb;
using System.Collections.Generic;
using UnityEngine;


public class FightPokerMgr : Singleton<FightPokerMgr>
{
    private List<IUser> _players = null;
    private IGameFlow _gameFlow = new CardFlow();

    public void init() {
        this._gameFlow = new CardFlow();
        this._players = new List<IUser>();
        this.newUser(FightDealType.npc);
        this.newUser(FightDealType.player);
    }

    private void newUser(FightDealType type) {
        AssetInfo info = FightDataMgr.Instance.getAssetInfo(type);
        IUser user = new UserObject(type == FightDealType.npc);
        user.setAttack(info.Attack);
        user.setDefense(info.Defense);
        user.setMagic(info.Magic);
        user.setMaxMagic(info.MaxMagic);
        user.setBlood(info.Hp);
        user.setMaxBlood(info.MaxHP);
        user.setState((UserState)info.State);
        this._players.Add(user);
    }

    public List<IUser> getPlayers()
    {
        return this._players;
    }

    private IUser getUserPlaying()
    {
        return this._players.Find(user => user.getState() == UserState.play);
    }

    private void userDealPoker(IUser user,int suit = -1) {
        int number = FightDataMgr.Instance.getRemainCards();
        if (number == 0) {
            FightDataMgr.Instance.shuffle();
        }

        if (number <= 1)
        {
            GameMessage.Instance.addMsg(GameConst.SHUFFLEPOKER, number);
        }

        if (suit > 0) {
            suit = RandomMgr.Instance.getRangeInt(0, 2) == 0 ? -1 : suit;
        }

        IPoker poker = FightDataMgr.Instance.dealPoker(this.getDealType(user), suit);
        GameMessage.Instance.addMsg(GameConst.DEALPOKER, new DealPokerPara(user, poker));
    }

    private void setUserState(IUser user,UserState state){
        user.setState(state);
        FightDataMgr.Instance.setUserState(this.getDealType(user),state);
    }

    public void setFlowState(FightFlowState state) {
        FightDataMgr.Instance.setState((int)state);
    }
   
    public void runFlow() {
        FightFlowState state = (FightFlowState)FightDataMgr.Instance.getState();
        switch (state) {
            case FightFlowState.dealCard:
                {
                    bool isImmediately = true;
                    for (int i = 0; i < this._players.Count; i++)
                    {
                        if (this.getUserRound() % 2 == 1) 
                            continue;

                        IUser user = this._players[i];
                        List<ICard> cards = CardMgr.Instance.getRandomCard(user);

                        if (cards.Count == 0)
                            continue;

                        if (user.isNpc())
                        {
                            ICard card = this.addNpcCard(user, cards);
                            GameMessage.Instance.addMsg(GameConst.DEALCARD, new DealCardPara(user, card));
                        }
                        else
                        {
                            isImmediately = false;
                            GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, new CandidacyCardPara(user, cards));
                        }
                    }
                    this.addUserRound();
                    this._gameFlow.addCardAfter(new AddCardAfterPara(this._players));

                    if (isImmediately) {
                        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.twoHandPoker);
                    }
                }
                break;
            case FightFlowState.twoHandPoker:
                { 
                    for (int i = 0; i < 2; i++) {
                        foreach (var user in this._players) {
                            this.userDealPoker(user);
                        }
                    }
                    this._gameFlow.handPokerAfter(new HandPokerAfterPara(this._players));
                    GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.turnPlayer);
                }
                break;
            case FightFlowState.turnPlayer:
                {
                    IUser user = this._players.Find(user => user.getState() == UserState.none);
                    if (user != null)
                    {
                        this.setUserState(user, UserState.play);
                        GameMessage.Instance.addMsg(GameConst.TURNPLAYER, user);
                        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.waitOperator);

                        //空闲的切状态
                        IUser idleUser = this._players.Find(user => user.getState() == UserState.idle);
                        if (idleUser != null) {
                            this.setUserState(idleUser, UserState.none);
                        }
                    }
                    else
                    {
                        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.fightSettle);
                    }                    
                }
                break;
            case FightFlowState.waitOperator:
                {
                    IUser user = this.getUserPlaying();
                    GameMessage.Instance.addMsg(GameConst.WAITOPERATOR, user);
                }
                break;
            case FightFlowState.dealPoker:
                {
                    IUser user = this.getUserPlaying();
                    this.userDealPoker(user);
                    this._gameFlow.dealPokerAfter(new DealPokerAfterPara(this._players, user));
                    GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.totalPokerPoint);
                }
                break;
            case FightFlowState.stopDealPoker:
                {
                    IUser user = this.getUserPlaying();
                    this.setUserState(user, UserState.end);
                    GameMessage.Instance.addMsg(GameConst.STOPDEALPOKER, user);
                    GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.turnPlayer);
                }
                break;
            case FightFlowState.totalPokerPoint:
                {
                    IUser user = this.getUserPlaying();
                    int point = this.getUserHandPokerPoint(user, false);
                    this.setUserState(user, point <= 21 ? UserState.idle : UserState.death);
                    GameMessage.Instance.addMsg(GameConst.TOTALPOKERPOINT, new TotalHandPokerPointPara(user, point));
                    GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.turnPlayer);
                }
                break;
            case FightFlowState.fightSettle: 
                {
                    this.fightSettle();
                }
                break;
        }
    }

    private void fightSettle() {        
        int number0 = this.getUserHandPokerPoint(this._players[0], false);
        int number1 = this.getUserHandPokerPoint(this._players[1], false);
        bool isBackJack0 = this.isUserHandPokerBlackJack(this._players[0]);
        bool isBackJack1 = this.isUserHandPokerBlackJack(this._players[1]);

        int index = -1;
        bool isBack = false;
        if (number0 <= 21 && number1 > 21){
            index = 0;
        } else if (number0 > 21 && number1 <= 21) {
            index = 1;
        }else if(number0 <= 21 && number1 <= 21){
            if (number0 > number1){
                index = 0;
            }else if (number0 < number1){
                index = 1;
            }
        }

        if (isBackJack0 && !isBackJack1){
            index = 0;
            isBack = true;
        }else if (!isBackJack0 && isBackJack1){
            index = 1;
            isBack = true;
        }

        IUser user = null;
        for (int i = 0; i < this._players.Count; i++) {
            this._players[i].addPlay();
            if (index == i) {
                user = this._players[i];
                user.addWins();
            }
        }
        GameMessage.Instance.addMsg(GameConst.GAMESETTLE, user);

        bool isOver = this._gameFlow.gameSettle(new GameSettlePara(this._players, index, isBack));
        if (isOver){
            GameMessage.Instance.addMsg(GameConst.GAMEOVER);
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.fightOver);
        }else {
            GameMessage.Instance.addMsg(GameConst.GAMECLEAR);
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.dealCard);
        }
        FightDataMgr.Instance.setIsFilp(FightDealType.npc, 1);   
    }

    public void dealPoker(IUser user,int dealNumber,int suit = -1)
    {
        for (int i = 0; i < dealNumber; i++){
            this.userDealPoker(user,suit);
        }
    }

    private List<int> getUserPokers(IUser user, bool isFilter) {
        List<IPoker> pokers = FightDataMgr.Instance.getPokers(this.getDealType(user));
        List<int> values = new List<int>();
        foreach (var item in pokers){
            if (isFilter){
                if (!item.isBack()){
                    values.Add(item.getValue());
                }
            }else{
                values.Add(item.getValue());
            }
        }
        return values;
    }

    public int getUserHandPokerPoint(IUser user, bool isFilter) {
        return PokerPointMgr.Instance.getPokerPoint(this.getUserPokers(user,isFilter));
    }

    public bool isUserHandPokerBlackJack(IUser user)
    {
        return PokerPointMgr.Instance.isBlackJack(this.getUserPokers(user, true));
    }

    private FightDealType getDealType(IUser user) {
        return user.isNpc() ? FightDealType.npc : FightDealType.player;
    }

    public List<IPoker> getUsetHandPoker(IUser user) {
        return FightDataMgr.Instance.getPokers(this.getDealType(user));
    }

    public void clearUserHandPoker(IUser user) {
        this.getUsetHandPoker(user).Clear(); //清除手牌
        EventDispatcher.Instance.emit(GameConst.CLEARHANDPOKER, user);
    }

    public List<ICard> getUserCards(IUser user) {
        return FightDataMgr.Instance.getCards(this.getDealType(user));
    }

    public int getUserRound()
    {
        return FightDataMgr.Instance.getRound();
    }

    public int addUserRound()
    {
        return FightDataMgr.Instance.addRound();
    }

    public ICard addNpcCard(IUser user,List<ICard> cards) {
        //优选第2级
        ICard card = cards.Find(card => card.getLevel() == 2);

        //随机第1级
        if (card == null){
            card = cards[RandomMgr.Instance.getRangeInt(0, cards.Count)];
        }

        bool success = FightDataMgr.Instance.addCard(this.getDealType(user), card);
        if (success){
            return card;
        }else { 
            return null;
        }
    }

    public bool addUserCard(bool isOk, IUser user, ICard card)
    {
        bool success = isOk;
        if (success)
        {
            success = FightDataMgr.Instance.addCard(this.getDealType(user), card);
        }
        if (success)
        {
            this._gameFlow.addCardAfter(new AddCardAfterPara(this._players));
        }
        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.twoHandPoker);
        return success;
    }

    public void reDealHandPoker(IUser user,int suit = -1)
    {
        if (user.isNpc()) {
            int number = this.getUserHandPokerPoint(user, false);
            if (number >= 21)
            {
                return;
            }
            if (RandomMgr.Instance.getRangeInt(0, 100) > 30)
            {
                return;
            }
        }
        this.clearUserHandPoker(user);
        this.dealPoker(user,2, suit);
    }

    public void clear() {
        foreach (var user in _players){
            this.setUserState(user, UserState.none);
            this.getUsetHandPoker(user).Clear(); //清除手牌    
        }
        FightDataMgr.Instance.setIsFilp(FightDealType.npc, 0);
    }
}
