using Pb;
using System.Collections.Generic;

public class IRoleStatistics{
    public int roleId;
    public int level;
    public int exp;
}

public class GameStatisticsMgr : Singleton<GameStatisticsMgr>
{
    private GameProperty _gameProperty;
    private List<IRoleStatistics> _roleStatistics = new List<IRoleStatistics>();
    
    public void init(GameProperty data) {
        this._gameProperty = data;
    }

    public void deserialized() {
        this._roleStatistics.Clear();
        for (int i = 0; i < this._gameProperty.GameStatistics.RoleStatistics.Count; i++) {
            IRoleStatistics roleStatistics = new IRoleStatistics();
            roleStatistics.roleId = this._gameProperty.GameStatistics.RoleStatistics[i].RoleId;
            roleStatistics.level = this._gameProperty.GameStatistics.RoleStatistics[i].Level;
            roleStatistics.exp = this._gameProperty.GameStatistics.RoleStatistics[i].Exp;
            this._roleStatistics.Add(roleStatistics);
        }
    }

    public void serialized() {
        this._gameProperty.GameStatistics.RoleStatistics.Clear();
        for (int i = 0; i < this._roleStatistics.Count; i++) {
            RoleStatistics roleStatistics = new RoleStatistics();
            roleStatistics.RoleId = this._roleStatistics[i].roleId;
            roleStatistics.Level = this._roleStatistics[i].level;
            roleStatistics.Exp = this._roleStatistics[i].exp;            
            this._gameProperty.GameStatistics.RoleStatistics.Add(roleStatistics);
        }
    }

    public GameStatistics newGameStatistics() {
        GameStatistics gameStatistics = new GameStatistics();
        for (int i = 1; i <= 6; i++)
        {
            RoleStatistics roleStatistics = new RoleStatistics();
            roleStatistics.RoleId = i;
            roleStatistics.Level = 1;
            roleStatistics.Exp = 0;
            gameStatistics.RoleStatistics.Add(roleStatistics);
        }
        return gameStatistics;
    }

    public IRoleStatistics getRoleStatistics(int roleId) {
        for (int i = 0; i < this._roleStatistics.Count; i++) {
            if (this._roleStatistics[i].roleId == roleId) {
                return this._roleStatistics[i];
            }
        }

        IRoleStatistics roleStatistics = new IRoleStatistics();
        roleStatistics.roleId = roleId;
        roleStatistics.exp = 0;
        roleStatistics.level = 0;
        return roleStatistics;
    }
}
