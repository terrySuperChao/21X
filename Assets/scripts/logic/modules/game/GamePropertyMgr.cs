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
            fileStream.Close();
        }
        if (this._gameProperty == null) {
            this._gameProperty = new GameProperty();
        }
        if (this._gameProperty.GameData == null) {
            this._gameProperty.GameData = GameDataMgr.Instance.newGameData();
        }
        if (this._gameProperty.Account == null) {
            this._gameProperty.Account = new Account();
        }
        if (this._gameProperty.Setting == null) {
            this._gameProperty.Setting = SettingDataMgr.Instance.newSetting();
        }
        this.deserialized();
    }

    public void save() {
        this.serialized();
    }

    public GameProperty getGameProperty()
    {
        return _gameProperty;
    }

    private void deserialized() {
        GameDataMgr.Instance.init(this._gameProperty);
        GameDataMgr.Instance.deserialized();

        SettingDataMgr.Instance.init(this._gameProperty);
        SettingDataMgr.Instance.deserialized();

        LangMgr.Instance.setCurLanguage(SettingDataMgr.Instance.getLanguage());
    }

    private void serialized() {
        GameDataMgr.Instance.serialized();
        SettingDataMgr.Instance.serialized();
        //±£´æ
        ProtobufMgr.Instance.serializeToFile(_fileName, this._gameProperty.ToByteArray());
    }
}
