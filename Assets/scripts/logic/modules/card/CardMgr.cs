using System;
using System.Collections.Generic;
using System.Reflection;

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
        ITriggerHandlePara handlePara = new TriggerHandleParaObject();
        handlePara.setRoundResult(new RoundResultObject());
        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
        this.handle(handlePara, type);
    }

    //触发升级
    private void triggerUpgrade(ITriggerHandlePara para,IAssembleCard card) {
        bool isNpc = para.getAttackUser().isNpc();
        bool isUpgrade = ImprintDataMgr.Instance.addTriggerNumber(isNpc, card.getTriggerId());
        if (!isUpgrade) return;

        BaseEffectInfo info = GameStaticConfigMgr.Instance.getBaseEffectConfig().getBaseEffectId(card.getBaseEffectId());
        if (info == null) return;

        List<IPart> selectPart = new List<IPart>();
        List<IPart> advancedPart = new List<IPart>();

        List<AdvancedEffectInfo> effectInfos = GameStaticConfigMgr.Instance.getAdvancedEffectConfig().getAdvancedEffect();
        for (int i = 0; i < effectInfos.Count; i++) {
            if (effectInfos[i].getProfession() == 0 ||
                effectInfos[i].getProfession() == PlayerDataMgr.Instance.getRoleId())
            {
                if (effectInfos[i].getBelongBase().IndexOf(info.Correspond_Advanced) == 0)
                {
                    if (ImprintDataMgr.Instance.hasAdvancedEffect(isNpc, effectInfos[i].getId()))
                    {
                        advancedPart.Add(effectInfos[i]);
                    }
                }
            }
        }

        if (advancedPart.Count == 0) return;

        for (int i = 0; i < MAXSLOT; i++)
        {
            if (advancedPart.Count == 0){
                break;
            }
            int index = RandomMgr.Instance.getRangeInt(0, advancedPart.Count);
            IPart part = advancedPart[index];
            selectPart.Add(part);
            advancedPart.Remove(part);
        }

        ICandidacyPartPara partPara = new CandidacyPartPara(para.getAttackUser(),card, selectPart);
        GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, partPara);
    }


    public void handle(ICardHandlePara para, CardHandleType type) {

    }

    //
    public void handle(ITriggerHandlePara para, CardHandleType type) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getUser().isNpc());
        for (int i = 0; i < cards.Count; i++) {
            ITriggerHandle handle = TriggerEventHandle.getTriggerEventHandle(cards[i].getTrigger().getTriggerEvent());
            if (handle == null) { continue;}

            para.setAssembleCard(cards[i]);

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
        }
    }
}
