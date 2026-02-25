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
        this._view.transform.SetParent(this._scene.transform);
        this._alert.transform.SetParent(this._scene.transform);
        this._tip.transform.SetParent(this._scene.transform);
    }

    private GameObject newCreateObject(string name) {
        GameObject newObject = new GameObject(name);
        // 新创建的对象自动包含Transform组件
        Transform newTransform = newObject.transform; // 获取其Transform组件
        // 可以通过Transform组件设置位置、旋转、缩放
        newTransform.position = new Vector3(0, 0, 0);
        newTransform.rotation = Quaternion.Euler(0, 0, 0);
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
            if (tempInfo.viewType == (int)ViewType.view)
            {
                gameObject.transform.SetParent(this._view.transform);
            }
            else if (tempInfo.viewType == (int)ViewType.alert)
            {
                gameObject.transform.SetParent(this._alert.transform);
            }
            else if (tempInfo.viewType == (int)ViewType.alert)
            {
                gameObject.transform.SetParent(this._tip.transform);
            }
            
            IBaseView baseView = gameObject.GetComponent<IBaseView>();
            if (baseView != null)
            {
                baseView.init();
                baseView.beforeShow();
                baseView.afterShow();
            }

            //删除同一类型
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

    public void showAlert(string name,string conetnt, Action okAction,Action cancelAction) {
        this.showView(name);
        if (this._mapViews.ContainsKey(name)) {
            IBaseView baseView = this._mapViews[name].GetComponent<IBaseView>();
            if (baseView != null) {
                baseView.setAlert(conetnt, okAction, cancelAction);
            }
        }
    }

    public void showTips(string name, string conetnt) {
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
