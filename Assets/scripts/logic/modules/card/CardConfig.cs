//牌堆
using System;
using System.Collections.Generic;

public class CardConfig
{
    private static List<ICard> _config = new List<ICard> { 
        new CardObject(1,(int)CardType.cardType1,1,"黑桃大师",""),
		new CardObject(2,(int)CardType.cardType1,2,"黑桃大师+",""),
		new CardObject(3,(int)CardType.cardType2,1,"红桃天使",""),
		new CardObject(4,(int)CardType.cardType2,2,"红桃天使+",""),
		new CardObject(5,(int)CardType.cardType3,1,"梅花法师",""),
		new CardObject(6,(int)CardType.cardType3,2,"梅花法师+",""),
		new CardObject(7,(int)CardType.cardType4,1,"方块护卫",""),
		new CardObject(8,(int)CardType.cardType4,2,"方块护卫+",""),
		new CardObject(9,(int)CardType.cardType5,1,"牌序重构",""),
		new CardObject(10,(int)CardType.cardType5,2,"牌序重构+",""),
		new CardObject(11,(int)CardType.cardType6,1,"狂战士之怒",""),
		new CardObject(12,(int)CardType.cardType6,2,"狂战士之怒+",""),
		new CardObject(13,(int)CardType.cardType7,1,"魔能外溢",""),
		new CardObject(14,(int)CardType.cardType7,2,"魔能外溢+",""),
		new CardObject(15,(int)CardType.cardType8,1,"爆牌之盾",""),
		new CardObject(16,(int)CardType.cardType8,2,"爆牌之盾+",""),
		new CardObject(17,(int)CardType.cardType9,1,"嗜血狂欢",""),
		new CardObject(18,(int)CardType.cardType9,2,"嗜血狂欢+",""),
		new CardObject(19,(int)CardType.cardType10,1,"命运馈赠",""),
		new CardObject(20,(int)CardType.cardType10,2,"命运馈赠+",""),
	};
    public static List<ICard> getConfig() {
        return _config;
    }
}
