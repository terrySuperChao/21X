//
using System.Collections.Generic;

public class BuffEffectObject : IBuffEffect
{
    private List<BaseEffectType> _npcBuffTypes = new List<BaseEffectType>();
    private List<BaseEffectType> _playerBuffTypes = new List<BaseEffectType>();
    public List<BaseEffectType> getBuffs(IUser user)
    {
        return this.getBuffList(user);
    }

    public void addBuffType(IUser user, BaseEffectType type, bool addMsg = true)
    {
        if (this.isFilter(type))
        {
            return;
        }

        List<BaseEffectType> buffTypes = this.getBuffList(user);
        int index = buffTypes.FindIndex(buffType => buffType == type);
        if (index != -1)
        {
            return;
        }

        buffTypes.Add(type);

        if (addMsg)
        {
            IUIBuffPara buffPara = new UIBuffParaObject(user, type);
            GameMessage.Instance.addMsg(GameConst.ADDBUFFTYPE, buffPara);
        }
    }

    public void removeBuffType(IUser user, BaseEffectType type)
    {
        List<BaseEffectType> buffTypes = this.getBuffList(user);
        int index = buffTypes.FindIndex(buffType => buffType == type);
        if (index == -1){
            return;
        }

        buffTypes.RemoveAt(index);
        IUIBuffPara buffPara = new UIBuffParaObject(user, type);
        GameMessage.Instance.addMsg(GameConst.REMOVEBUFFTYPE, buffPara);
    }

    

    private List<BaseEffectType> getBuffList(IUser user) {
        return user.isNpc() ? this._npcBuffTypes : this._playerBuffTypes;
    }

    private bool isFilter(BaseEffectType type)
    {
        if (type == BaseEffectType.addLevel)
        {
            return true;
        }
        else {
            return false;
        }
    }
}
