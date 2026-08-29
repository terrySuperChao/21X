using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    private GameObject _scene = null;
    private GameObject _view = null;
    private GameObject _alert = null;
    private GameObject _tip = null;
    private List<IViewInfo> _viewInfos = null;
    private Dictionary<string,GameObject> _mapViews = new Dictionary<string, GameObject>();
    public void init(GameObject scene,string path) {
        this._viewInfos = JsonMgr.Instance.readObject<List<IViewInfo>>(path);
        this._scene = scene;
        this.initLayers();
    }

    private void initLayers() {
        this._view = this.newCreateObject("view");
        this._alert = this.newCreateObject("alert");
        this._tip = this.newCreateObject("tip");
        this._view.transform.SetParent(this._scene.transform, false);
        this._alert.transform.SetParent(this._scene.transform, false);
        this._tip.transform.SetParent(this._scene.transform, false);
    }

    private GameObject newCreateObject(string name) {
        GameObject newObject = new GameObject(name);
        Transform newTransform = newObject.transform;
        newTransform.position = Vector3.zero;
        newTransform.rotation = Quaternion.identity;
        newTransform.localScale = Vector3.one;
        return newObject;
    }

    public void showView(string name) {
        IViewInfo tempInfo = null;
        for (int i = 0; i < this._viewInfos.Count; i++) {
            IViewInfo viewInfo = this._viewInfos[i];
            if (viewInfo.name == name) {
                tempInfo = viewInfo;
                break;
            }
        }

        if (tempInfo == null) return;

        if (this._mapViews.ContainsKey(name))
        {
            GameObject gameObject = this._mapViews[name];
            IBaseView baseView = gameObject.GetComponent<IBaseView>();
            if (baseView != null)
            {
                baseView.refresh();
            }
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(tempInfo.resPath);
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            gameObject.name = name;
            Transform parent = null;
            if (tempInfo.viewType == (int)ViewType.view)
            {
                parent = this._view.transform;
            }
            else if (tempInfo.viewType == (int)ViewType.alert)
            {
                parent = this._alert.transform;
            }
            else if (tempInfo.viewType == (int)ViewType.tip)
            {
                parent = this._tip.transform;
            }

            if (parent != null)
            {
                Transform viewTransform = gameObject.transform;
                viewTransform.SetParent(parent, false);
                viewTransform.localPosition = Vector3.zero;
                viewTransform.localRotation = Quaternion.identity;
                viewTransform.localScale = Vector3.one;
            }
            
            IBaseView baseView = gameObject.GetComponent<IBaseView>();
            if (baseView != null)
            {
                baseView.init();
                baseView.beforeShow();
                baseView.afterShow();
            }

            //ɾ��ͬһ����
            string deleteName = "";
            foreach (var key in this._mapViews.Keys)
            {
                for (int i = 0; i < this._viewInfos.Count; i++)
                {
                    IViewInfo viewInfo = this._viewInfos[i];
                    if (viewInfo.viewType == tempInfo.viewType && viewInfo.name == key)
                    {
                        deleteName = key;
                        break;
                    }
                }
                if (deleteName != "") {
                    break;
                }
            }
            
            this.closeView(deleteName);

            this._mapViews[name] = gameObject;
        }
    }

    public void showAlert(string name, object conetnt, Action okAction,Action cancelAction) {
        this.showView(name);
        if (this._mapViews.ContainsKey(name)) {
            IBaseView baseView = this._mapViews[name].GetComponent<IBaseView>();
            if (baseView != null) {
                baseView.setAlert(conetnt, okAction, cancelAction);
            }
        }
    }

    public void showTips(string name, object conetnt) {
        this.showAlert(name,conetnt, () => { }, () => { });
    }

    public void closeView(string name) {
        if (this._mapViews.ContainsKey(name))
        {
            UnityEngine.Object.Destroy(this._mapViews[name]);
            this._mapViews.Remove(name);
        }
    }

    public void refreshView() {
        for (int i = 0; i < this._viewInfos.Count; i++)
        {
            IViewInfo viewInfo = this._viewInfos[i];
            if (viewInfo.viewType == (int)ViewType.view)
            {
                if (this._mapViews.ContainsKey(viewInfo.name)) { 
                    this.closeView(viewInfo.name);
                    this.showView(viewInfo.name);
                    break;
                }
            }
        }
    }
}
