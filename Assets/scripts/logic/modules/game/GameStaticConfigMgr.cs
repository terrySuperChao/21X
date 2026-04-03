public class GameStaticConfigMgr : Singleton<GameStaticConfigMgr>
{
    private ShopConfig _shopConfig = new ShopConfig();
    private ChapterConfig _chapterConfig = new ChapterConfig();
    private PlayerRoleConfig _playerRoleConfig = new PlayerRoleConfig();
    private BasePartConfig _basePartConfig = new BasePartConfig();
    private TriggerPartConfig _triggerPartConfig = new TriggerPartConfig();
    private CardPartConfig _cardPartConfig = new CardPartConfig();

    public void init() {
        this._shopConfig.init();
        this._chapterConfig.init();
        this._playerRoleConfig.init();
        this._basePartConfig.init();
        this._triggerPartConfig.init();
        this._cardPartConfig.init();
    }

    public ShopConfig getShopConfig() { 
        return this._shopConfig;
    }
    public ChapterConfig getChapterConfig(){
        return this._chapterConfig;
    }

    public PlayerRoleConfig getPlayerRoleConfig() { 
        return this._playerRoleConfig;
    }
    public BasePartConfig getBasePartConfig() { 
        return this._basePartConfig;
    }

    public TriggerPartConfig getTriggerPartConfig(){
        return this._triggerPartConfig;
    }

    public CardPartConfig getCardPartConfig(){
        return this._cardPartConfig;
    }
}
