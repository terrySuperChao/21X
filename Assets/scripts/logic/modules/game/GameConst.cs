//≈∆∂—
public class GameConst
{
    public const string RETURNTOLOBBY   = "returnToLobby";
    public const string STARTGAME       = "startGame";
    public const string SHOWTIPS        = "SHOWTIPS";
    public const string DEALPOKER       = "dealPoker";
    public const string STOPDEALPOKER   = "stopDealPoker";
    public const string PLAYERACTION    = "playerAction";
    public const string SHUFFLEPOKER    = "shufflePoker";
    public const string GAMESETTLE      = "gameSettle";
    public const string GAMENEXTROUND   = "gameNextRound";
    public const string GAMEOVER        = "gameOver";
    public const string FLIPPOKER       = "flipPoker";
    public const string DEALCARD        = "dealCard";
    public const string SELECTCARD      = "selectCard";
    public const string SELECTFINSIHCARD = "SELECTFINSIHCARD";
    public const string ADDPOKERVALUE   = "addPokerValue";
    public const string ADDCARDVALUE    = "addCardValue";
    public const string COMMONATTACK    = "commonAttack";
    public const string FLYFONT         = "flyFont";
    public const string REFACTORING     = "reFactoring";
    public const string REHANDPOKER     = "reHandPoker";
    public const string CLEARHEADPOKER  = "clearHandPoker";

    public static ValueType SuitTransformValueType(PokerSuit suit)
    {
        if (suit == PokerSuit.club)
        {
            return ValueType.magic;
        }
        else if (suit == PokerSuit.diamond)
        {
            return ValueType.defense;
        }
        else if (suit == PokerSuit.spade)
        {

            return ValueType.attack;
        }
        else if (suit == PokerSuit.heart)
        {
            return ValueType.blood;
        }
        else
        {
            return ValueType.nil;
        }
    }

    public readonly static int[] CARDS = {
    102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,    // ∑Ω 2 ~ A
    202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214,    // ∫Ï
    302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314,    // ∫⁄
    402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414     // √∑
};
}

public enum GameMode { 
    Common,
    Fight,
    Card
}

public enum CardType { 
    cardType1=1,//∫⁄Ã“¥Û ¶
    cardType2,//∫ÏÃ“ÃÏ π
    cardType3,//√∑ª®∑® ¶
    cardType4,//∑ΩøÈª§Œ¿
    cardType5,// ≈∆–Ú÷ÿππ
    cardType6,//øÒ’Ω ø÷Æ≈≠
    cardType7,//ƒßƒ‹Õ‚“Á
    cardType8,//±¨≈∆÷Æ∂‹
    cardType9,// »—™øÒª∂
    cardType10,//√¸‘À¿°‘˘
}

public enum PokerSuit {
    /**
    * ∑ΩøÈ≈∆
    */
    diamond = 1,
    /**
     * ∫ÏÃ“≈∆
     */
    heart = 2,
    /**
     * ∫⁄Ã“≈∆
     */
    spade = 3,
    /**
     * √∑ª®≈∆
     */
    club = 4,
}

public enum CardHandleType {
    addNewCardAfter,
    handPokerAfter,
    dealPokerAfter,
    roundBegin,
    roundAddValueBefore,
    roundAddValue,
    roundAddMagic,
    roundSpecialAttr,
    roundAttackBegin,
    roundAttack,
    roundMagicAttack,
    roundSubDefense,
    roundSubBlood,
    roundAttackAfter,
    roundEnd,
}

public enum ValueType { 
    nil,
    blood,
    attack,
    defense,
    magic
}

public enum PageIndex {
    EntryView,
    LobbyView,
    BarrierView,
    GameView,
    RelaxView,
    ShopView,
}

public enum GameState { 
    idle,
    playing,
}

public enum BarrierState { 
    startPoker,  //ø™ º≈∆
    matchPoker, //∆•≈‰≈∆
    dealPoker, //∑¢≈∆
    stopPoker, //Õ£≈∆
    fillPoker, //≤π≈∆
}

public enum BarrierDealType
{
    npc,
    player,
    other,
}




