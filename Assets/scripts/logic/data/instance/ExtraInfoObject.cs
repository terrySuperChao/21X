using System;
using System.Collections.Generic;

public class ExtraInfoObject : IExtraInfo
{
    //运行时
    private float _rtFreezeArmorValue = 0;
    private float _rtAddDefenseValue = 0;
    private float _rtOverflowBloodValue = 0;
    private bool _isMagicAttack = false;

    //效果
    private List<IBaseEffectData> _baseEffect = new List<IBaseEffectData>();
    private Func<int,IBaseEffectData> _baseEffectDataInstance = null;
       
    public void setRtFreezeArmorValue(float value)
    {
        this._rtFreezeArmorValue = value;
        this._rtFreezeArmorValue = this._rtFreezeArmorValue < 0 ? 0 : this._rtFreezeArmorValue;
    }

    public float getRtFreezeArmorValue()
    {
        return this._rtFreezeArmorValue;
    }

    public void clearRtFreezeArmorValue() {
        this._rtFreezeArmorValue = 0;
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

    public void setMagicAttack(bool isMagicAttack)
    {
        this._isMagicAttack = isMagicAttack;
    }
    public bool isMagicAttack()
    {
        return this._isMagicAttack;
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
            data = this._baseEffectDataInstance(id);
            this._baseEffect.Add(data);
        }
        return data;
    }

    public void addBaseEffectData(Pb.BaseEffectData value)
    {
        IBaseEffectData data = this._baseEffectDataInstance(value.Id);
        data.setState(value.State);        
        this._baseEffect.Add(data);
    }

    public void setBaseEffectDataInstance(Func<int,IBaseEffectData> func) {
        this._baseEffectDataInstance = func;
    }

    public void clearBaseEffectData() {
        this._baseEffect.Clear();
    }
}
