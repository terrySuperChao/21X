public interface IExtraInfo
{
    //下次攻击额外造成伤害
    public void setMultATK(float value);
    public float getMultATK();

    //固定增加 %s% 暴击率
    public void setAddCrit(float value);
    public float getAddCrit();
}
