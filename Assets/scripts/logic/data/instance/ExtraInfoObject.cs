using System;
using System.Collections.Generic;

public class ExtraInfoObject : IExtraInfo
{
    //运行时
    private float _rtHurtValue = 0;
    private float _rtFreezeArmorValue = 0;
    private float _rtAddDefenseValue = 0;
    private float _rtOverflowBloodValue = 0;

    private List<BuffType> _buffTypes = new List<BuffType>();
    private Action<BuffAction, BuffType> _callback = null;

    //效果
    private List<IBaseEffectData> _baseEffect = new List<IBaseEffectData>();
    private Func<int,IBaseEffectData> _baseEffectEvent = null;
    
    public void setBuffAction(Action<BuffAction, BuffType> callback) {
        this._callback = callback;
    }

    public List<BuffType> getBuffs() {
        return this._buffTypes;
    }

    public void setRtHurtValue(float value)
    {
        this._rtHurtValue += value;
        this._rtHurtValue = this._rtHurtValue < 0 ? 0 : this._rtHurtValue;
    }
    public float getRtHurtVaule()
    {
        return this._rtHurtValue;
    }
    public void clearRtHurtValue() {
        this._rtHurtValue = 0;
    }

    public void setRtFreezeArmorValue(float value)
    {
        this._rtFreezeArmorValue = value;
        this._rtFreezeArmorValue = this._rtFreezeArmorValue < 0 ? 0 : this._rtFreezeArmorValue;
        this.addBuffType(BuffType.rtFreezeArmorValue, value);
    }

    public float getRtFreezeArmorValue()
    {
        return this._rtFreezeArmorValue;
    }

    public void clearRtFreezeArmorValue() {
        this._rtFreezeArmorValue = 0;
        this.removeBuffType(BuffType.rtFreezeArmorValue);
    }

    //添加的护甲值
    public void setRtAddDefenseValue(float value) {
        this._rtAddDefenseValue += value;
        this._rtAddDefenseValue = this._rtAddDefenseValue < 0 ? 0 : this._rtAddDefenseValue;
    }
    public float getRtAddDefenseValue() {
        return this._rtAddDefenseValue;
    }

    //溢出的治疗量
    public void setRtOverflowBloodValue(float value) {
        this._rtOverflowBloodValue += value;
        this._rtOverflowBloodValue = this._rtOverflowBloodValue < 0 ? 0 : this._rtOverflowBloodValue;
    }
    public float getRtOverflowBloodValue() {
        return this._rtOverflowBloodValue;
    }
    public void clearRtOverflowBloodValue() {
        this._rtOverflowBloodValue = 0;
    }

    //基础效果
    public List<IBaseEffectData> getBaseEffectDatas()
    {
        return this._baseEffect;
    }

    public IBaseEffectData getBaseEffectData(int id)
    {
        IBaseEffectData data = null;
        for (int i = 0; i < this._baseEffect.Count; i++)
        {
            if (this._baseEffect[i].getId() == id)
            {
                data = this._baseEffect[i];
                break;
            }
        }
        if (data == null)
        {
            data = this._baseEffectEvent(id);
            this._baseEffect.Add(data);
        }
        return data;
    }

    public void addBaseEffectData(Pb.BaseEffectData value)
    {
        IBaseEffectData data = this._baseEffectEvent(value.Id);
        data.setState(value.State);        
        this._baseEffect.Add(data);
    }

    public void setBaseEffectDataInstance(Func<int,IBaseEffectData> func) {
        this._baseEffectEvent = func;
    }

    private void addBuffType(BuffType type,float value) {
        if (value <= 0) return;

        int index = this._buffTypes.FindIndex(buffType => buffType == type);
        if (index == -1) {
            UnityEngine.Debug.Log("============="+type);
            this._buffTypes.Add(type);
            this._callback?.Invoke(BuffAction.add, type);
        }
    }

    private void removeBuffType(BuffType type) {
        int index = this._buffTypes.FindIndex(buffType => buffType == type);
        if(index != -1)
        {
            this._buffTypes.RemoveAt(index);
            this._callback?.Invoke(BuffAction.remove, type);
        }
    }
}
