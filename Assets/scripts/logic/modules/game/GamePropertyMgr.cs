using Google.Protobuf;
using Pb;
using System.IO;
using UnityEngine;

public class GamePropertyMgr : Singleton<GamePropertyMgr>
{
    private GameProperty _gameProperty;
    private string _fileName = Application.persistentDataPath + "/gameProperty.txt";
    public void init() {
        FileStream fileStream = ProtobufMgr.Instance.deserializeFromFile(_fileName);
        if (fileStream != null) {
            this._gameProperty = GameProperty.Parser.ParseFrom(fileStream);
        }
        if (this._gameProperty == null) {
            this._gameProperty = new GameProperty();
            this._gameProperty.GameData =  new GameData();
            this._gameProperty.Account = new Account();
            this._gameProperty.Setting = new Setting();
        }
    }

    public void save() {
        ProtobufMgr.Instance.serializeToFile(_fileName,this._gameProperty.ToByteArray());
    }

    public GameProperty getGameProperty()
    {
        return _gameProperty;
    }

    public GameData getGameData()
    {
        return _gameProperty.GameData;
    }
}
