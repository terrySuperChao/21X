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
            this._gameProperty.GameData = new GameData();
        }
        if (this._gameProperty.GameData.Barrier == null) {
            this._gameProperty.GameData.Barrier = new Barrier();
        }
        if (this._gameProperty.Account == null) {
            this._gameProperty.Account = new Account();
        }
        if (this._gameProperty.Setting == null) {
            this._gameProperty.Setting = new Setting();
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
        GameDataMgr.Instance.init(this._gameProperty.GameData);
        GameDataMgr.Instance.deserialized();
    }

    private void serialized() {
        GameDataMgr.Instance.serialized();



        //±£´æ
        ProtobufMgr.Instance.serializeToFile(_fileName, this._gameProperty.ToByteArray());
    }
}
