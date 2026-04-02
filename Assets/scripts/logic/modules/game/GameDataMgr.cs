using Pb;
using UnityEngine;
public class GameDataMgr : Singleton<GameDataMgr>
{
    private GameProperty _gameProperty;
    private GameData _gameData;
    public void init(GameProperty data) {
        this._gameProperty = data;
        this._gameData = data.GameData;
    }

    public GameData newGameData() {
        GameData gameData = new GameData();
        gameData.Shop = ShopDataMgr.Instance.newShop();
        gameData.Barrier = BarrierDataMgr.Instance.newBarrier();
        gameData.Player = PlayerDataMgr.Instance.newPlayer();
        gameData.Fight = FightDataMgr.Instance.newFight();
        gameData.Imprint = ImprintDataMgr.Instance.newImprint();
        return gameData;
    }

    public void deserialized() {
        RandomMgr.Instance.deserialized(this._gameData);
        ShopDataMgr.Instance.deserialized(this._gameData);
        BarrierDataMgr.Instance.deserialized(this._gameData);
        PlayerDataMgr.Instance.deserialized(this._gameData);
        FightDataMgr.Instance.deserialized(this._gameData);
        ImprintDataMgr.Instance.deserialized(this._gameData);
    }

    public void serialized() {
        RandomMgr.Instance.serialized(this._gameData);
        ShopDataMgr.Instance.serialized(this._gameData);
        BarrierDataMgr.Instance.serialized(this._gameData);
        PlayerDataMgr.Instance.serialized(this._gameData);
        FightDataMgr.Instance.serialized(this._gameData);
        ImprintDataMgr.Instance.serialized(this._gameData);
    }

    public void newGame() {
        this._gameProperty.GameData = this._gameData = this.newGameData();
        this.deserialized();
    }

    public GameState getGameState() {
        return (GameState)this._gameData.GameState;
    }

    public void setGameState(GameState state)
    {
        this._gameData.GameState = (int)state;
    }

    public PageIndex getPageIndex()
    {
        return (PageIndex)this._gameData.PageIndex;
    }

    public void setPageIndex(PageIndex pageIndex)
    {
        this._gameData.PageIndex = (int)pageIndex;
    }
}
