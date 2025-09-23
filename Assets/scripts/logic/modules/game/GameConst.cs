//牌堆
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameConst
{
    public const string DEALPOKER       = "dealPoker";
    public const string STOPDEALPOKER   = "stopDealPoker";
    public const string PLAYERACTION    = "playerAction";
    public const string SHUFFLEPOKER    = "shufflePoker";
    public const string GAMEOVER        = "gameOver";
    public const string FLIPPOKER       = "flipPoker";
    public const string DEALCARD        = "dealCard";
    public const string SELECTCARD      = "selectCard";
    public const string SELECTFINSIHCARD = "SELECTFINSIHCARD";
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
