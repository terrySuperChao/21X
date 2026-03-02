public class GameStaticConfigMgr : Singleton<GameStaticConfigMgr>
{
    private ShopConfig _shopConfig = new ShopConfig();
    private ChapterConfig _chapterConfig = new ChapterConfig();
    private PlayerRoleConfig _playerRoleConfig = new PlayerRoleConfig();

    public void init() {
        this._shopConfig.init();
        this._chapterConfig.init();
        this._playerRoleConfig.init();
    }

    public ShopConfig getShopConfig() { 
        return this._shopConfig;
    }
    public ChapterConfig getChapterConfig()
    {
        return this._chapterConfig;
    }

    public PlayerRoleConfig getPlayerRoleConfig() { 
        return this._playerRoleConfig;
    }
}
