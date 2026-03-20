using System.Collections.Generic;

public class CardMgr: Singleton<CardMgr>
{
    private const int MAXSLOT = 3;
    public void init()
    {
    
    }

    public List<ICard> getRandomCard(IUser user) {
        List<ICard> config = CardConfig.getConfig();
        List<ICard> list0 = new List<ICard>();
        List<ICard> list1 = new List<ICard>();
        List<ICard> list2 = new List<ICard>();
        List<ICard> userCards = FightPokerMgr.Instance.getUserCards(user);

        //判断满槽了
        if (userCards.FindAll(item => item.getLevel() > 1).Count >= MAXSLOT) {
            return list0;
        }
        
        //没有该类型的选择Level=1,
        //拥有该类型的选择Level=2,
        for (int i = 0; i < config.Count; i++) {
            if ((config[i].getLevel() == 1 && -1 == userCards.FindIndex(card => card.getType() == config[i].getType())) ||
                (config[i].getLevel() == 2 && -1 != userCards.FindIndex(card => card.getType() == config[i].getType() && card.getLevel() == 1)))
            {
                list1.Add(config[i]);
            }
        }

        for (int i = 0; i < MAXSLOT; i++) {
            if (list1.Count == 0) break;
            ICard card = list1[RandomMgr.Instance.getRangeInt(0,list1.Count)];
            list0.Add(card);
            list1.Remove(card);
        }
        return list0;
    }

    public int getMaxSlot() {
        return MAXSLOT;
    }

    public void cardHandleTypeHandle(List<IUser> list, bool isNpc, CardHandleType type)
    {
        ICardHandlePara handlePara = new CardHandleParaObject();
        IRoundResult roundResult = new RoundResultObject();
        handlePara.setRoundResult(roundResult);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isNpc() == isNpc)
            {
                handlePara.setUser(list[i]);
                handlePara.setAttackUser(list[i]);
            }
            else
            {
                handlePara.setDefenseUser(list[i]);
            }
        }
        this.handle(handlePara, type);
    }

    //
    public void handle(ICardHandlePara para, CardHandleType type) {
        List<ICard> cards = FightPokerMgr.Instance.getUserCards(para.getUser());
        List<ICardHandle> handles = CardConfig.getHandle();
        for (int i = 0; i < cards.Count; i++) {
            para.setCard(cards[i]);
            int index = cards[i].getId() - 1;
            if (index >= handles.Count) continue;
            switch (type) {
                case CardHandleType.addNewCardAfter:
                    handles[index].addNewCardAfterHandle(para);
                    break;
                case CardHandleType.handPokerAfter:
                    handles[index].handPokerAfterHandle(para);
                    break;
                case CardHandleType.dealPokerAfter:
                    handles[index].dealPokerAfterHandle(para);
                    break;
                case CardHandleType.roundBegin:
                    handles[index].roundBeginHandle(para);
                    break;
                case CardHandleType.roundAddValueBefore:
                    handles[index].roundAddValueBeforeHandle(para);
                    break;
                case CardHandleType.roundAddValue:
                    handles[index].roundAddValueHandle(para);
                    break;
                case CardHandleType.roundAddMagic:
                    handles[index].roundAddMagicHandle(para);
                    break;
                case CardHandleType.roundSpecialAttr:
                    handles[index].roundSpecialAttrHandle(para);
                    break;
                case CardHandleType.roundAttackBegin:
                    handles[index].roundAttackBeforeHandle(para);
                    break;
                case CardHandleType.roundAttack:
                    handles[index].roundAttackHandle(para);
                    break;
                case CardHandleType.roundMagicAttack:
                    handles[index].roundMagicAttackHandle(para);
                    break;
                case CardHandleType.roundSubDefense:
                    handles[index].roundSubDefenseHandle(para);
                    break;
                case CardHandleType.roundSubBlood:
                    handles[index].roundSubBloodHandle(para);
                    break;
                case CardHandleType.roundAttackAfter:
                    handles[index].roundAttackAfterHandle(para);
                    break;
                case CardHandleType.roundEnd:
                    handles[index].roundEndHandle(para);
                    break;
            }
        }
    }
}
