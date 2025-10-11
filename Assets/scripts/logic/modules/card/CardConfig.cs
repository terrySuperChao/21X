//牌堆

using System.Collections.Generic;

public class CardConfig
{
    private static List<ICard> _config = new List<ICard> { 
        new CardObject(1,(int)CardType.cardType1,1,"黑桃大师","每次获得黑桃属性时（其实就是每张黑桃牌都需要单独判断），属性值额外+1"),
		new CardObject(2,(int)CardType.cardType1,2,"黑桃大师+","每次获得黑桃属性时，属性值额外+2，且有50%几率触发穿透攻击（忽略护甲，直接造成攻击）"),
		new CardObject(3,(int)CardType.cardType2,1,"红桃天使","每次获得红桃属性时，治疗效果提升20%"),
		new CardObject(4,(int)CardType.cardType2,2,"红桃天使+","每次获得红桃属性时，治疗效果提升50%，且溢出的治疗量将转化为等量护甲"),
		new CardObject(5,(int)CardType.cardType3,1,"梅花法师","每次获得梅花属性时，属性值额外+1"),
		new CardObject(6,(int)CardType.cardType3,2,"梅花法师+","每次获得梅花属性时，属性值额外+2，且释放技能后保留30%的魔法值"),
		new CardObject(7,(int)CardType.cardType4,1,"方块护卫","每次获得方块属性时，额外获得20%的护甲值"),
		new CardObject(8,(int)CardType.cardType4,2,"方块护卫+","每次获得方块属性时，额外获得50%的护甲值，且每次扣除护甲时，反弹50%的真实"),
		new CardObject(9,(int)CardType.cardType5,1,"牌序重构","每次发牌时有1次重新发牌的机会"),
		new CardObject(10,(int)CardType.cardType5,2,"牌序重构+","每次发牌时有2次重新发牌的机会，且可指定花色"),
		new CardObject(11,(int)CardType.cardType6,1,"狂战士之怒","每次攻击保留20%的攻击力"),
		new CardObject(12,(int)CardType.cardType6,2,"狂战士之怒+","每次攻击保留100%的攻击力，但会额外消耗10点血量"), 
        new CardObject(13,(int)CardType.cardType7,1,"魔能外溢","每次获得魔法值时，随机给对方造成1-2点随机魔法伤害"),
		new CardObject(14,(int)CardType.cardType7,2,"魔能外溢+","每次获得魔法值时，随机给对方造成2-4点随机魔法伤害，且不管21点是否获胜，每回合都会获得5点魔法值"),
		new CardObject(15,(int)CardType.cardType8,1,"爆牌之盾","每次爆牌，额外获得5点护甲"),
		new CardObject(16,(int)CardType.cardType8,2,"爆牌之盾+","每次爆牌，额外获得10点护甲，且有50%几率翻倍"),
		new CardObject(17,(int)CardType.cardType9,1,"嗜血狂欢","当你的血量低于25%时，每次获得的所有属性翻倍"),
		new CardObject(18,(int)CardType.cardType9,2,"嗜血狂欢+","当你的血量低于25%时，每次获得的所有属性翻倍，如果你的血量低于5%，每次获得的所有属性翻5倍"),
		new CardObject(19,(int)CardType.cardType10,1,"命运馈赠","对方每次要牌，有40%的几率额外获得1张手牌"),
		new CardObject(20,(int)CardType.cardType10,2,"命运馈赠+","对方每次要牌，有40%的几率额外获得2张手牌"),
	};
    public static List<ICard> getConfig() {
		List<ICard> list = new List<ICard>();
		for (int i = 0; i < _handle.Count; i++) {
			list.Add(_config[i]);
        }
        return list;
        //return _config;
    }

	private static List<ICardHandle> _handle = new List<ICardHandle> {
		new SpadeCardHandle(),
		/*
		new SpadeCardPlusHandle(),
		new HeartCardHandle(),
		new HeartCardPlusHandle(),
		new ClubCardHandle(),
		new ClubCardPlusHandle(),
		new DiamondCardHandle(),
		new DiamondCardPlusHandle(),
		new RefactoringHandle(),
		new RefactoringPlusHandle(),
        new BerserkerHandle(),
        new BerserkerPlusHandle(),
        new MagickaHandle(),
		new MagickaPlusHandle(),
		new ExplosiveShieldHandle(),
		new ExplosiveShieldPlusHandle(),
		new BloodthirstyHandle(),
		new BloodthirstyPlusHandle(),
		new FateLuckyHandle(),
		new FateLuckyPlusHandle(),*/
	};

    public static List<ICardHandle> getHandle()
    {
        return _handle;
    }

}
