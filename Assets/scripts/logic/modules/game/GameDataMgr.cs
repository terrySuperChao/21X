using Pb;

public class GameDataMgr : Singleton<GameDataMgr>
{
    public GameData gameData;
    public void init(GameData data) {
        this.gameData = data;
    }
    public void deserialized() {
        RandomMgr.Instance.deserialized(this.gameData);
        BarrierDataMgr.Instance.deserialized(this.gameData);
    }

    public void serialized() {
        RandomMgr.Instance.serialized(this.gameData);
        BarrierDataMgr.Instance.serialized(this.gameData);
    }

    public GameState getGameState() {
        return (GameState)this.gameData.GameState;
    }

    public void setGameState(GameState state)
    {
        this.gameData.GameState = (int)state;
    }

    public PageIndex getPageIndex()
    {
        return (PageIndex)this.gameData.PageIndex;
    }

    public void setPageIndex(PageIndex pageIndex)
    {
        this.gameData.PageIndex = (int)pageIndex;
    }
}
