//ÅÆ¶Ñ
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameCtrl : Singleton<GameCtrl>
{
    private class ICacheMsg {
        string _key;
        System.Object[] _data;
        public ICacheMsg(string key, params System.Object[] data) {
            _key = key;
            _data = data;
        }
        public string getKey() {
            return _key;
        }

        public System.Object[] getData() {
            return _data;
        }
    }

    private List<ICacheMsg> _msgCache = new List<ICacheMsg>();
    private bool _canHandleMsg = false;

    public void addMsg(string key, params System.Object[] data) {
        _msgCache.Add(new ICacheMsg(key, data));
    }

    public void handleMessage()
    {
        if (_canHandleMsg && _msgCache.Count > 0)
        {
            _canHandleMsg = false;
            ICacheMsg msg = _msgCache[0];
            _msgCache.RemoveAt(0);
            EventDispatcher.Instance.emit(msg.getKey(), msg.getData());
        }
    }

    public void setHandleMessageComplete()
    {
        _canHandleMsg = true;
    }

    public void clearCacheMessage() {
        _canHandleMsg = false;
        _msgCache.Clear();
    }
}
