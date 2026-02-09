using UnityEngine;

public interface IBaseView
{
    public void init();
    public void beforeShow();
    public void refresh();
    public void afterShow();

}
