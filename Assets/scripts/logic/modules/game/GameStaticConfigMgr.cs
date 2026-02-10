using Google.Protobuf;
using Pb;
using System.IO;
using UnityEngine;

public class GameStaticConfigMgr : Singleton<GameStaticConfigMgr>
{
    private PlayerRoleConfig _playerRoleConfig = new PlayerRoleConfig();
    public void init() {
        this._playerRoleConfig.init();
    }

    public PlayerRoleConfig getPlayerRoleConfig() { 
        return this._playerRoleConfig;
    }
}
