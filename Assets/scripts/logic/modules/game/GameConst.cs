using System;
//
public class GameConst
{
    public const string RETURNTOLOBBY   = "returnToLobby";
    public const string STARTGAME       = "startGame";
    public const string SHOWTIPS        = "SHOWTIPS";
    public const string DEALPOKER       = "dealPoker";
    public const string STOPDEALPOKER   = "stopDealPoker";
    public const string TURNPLAYER      = "turnPlayer";
    public const string WAITOPERATOR    = "waitOperator";
    public const string SHUFFLEPOKER    = "shufflePoker";
    public const string GAMESETTLE      = "gameSettle";
    public const string GAMENEXTROUND   = "gameNextRound";
    public const string GAMECLEAR       = "gameClear";
    public const string GAMEOVER        = "gameOver";
    public const string FLIPPOKER       = "flipPoker";
    public const string DEALCARD        = "dealCard";
    public const string CANDIDACYCARD   = "candidacyCard";
    public const string SELECTCARD      = "selectCard";
    public const string CANCELSELECTCARD = "cancelSelectCard";
    public const string OKSELECTCARD     = "onSelectCard";
    public const string ADDPOKERVALUE   = "addPokerValue";
    public const string ADDCARDVALUE    = "addCardValue";
    public const string COMMONATTACK    = "commonAttack";
    public const string FLYFONT         = "flyFont";
    public const string REFACTORING     = "reFactoring";
    public const string REHANDPOKER     = "reHandPoker";
    public const string FIGHTFLOWSTATE  = "fightFlowState";
    public const string TOTALPOKERPOINT = "totalPokerPoint";
    public const string CLEARHANDPOKER  = "clearHandPoker";
    public const string PLAYERACTION = "playerAction";
    public const string RUN_REFACTORING = "RUN_REFACTORING";
    public const string HIDE_REFACTORING = "HIDE_REFACTORING";
    public const string ADDBUFFTYPE     = "addBuffType";
    public const string REMOVEBUFFTYPE  = "removeBuffType";

    public const string CANDIDACYPART = "candidacyPart";
    public const string SELECTPART = "selectPart";

    //�ؿ�
    public const string BARRIERVIEW_NEWPOKER = "BARRIERVIEW_NEWPOKER";
    public const string BARRIERVIEW_EXIT = "BARRIERVIEW_EXIT";

    //��Ϣ
    public const string RELAXVIEW_RELAX = "RELAXVIEW_RELAX";
    //����
    public const string ADVENTURE_FOOD = "ADVENTURE_FOOD";
    //�̵�
    public const string SHOPVIEW_PURCHASE = "SHOPVIEW_PURCHASE";
    public const string SHOPVIEW_REFRESH = "SHOPVIEW_REFRESH";

    public const string EXIT_PAGE = "EXIT_PAGE";
    public const string UPDATE_PLAYER_INFO = "UPDATE_PLAYER_INFO";

    public const string IMPRINT_SELECT_PART = "Imprint_select_part";

    public readonly static int[] CARDS = {
    102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,    //  2 ~ A
    202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214,    // 
    302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314,    // 
    402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414     // 
    };

    public readonly static PageIndex[] PAGEINDEX_SUIT = { PageIndex.BarrierView, PageIndex.AdventureView, PageIndex.RelaxView, PageIndex.FightCardView, PageIndex.ShopView, PageIndex.ImprintView };
}

public enum GameMode { 
    Common,
    Fight,
    Card
}

public enum CardType { 
    cardType1=1,//���Ҵ�ʦ
    cardType2,//������ʹ
    cardType3,//÷����ʦ
    cardType4,//���黤��
    cardType5,// �����ع�
    cardType6,//��սʿ֮ŭ
    cardType7,//ħ������
    cardType8,//����֮��
    cardType9,//��Ѫ��
    cardType10,//��������
}

public enum PokerSuit {
    /**
    * 方块
    */
    diamond = 1,
    /**
     * 红桃
     */
    heart = 2,
    /**
     * 黑桃
     */
    spade = 3,
    /**
     * 梅花
     */
    club = 4,
}

public enum CardHandleType {
    addNewCardAfter,
    handPokerAfter,
    dealPokerAfter,
    stopPokerAfter,
    settlementBefore,
    roundBegin,
    roundAddValueBefore,
    roundAddValue,
    roundAddMagic,
    roundSpecialAttr,
    roundAttackBefore,
    roundAttack,
    roundMagicAttack,
    roundSubDefense,
    roundSubBlood,
    roundAttackAfter,
    roundEnd,
    settlementAfter,
}

public enum ValueType { 
    nil,
    blood,
    attack,
    defense,
    magic,
    point,
    winRate,
}

public enum PageIndex {
    EntryView,
    LobbyView,
    BarrierView,
    GameView,
    FightCardView,
    RelaxView,
    AdventureView,
    ShopView,
    ImprintView
}

public enum GameState { 
    idle,
    playing,
}

public enum BarrierState { 
    startPoker,  //
    dragPoker,   //
    matchPoker,  //
    dealPoker,   //
    stopPoker,   //
    fillPoker,   //
}

public enum BarrierDealType
{
    npc,
    player,
    other,
}

public enum FightDealType
{
    npc,
    player,
}

public enum UserState
{
    none,
    idle,
    play,
    end,
    death
}


public enum FightFlowState {
    handPokerBefore,
    twoHandPoker,
    turnPlayer,
    waitOperator,
    dealPoker,
    stopDealPoker,
    totalPokerPoint,
    fightSettle,
    fightOver,
}

public enum TargetPart
{
    baseEffect,
    advancedEffect,
    trigger,
}

public enum TriggerEvent {
    initPokerBefore,        //发牌前
    dealPokerBefore,        //要牌前
    dealPokerAfter,         //要牌后
    stopPokerAfter,         //停牌后
    settlementBefore,       //牌局结算前
    transformAttribute,     //属性转化
    roundAttackBefore,      //开始行动前
    normalAttackAfter,      //普通攻击后
    magicAttackAfter,       //魔法攻击后
    roundAttackAfter,       //结束行动后
    roundOther = 100,       //其他
}

public enum BuffAction
{
    add,
    remove,
}

public enum BuffType {
    multATK,
    bonusArmor,
    lifeSteal,
    healToMP,
    reflectDMG,
    skillDamageUp,
    mpRegen,
    addCrit,
    temporaryArmor,
}