using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPartItem {
    public int index;
    public IPart partInfo;
    public TargetPart targetType;
    public RectTransform targetArea;
}

public class CardPartController {
    private Action<int, TargetPart> _dragCallBack = null;
    private List<CardPartItem> _items = new List<CardPartItem> ();
    public void addCardPartItem(int index,IPart partInfo, RectTransform targetArea, TargetPart targetType) {
        CardPartItem item = new CardPartItem();
        item.index = index;
        item.partInfo = partInfo;
        item.targetArea = targetArea;
        item.targetType = targetType;
        this._items.Add(item);
    }

    public void setDragCallBack(Action<int, TargetPart> dragCallBack) { 
        this._dragCallBack = dragCallBack;
    }

    public List<RectTransform> getTargetAreas(TargetPart targetType) {
        List < RectTransform > list = new List < RectTransform >();
        for (int i = 0; i < this._items.Count; i++) {
            if (this._items[i].targetType == targetType) {
                list.Add(this._items[i].targetArea);
            }
        }
        return list;
    }

    public RectTransform getTargetArea(int index, TargetPart targetType)
    {
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].index == index && this._items[i].targetType == targetType)
            {
                return this._items[i].targetArea;
            }
        }
        return null;
    }

    public IPart getTargetAreaPart(int index, TargetPart targetType)
    {
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].index == index && this._items[i].targetType == targetType)
            {
                return this._items[i].partInfo;
            }
        }
        return null;
    }

    public void matchTargetAreaPart(RectTransform targetArea, IPart partInfo) {
        CardPartItem item = null;
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].targetArea == targetArea)
            {
                item = this._items[i];
                break;
            }
        }

        if (item != null) {
            if (item.partInfo != null)
            {
                if (this._dragCallBack != null)
                {
                    this._dragCallBack(item.index, item.partInfo.getTargetPart());
                }
            }

            for (int i = 0; i < this._items.Count; i++)
            {
                if (this._items[i] != item &&
                    this._items[i].index == item.index &&
                    this._items[i].partInfo != null) {
                    IPart part = this._items[i].partInfo;
                    if (part.getTargetPart() == TargetPart.trigger) {
                        if (part.getCorrespondBase().IndexOf(partInfo.getBelongBase()) != 0) {
                            this._dragCallBack(this._items[i].index, this._items[i].partInfo.getTargetPart());
                        }
                    }
                    else
                    {
                        if (partInfo.getCorrespondBase().IndexOf(part.getBelongBase()) != 0)
                        {
                            this._dragCallBack(this._items[i].index, this._items[i].partInfo.getTargetPart());
                        }
                    }
                    break;
                }
            }

            item.partInfo = partInfo;
            ImprintDataMgr.Instance.addPart(item.index, item.targetType, partInfo.getId());
        }
    }

    public void clearTargetAreaPart(IPart partInfo) {
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].partInfo == partInfo)
            {
                this._items[i].partInfo = null;
                ImprintDataMgr.Instance.addPart(this._items[i].index, this._items[i].targetType, 0);
                break;
            }
        }
    }
}