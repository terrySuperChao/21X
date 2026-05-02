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
    private void triggerUpgrade(ICardHandlePara para,IAssembleCard card) {
        bool isNpc = para.getAttackUser().isNpc();
        bool isUpgrade = ImprintDataMgr.Instance.addTriggerNumber(isNpc, card.getTriggerId());
        if (!isUpgrade) return;

        IAssembleCard assembleCard = ImprintDataMgr.Instance.getAssembleCard(isNpc, card.getTriggerId());
        if (assembleCard == null) return;

        BaseEffectInfo info = GameStaticConfigMgr.Instance.getBaseEffectConfig().getBaseEffectId(assembleCard.getBaseEffectId());
        if (info == null) return;

        List<IPart> selectPart = new List<IPart>();
        List<IPart> advancedPart = new List<IPart>();

        List<AdvancedEffectInfo> effectInfos = GameStaticConfigMgr.Instance.getAdvancedEffectConfig().getAdvancedEffect();
        for (int i = 0; i < effectInfos.Count; i++) {
            if (effectInfos[i].getBelongBase().IndexOf(info.Correspond_Advanced) == 0) {
                if (ImprintDataMgr.Instance.hasAdvancedEffect(isNpc, effectInfos[i].getId()))
                {
                    advancedPart.Add(effectInfos[i]);
                }
            }
        }

        for (int i = 0; i < MAXSLOT; i++)
        {
            int index = RandomMgr.Instance.getRangeInt(0, advancedPart.Count);
            IPart part = advancedPart[index];
            selectPart.Add(part);
            advancedPart.Remove(part);
        }

        ICandidacyPartPara partPara = new CandidacyPartPara(para.getAttackUser(),card, selectPart);
        GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, partPara);
        
    }

    //
    public void handle(ICardHandlePara para, CardHandleType type) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getUser().isNpc());
        for (int i = 0; i < cards.Count; i++) {
            TriggerInfo info = GameStaticConfigMgr.Instance.getTriggerConfig().getTriggerId(cards[i].getTriggerId());
            if (info == null) {
                continue;
            }

            ITriggerHandle handle = TriggerEvent.getTriggerEventHandle(info.Trigger);
            if (handle == null) {
                continue;
            }
            int beforeCount = GameMessage.Instance.getMsgCount();
            Type objType = handle.GetType();
            string methodName = Enum.GetName(typeof(CardHandleType), type);
            MethodInfo method = objType.GetMethod(methodName + "Handle");
            method?.Invoke(handle, new object[] { para });
            int afterCount = GameMessage.Instance.getMsgCount();

            //说明增加了一条数据
            if (afterCount > beforeCount){
                //触发升级的内容
                this.triggerUpgrade(para, cards[i]);
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
