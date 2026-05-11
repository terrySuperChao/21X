public class ExtraInfoObject : IExtraInfo
{
    private float _multATK = 0;
    private float _addCrit = 0;
    public void setMultATK(float value) {
        this._multATK += value;
    }

    public float getMultATK() {
        return this._multATK;
    }

    public void setAddCrit(float value) {
        this._addCrit += value;
    }

    public float getAddCrit() {
        return this._addCrit;
    }
}
