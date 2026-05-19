using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffPartPopup : MonoBehaviour
{
    public Text partName;
    private IUser _user;
    private Dictionary<BuffType, string> _buffDic = new Dictionary<BuffType, string>();
    private Dictionary<BuffType, Func<float>> _buffAction = new Dictionary<BuffType, Func<float>>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void setUser(IUser user)
    {
        this._user = user;
        this.initBuffDesc();
        this.initBuffAction();
    }

    public void setBuffType(BuffType buffType) {
        string desc = "";
        if (this._buffDic.ContainsKey(buffType))
        {
            desc = this._buffDic[buffType];
        }

        float value = 0f;
        if (this._buffAction.ContainsKey(buffType))
        {
            value = this._buffAction[buffType].Invoke();
        }
        this.partName.text = GameUtils.formatDescription(desc, value);
    }

    private void initBuffDesc() {
        if (this._buffDic.Count != 0) return;
        this._buffDic.Add(BuffType.multATK, "普通攻击额外造成<color=red>%s%</color>的伤害");
    }

    private void initBuffAction()
    {
        this._buffAction.Clear();
        this._buffAction.Add(BuffType.multATK, this._user.getExtraInfo().getMultATK);
    }
}   
