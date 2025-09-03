using System;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher : Singleton<EventDispatcher>
{
    
    private Dictionary<string, List<Action<System.Object[]>>> _eventCacheDic = new Dictionary<string, List<Action<System.Object[]>>>();
    public void emit(string eventName, params System.Object[] data) {
        if (!_eventCacheDic.ContainsKey(eventName)) return;
        List<Action<System.Object[]>> list = _eventCacheDic[eventName];
        for (int i = 0; i < list.Count; i++){
            list[i](data);
        }
    }

    public void on(string eventName, Action<System.Object[]> action) {
        if (!_eventCacheDic.ContainsKey(eventName)){
            _eventCacheDic[eventName] = new List<Action<System.Object[]>>() { action };
        }else {
            List<Action<System.Object[]>> list = _eventCacheDic[eventName];
            for (int i = 0; i < list.Count; i++){
                if (list[i] == action){
                    return;
                }
            }
            list.Add(action);
        }
           
    }

    public void off(string eventName, Action<System.Object[]> action) {
        if (!_eventCacheDic.ContainsKey(eventName)) return;

        List<Action<System.Object[]>> list = _eventCacheDic[eventName];
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == action) {
                list.RemoveAt(i);
                break;
            }
        }
    }
}
