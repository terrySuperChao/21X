public class GameStaticConfigMgr : Singleton<GameStaticConfigMgr>
{
    private PlayerRoleConfig _playerRoleConfig = new PlayerRoleConfig();
    private ChapterConfig _chapterConfig = new ChapterConfig();
    public void init() {
        this._playerRoleConfig.init();
        this._chapterConfig.init();
    }

    public PlayerRoleConfig getPlayerRoleConfig() { 
        return this._playerRoleConfig;
    }

    public ChapterConfig getChapterConfig()
    {
        return this._chapterConfig;
    }
}
