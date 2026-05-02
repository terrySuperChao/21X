//�ƶ�

using System.Collections.Generic;

public class TriggerEventConfig
{
    private static List<ICard> _config = new List<ICard> { 
        new CardObject(1,(int)CardType.cardType1,1,"���Ҵ�ʦ","ÿ�λ�ú�������ʱ����ʵ����ÿ�ź����ƶ���Ҫ�����жϣ�������ֵ����+1"),
		new CardObject(2,(int)CardType.cardType1,2,"���Ҵ�ʦ+","ÿ�λ�ú�������ʱ������ֵ����+2������50%���ʴ�����͸���������Ի��ף�ֱ����ɹ�����"),
		new CardObject(3,(int)CardType.cardType2,1,"������ʹ","ÿ�λ�ú�������ʱ������Ч������20%"),
		new CardObject(4,(int)CardType.cardType2,2,"������ʹ+","ÿ�λ�ú�������ʱ������Ч������50%�����������������ת��Ϊ��������"),
		new CardObject(5,(int)CardType.cardType3,1,"÷����ʦ","ÿ�λ��÷������ʱ������ֵ����+1"),
		new CardObject(6,(int)CardType.cardType3,2,"÷����ʦ+","ÿ�λ��÷������ʱ������ֵ����+2�����ͷż��ܺ���30%��ħ��ֵ"),
		new CardObject(7,(int)CardType.cardType4,1,"���黤��","ÿ�λ�÷�������ʱ��������20%�Ļ���ֵ"),
		new CardObject(8,(int)CardType.cardType4,2,"���黤��+","ÿ�λ�÷�������ʱ��������50%�Ļ���ֵ����ÿ�ο۳�����ʱ������50%����ʵ"),
		new CardObject(9,(int)CardType.cardType5,1,"�����ع�","ÿ�η���ʱ��1�����·��ƵĻ���"),
		new CardObject(10,(int)CardType.cardType5,2,"�����ع�+","ÿ�η���ʱ��2�����·��ƵĻ��ᣬ�ҿ�ָ����ɫ"),
		new CardObject(11,(int)CardType.cardType6,1,"��սʿ֮ŭ","ÿ�ι�������20%�Ĺ�����"),
		new CardObject(12,(int)CardType.cardType6,2,"��սʿ֮ŭ+","ÿ�ι�������100%�Ĺ������������������10��Ѫ��"), 
        new CardObject(13,(int)CardType.cardType7,1,"ħ������","ÿ�λ��ħ��ֵʱ��������Է����1-2�����ħ���˺�"),
		new CardObject(14,(int)CardType.cardType7,2,"ħ������+","ÿ�λ��ħ��ֵʱ��������Է����2-4�����ħ���˺����Ҳ���21���Ƿ��ʤ��ÿ�غ϶�����5��ħ��ֵ"),
		new CardObject(15,(int)CardType.cardType8,1,"����֮��","ÿ�α��ƣ�������5�㻤��"),
		new CardObject(16,(int)CardType.cardType8,2,"����֮��+","ÿ�α��ƣ�������10�㻤�ף�����50%���ʷ���"),
		new CardObject(17,(int)CardType.cardType9,1,"��Ѫ��","�����Ѫ������25%ʱ��ÿ�λ�õ��������Է���"),
		new CardObject(18,(int)CardType.cardType9,2,"��Ѫ��+","�����Ѫ������25%ʱ��ÿ�λ�õ��������Է�����������Ѫ������5%��ÿ�λ�õ��������Է�5��"),
		new CardObject(19,(int)CardType.cardType10,1,"��������","�Է�ÿ��Ҫ�ƣ���40%�ļ��ʶ�����1������"),
		new CardObject(20,(int)CardType.cardType10,2,"��������+","�Է�ÿ��Ҫ�ƣ���40%�ļ��ʶ�����2������"),
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
		new FateLuckyPlusHandle(),
    };

    public static List<ICardHandle> getHandle()
    {
        return _handle;
    }

	public static ICard getCard(int cardId) {
		return _config.Find(card => card.getId() == cardId);
    }
}
