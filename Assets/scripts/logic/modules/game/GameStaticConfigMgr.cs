public class GameStaticConfigMgr : Singleton<GameStaticConfigMgr>
{
    private ShopConfig _shopConfig = new ShopConfig();
    private ChapterConfig _chapterConfig = new ChapterConfig();
    private PlayerRoleConfig _playerRoleConfig = new PlayerRoleConfig();
    private BaseEffectConfig _baseEffectConfig = new BaseEffectConfig();
    private TriggerConfig _triggerConfig = new TriggerConfig();
    private AdvancedEffectConfig _advancedEffectConfig = new AdvancedEffectConfig();

    public void init() {
        this._shopConfig.init();
        this._chapterConfig.init();
        this._playerRoleConfig.init();
        this._triggerConfig.init();
        this._baseEffectConfig.init();
        this._advancedEffectConfig.init();
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
    public BaseEffectConfig getBaseEffectConfig() { 
        return this._baseEffectConfig;
    }

    public TriggerConfig getTriggerConfig(){
        return this._triggerConfig;
    }

    public AdvancedEffectConfig getAdvancedEffectConfig(){
        return this._advancedEffectConfig;
    }
}
