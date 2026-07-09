using System;
using System.Collections.Generic;
using System.Reflection;

public class GameCardMgr : Singleton<GameCardMgr>
{
    private const int MAXSLOT = 3;
    private string callFuncPath = "";
    public void init()
    {
    
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
            if (para.getAttackUser().getExtraInfo().isMagicAttack()) {
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

        if (para.getAttackUser().isNpc())
        {
            int idx =RandomMgr.Instance.getRangeInt(0, selectPart.Count);
            GameReqMgr.Instance.requestUpgradePart(para.getAttackUser(), para.getAssembleCard(), selectPart[idx]);
        }
        else {
            ICandidacyPartPara partPara = new CandidacyPartPara(para.getAttackUser(), para.getAssembleCard(), selectPart);
            GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, partPara);
        }
    }

    public void handle(IBaseEffectHandlePara para)
    {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getAttackUser().isNpc());
        for (int i = 0; i < cards.Count; i++)
        {
            //初级效果
            IBaseEffectHandle baseEffectHandle = BaseEffectHandleMgr.Instance.getBaseEffectHandle(cards[i].getBaseEffect().getId());
            if (baseEffectHandle != null)
            {
                baseEffectHandle.effect(para);
            }

            IBaseEffectHandle advancedEffectHandle = AdvancedEffectHandleMgr.Instance.getAdvancedEffectHandle(cards[i].getAdvancedEffectId());
            if (advancedEffectHandle != null)
            {
                advancedEffectHandle.effect(para);
            }
        }
    }

    //
    public void handle(ITriggerHandlePara para, TriggerEvent type, float temporaryValue = 0) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getAttackUser().isNpc());
        for (int i = 0; i < cards.Count; i++) {
            para.setAssembleCard(cards[i]);
            para.setTemporaryValue(temporaryValue);

            List<ITriggerHandle> handles = TriggerHandleMgr.Instance.getTriggerHandle(para);
            if (handles == null) {
                continue;
            }

            bool success = false;
            for (int j = 0; j < handles.Count; j++) { 
                Type objType = handles[j].GetType();
                string methodName = Enum.GetName(typeof(TriggerEvent), type);
                MethodInfo method = objType.GetMethod(this.snakeToCamel(methodName) + "Handle");
                object state = method?.Invoke(handles[j], new object[] { para });

                //触发了
                if ((bool)state) {
                    if (this.checkCycleCall(para, handles[j], type)){
                        return;
                    }
                    else {
                        success = true;
                        break;
                    }
                }
            }

            //触发了
            if (!success)
            {
                continue;
            }

            //初级效果
            IBaseEffectHandle baseEffectHandle = BaseEffectHandleMgr.Instance.getBaseEffectHandle(para); 
            if (baseEffectHandle != null) {
                baseEffectHandle.handle(para);
            }
            if (GameBloodMgr.Instance.checkGameOver(para)) {
                break;
            }

            //高级效果
            IBaseEffectHandle advancedEffectHandle = AdvancedEffectHandleMgr.Instance.getAdvancedEffectHandle(para);
            if (advancedEffectHandle != null){
                advancedEffectHandle.handle(para);
            }
            if (GameBloodMgr.Instance.checkGameOver(para)){
                break;
            }

            //触发升级的内容
            this.triggerUpgrade(i,para);
        }
        this.callFuncPath = "";
    }

    //防止递归调用,
    private bool checkCycleCall(ITriggerHandlePara para, ITriggerHandle handle, TriggerEvent type) {
        string callFuncPathTmp = para.getAttackUser().GetHashCode().ToString() +
                          para.getDefenseUser().GetHashCode().ToString() +
                          para.getAssembleCard().GetHashCode().ToString() +
                          handle.GetHashCode().ToString() +
                          type.ToString();
        if (callFuncPathTmp == this.callFuncPath)
        {
            UnityEngine.Debug.Log("callFuncPathTmp=====>>>" + callFuncPathTmp);
            UnityEngine.Debug.Log("callFuncPath=====>>>" + this.callFuncPath);
            return true;
        }
        else
        {
            this.callFuncPath = callFuncPathTmp;
            return false;
        }
    }

    //
    private string snakeToCamel(string input) {
        if (string.IsNullOrEmpty(input)) {
            return input;
        }
        
        string str = "";
        string[] parts = input.ToLower().Split("_");
        for (int i = 0; i < parts.Length; i++) {
            if (i > 0)
            {
                str += parts[i].Substring(0,1).ToUpper() + parts[i].Substring(1, parts[i].Length - 1);
            }
            else {
                str = parts[i];
            }
        }
        return str;
    }

    public float getBaseEffectValue(IUser user,BaseEffectType type) {
        float value = 0;
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
        for (int i = 0; i < cards.Count; i++) {
            IBaseEffectData data = user.getExtraInfo().getBaseEffectData(cards[i].getBaseEffectId());
            if (!data.isState()) continue;

            IBaseEffectValue baseEffectValue = data.getBaseEffectValues().Find(value=> value.getType() == type);
            if (baseEffectValue != null) {
                value += baseEffectValue.getValue();
            }
        }
        return value;
    }

    public float clearBaseEffectValue(IUser user, BaseEffectType type)
    {
        //移除
        FightPokerMgr.Instance.getBuffEffect().removeBuffType(user, type);

        float value = 0;
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
        for (int i = 0; i < cards.Count; i++)
        {
            IBaseEffectData data = user.getExtraInfo().getBaseEffectData(cards[i].getBaseEffectId());
            if (!data.isState()) continue;

            IBaseEffectValue baseEffectValue = data.getBaseEffectValues().Find(value => value.getType() == type);
            if (baseEffectValue != null) {
                data.setState(0);
                baseEffectValue.clearValue();
            }
        }
        return value;
    }
}
