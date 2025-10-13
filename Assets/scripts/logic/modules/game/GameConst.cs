//牌堆
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
}

public enum GameMode { 
    Common,
    Fight,
    Card
}

public enum CardType { 
    cardType1=1,//黑桃大师
    cardType2,//红桃天使
    cardType3,//梅花法师
    cardType4,//方块护卫
    cardType5,// 牌序重构
    cardType6,//狂战士之怒
    cardType7,//魔能外溢
    cardType8,//爆牌之盾
    cardType9,//嗜血狂欢
    cardType10,//命运馈赠
}

public enum PokerSuit {
    /**
    * 方块牌
    */
    diamond = 1,
    /**
     * 红桃牌
     */
    heart = 2,
    /**
     * 黑桃牌
     */
    spade = 3,
    /**
     * 梅花牌
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
    roundBust,
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

