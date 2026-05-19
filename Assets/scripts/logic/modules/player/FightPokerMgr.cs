//
using Pb;
using System.Collections;
using System.Collections.Generic;

public class FightPokerMgr : Singleton<FightPokerMgr>
{
    private List<IUser> _players = null;
    private IGameFlow _gameFlow = new CardFlow();

    public void init() {
        this._players = new List<IUser>();
        this.newUser(FightDealType.npc);
        this.newUser(FightDealType.player);
        this._gameFlow = new CardFlow();
        this._gameFlow.gameBegin(new GameBeginPara(this._players));
    }

    private void newUser(FightDealType type) {
        AssetInfo info = FightDataMgr.Instance.getAssetInfo(type);
        IExtraInfo extra = new ExtraInfoObject();
        extra.setMultATK(info.Extra.MultATK);
        extra.setAddCrit(info.Extra.AddCrit);
        extra.setBonusArmor(info.Extra.BonusArmor);
        extra.setTemporaryArmor(info.Extra.TemporaryArmor);
        extra.setLifeSteal(info.Extra.LifeSteal);
        extra.setHealToMP(info.Extra.HealtoMP);
        extra.setSkillDamageUp(info.Extra.SkillDamageUp);
        extra.setMpMaxSub(info.Extra.MpMaxSub);
        extra.setAddBleeding(info.Extra.AddBleeding);
        extra.setIgnoreArmor(info.Extra.IgnoreArmor);
        extra.setExecute(info.Extra.Execute);
        extra.setReflectPercent(info.Extra.ReflectPercent);
        extra.setMagicImmunity(info.Extra.MagicImmunity);
        extra.setArmorATK(info.Extra.ArmorATK);
        extra.setImmunityDeBuff(info.Extra.ImmunityDeBuff);
        extra.setRtHurtValue(info.Extra.RtHurtValue);
        extra.setRtMagicValue(info.Extra.RtMagicValue);
        foreach (float item in info.Extra.MpRegen) {
            extra.setMpRegen(item);
        }
        foreach (float item in info.Extra.HealOverTime)
        {
            extra.setHealOverTime(item);
        }

        IUser user = new UserObject(type == FightDealType.npc);
        user.setAttack(info.Attack);
        user.setDefense(info.Defense);
        user.setMagic(info.Magic);
        user.setMaxMagic(info.MaxMagic);
        user.setBlood(info.Hp);
        user.setMaxBlood(info.MaxHP);
        user.setState((UserState)info.State);
        user.setExtraInfo(extra);
        this._players.Add(user);

        //添加注册
        extra.setBuffAction((BuffAction buffAction, BuffType buffType) => {
            this.buffHandle(user,buffAction,buffType);
        });
    }

    private void saveUser(IUser user) {
        AssetInfo info = this.getAssetInfo(user);
        info.Hp = user.getBlood();
        info.MaxHP = user.getMaxBlood();
        info.Magic = user.getMagic();
        info.MaxMagic = user.getMaxMagic();
        info.Attack = user.getAttack();
        info.Defense = user.getDefense();
        info.State = (int)user.getState();
        info.Extra.MultATK = user.getExtraInfo().getMultATK();
        info.Extra.AddCrit = user.getExtraInfo().getAddCrit();
        info.Extra.BonusArmor = user.getExtraInfo().getBonusArmor();
        info.Extra.TemporaryArmor = user.getExtraInfo().getTemporaryArmor();
        info.Extra.LifeSteal = user.getExtraInfo().getLifeSteal();
        info.Extra.HealtoMP = user.getExtraInfo().getHealToMP();
        info.Extra.SkillDamageUp = user.getExtraInfo().getSkillDamageUp();
        info.Extra.MpMaxSub = user.getExtraInfo().getMpMaxSub();
        info.Extra.AddBleeding = user.getExtraInfo().getAddBleeding();
        info.Extra.IgnoreArmor = user.getExtraInfo().getIgnoreArmor();
        info.Extra.Execute = user.getExtraInfo().getExecute();
        info.Extra.ReflectPercent = user.getExtraInfo().getReflectPercent();
        info.Extra.MagicImmunity = user.getExtraInfo().getMagicImmunity();
        info.Extra.ArmorATK = user.getExtraInfo().getArmorATK();
        info.Extra.ImmunityDeBuff = user.getExtraInfo().getImmunityDeBuff();
        info.Extra.RtHurtValue = user.getExtraInfo().getRtHurtVaule();
        info.Extra.RtMagicValue = user.getExtraInfo().getRtMagicValue();
        info.Extra.MpRegen.Clear();
        info.Extra.HealOverTime.Clear();
        foreach (float item in user.getExtraInfo().getMpRegens())
        {
            info.Extra.MpRegen.Add(item);
        }
        foreach (float item in user.getExtraInfo().getHealOverTimes())
        {
            info.Extra.HealOverTime.Add(item);
        }
    }

    public AssetInfo getAssetInfo(IUser user) {
        return FightDataMgr.Instance.getAssetInfo(this.getDealType(user));
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
        int point = this.getUserHandPokerPoint(user, true);
        GameMessage.Instance.addMsg(GameConst.DEALPOKER, new DealPokerPara(user, poker, point));
    }

    private void setUserState(IUser user,UserState state){
        user.setState(state);
        FightDataMgr.Instance.setUserState(this.getDealType(user),state);
    }

    public void setFlowState(FightFlowState state) {
        FightDataMgr.Instance.setState((int)state);
    }

    public void setUserInfo() {
        for (int i = 0; i < this._players.Count; i++) {
            this.saveUser(this._players[i]);
        }
    }
   
    public void runFlow() {
        FightFlowState state = (FightFlowState)FightDataMgr.Instance.getState();
        switch (state) {
            case FightFlowState.handPokerBefore:
                {
                    this.addUserRound();
                    this._gameFlow.handPokerBefore(new HandPokerBeforePara(this._players));
                    GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.twoHandPoker);
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
                    if (user == null) {
                        user = this._players.Find(user => user.getState() == UserState.idle);
                    }
                    if (user != null)
                    {
                        this.setUserState(user, UserState.play);
                        GameMessage.Instance.addMsg(GameConst.TURNPLAYER, user);
                        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.waitOperator);

                        //
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

                    this._gameFlow.stopPokerAfter(new StopPokerAfterPara(this._players, user));
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
        int point0 = this.getUserHandPokerPoint(this._players[0], false);
        int point1 = this.getUserHandPokerPoint(this._players[1], false);
        bool isBlackJack0 = this.isUserHandPokerBlackJack(this._players[0]);
        bool isBlackJack1 = this.isUserHandPokerBlackJack(this._players[1]);

        int index = -1;
        bool isBlackJack = false;
        if (point0 <= 21 && point1 > 21){
            index = 0;
        } else if (point0 > 21 && point1 <= 21) {
            index = 1;
        }else if(point0 <= 21 && point1 <= 21){
            if (point0 > point1)
            {
                index = 0;
            }else if (point0 < point1)
            {
                index = 1;
            }
        }

        if (isBlackJack0 && !isBlackJack1){
            index = 0;
            isBlackJack = true;
        }else if (!isBlackJack0 && isBlackJack1){
            index = 1;
            isBlackJack = true;
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

        bool isOver = this._gameFlow.gameSettle(new GameSettlePara(this._players, index, isBlackJack));
        if (isOver){
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.fightOver);
            GameMessage.Instance.addMsg(GameConst.GAMEOVER);
        }
        else {
            GameMessage.Instance.addMsg(GameConst.GAMECLEAR);
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.handPokerBefore);
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
        this.getUsetHandPoker(user).Clear();
        EventDispatcher.Instance.emit(GameConst.CLEARHANDPOKER, user);
    }

    public List<ICard> getUserCards(IUser user) {
        return FightDataMgr.Instance.getCards(this.getDealType(user));
    }

    public List<IAssembleCard> getUserAssembleCards(IUser user) {
        return ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
    }

    public int getUserRound()
    {
        return FightDataMgr.Instance.getRound();
    }

    public int addUserRound()
    {
        return FightDataMgr.Instance.addRound();
    }

    public void addNpcCard(IUser user,List<ICard> cards) {
        ICard card = cards.Find(card => card.getLevel() == 2);
        
        if (card == null){
            card = cards[RandomMgr.Instance.getRangeInt(0, cards.Count)];
        }

        bool success = FightDataMgr.Instance.addCard(this.getDealType(user), card);
        if (success)
        {
            GameMessage.Instance.addMsg(GameConst.DEALCARD, new DealCardPara(user, card));
            //this._gameFlow.addCardAfter(new AddCardAfterPara(this._players));
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
            //this._gameFlow.addCardAfter(new AddCardAfterPara(this._players));
        }
        GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.twoHandPoker);
        return success;
    }

    public void setAdvancedEffectId(int triggerId, IUser user, IPart part)
    {
        ImprintDataMgr.Instance.setAdvancedEffectId(user.isNpc(), triggerId, part.getId());        
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
            this.getUsetHandPoker(user).Clear();
        }
        FightDataMgr.Instance.setIsFilp(FightDealType.npc, 0);
    }

    public void buffHandle(IUser user,BuffAction buffAction,BuffType buffType) {
        string key = buffAction == BuffAction.add ? GameConst.ADDBUFFTYPE : GameConst.REMOVEBUFFTYPE;
        IUIBuffPara buffPara = new UIBuffParaObject(user, buffType);
        GameMessage.Instance.addMsg(key, buffPara);
    }
}
