using System.Collections.Generic;
using UnityEngine;

public class CardPartItem {
    public int index;
    public IPart partInfo;
    public TargetPart targetType;
    public RectTransform targetArea;
}

public class CardPartController {
    private List<CardPartItem> _items = new List<CardPartItem> ();
    public void addCardPartItem(int index,IPart partInfo, RectTransform targetArea, TargetPart targetType) {
        CardPartItem item = new CardPartItem();
        item.index = index;
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
                ImprintDataMgr.Instance.addPart(this._items[i].index, this._items[i].targetType, partInfo.getId());
                break;
            }
        }
    }

    public void deleteTargetArea(MonoBehaviour mono,int index, TargetPart targetType) {
        for (int i = 0; i < this._items.Count; i++)
        {
            if (this._items[i].index == index && this._items[i].targetType == targetType)
            {
                ImprintDataMgr.Instance.addPart(this._items[i].index, this._items[i].targetType, 0);
                this._items[i].partInfo = null;

                Transform transform = this._items[i].targetArea.gameObject.transform;
                for (int j = transform.childCount - 1; j >= 0; j--)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(j).gameObject);
                }
                break;
            }
        }
    }
}