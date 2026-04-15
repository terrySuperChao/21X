using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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
            if ((config[i].getLevel() == 1 && - 1 == userCards.FindIndex(card => card.getType() == config[i].getType()) && userCards.Count != MAXSLOT) ||
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
        IUser attackUser = list.Find(user => user.isNpc() == isNpc);
        IUser defenseUser = list.Find(user => user.isNpc() != isNpc);
        ICardHandlePara handlePara = new CardHandleParaObject();
        handlePara.setRoundResult(new RoundResultObject());
        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
        this.handle(handlePara, type);
    }

    //触发升级
    private void triggerUpgrade(ICardHandlePara para, int triggerId) {
        bool isUpgrade = ImprintDataMgr.Instance.addTriggerNumber(para.getAttackUser().isNpc(), triggerId);
        if (!isUpgrade) return;

        TriggerPartInfo triggerPartInfo = GameStaticConfigMgr.Instance.getTriggerPartConfig().getTriggerPartId(triggerId);
        if (triggerPartInfo == null ) return;
        
        List<IPart> selectBasePart = new List<IPart>();
        List<int> upgradePartIds = new List<int>();
        upgradePartIds.AddRange(triggerPartInfo.getPartIds());
        for (int i = 0; i < MAXSLOT; i++)
        {
            int index = RandomMgr.Instance.getRangeInt(0, upgradePartIds.Count);
            int basePartId = upgradePartIds[index];
            upgradePartIds.Remove(basePartId);

            IPart basePart = GameStaticConfigMgr.Instance.getBasePartConfig().getBasePartId(basePartId);
            if (basePart != null) {
                selectBasePart.Add(basePart);
            }
        }

        ICandidacyPartPara partPara = new CandidacyPartPara(para.getAttackUser(), selectBasePart, triggerId);
        GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, partPara);
    }

    //
    public void handle(ICardHandlePara para, CardHandleType type) {
        List<ICard> cards = FightPokerMgr.Instance.getUserCards(para.getUser());
        List<ICardHandle> handles = CardConfig.getHandle();
        for (int i = 0; i < cards.Count; i++) {
            para.setCard(cards[i]);
            int cardId = cards[i].getId();
            int index = cardId - 1;
            if (index >= handles.Count) continue;
            int beforeCount = GameMessage.Instance.getMsgCount();
            Type objType = handles[index].GetType();
            string methodName = Enum.GetName(typeof(CardHandleType), type);
            MethodInfo method = objType.GetMethod(methodName + "Handle");
            method?.Invoke(handles[index], new object[] { para });
            int afterCount = GameMessage.Instance.getMsgCount();

            //说明增加了一条数据
            if (afterCount > beforeCount){
                //触发升级的内容
                this.triggerUpgrade(para, cardId);
            }

            /*
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
            }*/
        }
    }
}
