using System;
using UnityEngine;

public interface IBaseView
{
    public void init();
    public void beforeShow();
    public void refresh();
    public void afterShow();
    public void setAlert(string content, Action okAction, Action cancelAction) => Debug.Log("Default implementation");
}
