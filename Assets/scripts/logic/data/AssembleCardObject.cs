
public class AssembleCardObject : IAssembleCard
{
    private int _baseDataId = 0;
    private int _triggerId = 0;
    private int _level = 0;

    public AssembleCardObject(int baseDataId, int triggerId, int level) {
        this._baseDataId = baseDataId;
        this._triggerId = triggerId;
        this._level = level;
    }

    public int getBaseDataId() {
        return this._baseDataId;
    }
    public void setBaseDataId(int id) {
        this._baseDataId = id;
    }

    public int getTriggerId() {
        return this._triggerId;
    }
    public void setTriggerId(int id) {
        this._triggerId = id;
    }

    public int getLevel() { 
        return this._level;
    }
    public void setLevel(int value) { 
        this._level = value;
    }
}
