using System;
using System.Collections.Generic;
using System.Reflection;
using Pb;

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

    //触发升级
    private void triggerUpgrade(int index,ITriggerHandlePara para) {
        //添加触发的次数
        if (index == 0)//触发3次数
        {
            para.getAssembleCard().addTriggerNumber();
        }
        else if (index == 1)//触发blackJock
        {
            if (para.getGameSettlePara().isBlackJack()) {
                para.getAssembleCard().addTriggerNumber();
            }
        }
        else if (index == 2) {//触发魔法技能
            if (para.isMagicAttack()) {
                para.getAssembleCard().addTriggerNumber();
            }
        }
        
        if (para.getAssembleCard().getTriggerNumber() != para.getAssembleCard().getUpgradeNumber()) {
            return;
        }

        List<IPart> selectPart = new List<IPart>();
        List<IPart> advancedPart = new List<IPart>();

        List<AdvancedEffectInfo> effectInfos = GameStaticConfigMgr.Instance.getAdvancedEffectConfig().getAdvancedEffect();
        for (int i = 0; i < effectInfos.Count; i++) {
            if (effectInfos[i].getProfession() == 0 ||
                effectInfos[i].getProfession() == PlayerDataMgr.Instance.getRoleId())
            {
                if (effectInfos[i].getBelongBase().IndexOf(para.getAssembleCard().getBaseEffect().getCorrespondAdvanced()) == 0)
                {
                    if (!ImprintDataMgr.Instance.hasAdvancedEffect(para.getAttackUser().isNpc(), effectInfos[i].getId()))
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
            int idx = RandomMgr.Instance.getRangeInt(0, advancedPart.Count);
            IPart part = advancedPart[idx];
            selectPart.Add(part);
            advancedPart.Remove(part);
        }

        ICandidacyPartPara partPara = new CandidacyPartPara(para.getAttackUser(), para.getAssembleCard(), selectPart);
        GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, partPara);
    }


    public void handle(ICardHandlePara para, CardHandleType type) {

    }

    //
    public void handle(ITriggerHandlePara para, TriggerEvent type) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getUser().isNpc());
        for (int i = 0; i < cards.Count; i++) {
            para.setAssembleCard(cards[i]);

            List<ITriggerHandle> handles = TriggerHandle.getTriggerHandle(para);
            if (handles == null) {
                continue;
            }

            bool success = false;
            for (int j = 0; j < handles.Count; j++) { 
                Type objType = handles[j].GetType();
                string methodName = Enum.GetName(typeof(TriggerEvent), type);
                MethodInfo method = objType.GetMethod(methodName + "Handle");
                object state = method?.Invoke(handles[j], new object[] { para });

                //触发了
                if ((bool)state) {
                    success = true;
                    break;
                }
            }

            //说明增加了一条数据
            if (!success){
                continue;
            }

            //初级效果
            IBaseEffectHandle baseEffectHandle = BaseEffectHandle.getBaseEffectHandle(para);
            if (baseEffectHandle != null) {
                baseEffectHandle.handle(para);
            }
            if (GameLossBloodMgr.Instance.checkGameOver(para)) {
                break;
            }

            //高级效果
            IBaseEffectHandle advancedEffectHandle = BaseEffectHandle.getAdvancedEffectHandle(para);
            if (advancedEffectHandle != null){
                advancedEffectHandle.handle(para);
            }
            if (GameLossBloodMgr.Instance.checkGameOver(para)){
                break;
            }

            //触发升级的内容
            this.triggerUpgrade(i,para);
        }
    }
}
