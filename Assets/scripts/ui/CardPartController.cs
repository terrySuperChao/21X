using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPartItem {
    public IPart partInfo;
    public TargetPart targetType;
    public RectTransform targetArea;
}

public class CardPartController {
    private List<CardPartItem> _items = new List<CardPartItem> ();
    public void addCardPartItem(IPart partInfo, RectTransform targetArea, TargetPart targetType) {
        CardPartItem item = new CardPartItem();
        item.partInfo = partInfo;
        item.targetArea = targetArea;
        item.targetType = targetType;
        this._items.Add(item);
    }

    public List<RectTransform> getTargetAreas(TargetPart targetType) {
        List < RectTransform > list = new List < RectTransform >();
        for (int i = 0; i < this._items.Count; i++) {
            if (this._items[i].targetType == targetType && this._items[i].partInfo == null) {
                list.Add(this._items[i].targetArea);
            }
        }
        return list;
    }

    public void matchTargetArea(RectTransform targetArea, IPart partInfo) {
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].targetArea == targetArea)
            {
                this._items[i].partInfo = partInfo;
                break;
            }
        }
    }
}