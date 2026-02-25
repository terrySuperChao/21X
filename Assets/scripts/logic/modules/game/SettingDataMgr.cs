using Google.Protobuf.WellKnownTypes;
using Pb;
using UnityEngine;
public class SettingDataMgr : Singleton<SettingDataMgr>
{
    private GameProperty _gameProperty;
    private float _mainVolume = 1.0f;
    private float _musicVolume = 1.0f;
    private float _soundVolume = 1.0f;
    private string _language = LanguageList.zh_CN;
    private int _appearance = 0;
    private int _resolution = 0;

    private float _mainVolume1 = 1.0f;
    private float _musicVolume1 = 1.0f;
    private float _soundVolume1 = 1.0f;
    private string _language1 = LanguageList.zh_CN;
    private int _appearance1 = 0;
    private int _resolution1 = 0;

    public void init(GameProperty data) {
        this._gameProperty = data;

    }

    public void deserialized() {
        _mainVolume1 = _gameProperty.Setting.MainVolume;
        _musicVolume1 = _gameProperty.Setting.MusicVolume;
        _soundVolume1 = _gameProperty.Setting.SoundVolume;
        _language1 = _gameProperty.Setting.Language;
        _appearance1 = _gameProperty.Setting.Appearance;
        _resolution1 = _gameProperty.Setting.Resolution;
    }

    public void serialized() {

    }

    public Setting newSetting() { 
        Setting setting = new Setting();
        setting.MainVolume = _mainVolume;
        setting.MusicVolume = _musicVolume;
        setting.SoundVolume = _soundVolume;
        setting.Language = _language;
        setting.Appearance = _appearance;
        setting.Resolution = _resolution;
        return setting;
    }

    public float getMainVolume() {  return _gameProperty.Setting.MainVolume; }
    public void setMainVolume(float value) { _mainVolume1 = value; }

    public float getMusicVolume() { return _gameProperty.Setting.MusicVolume; }
    public void setMusicVolume(float value) { _musicVolume1 = value; }

    public float getSoundVolume() { return _gameProperty.Setting.SoundVolume; }
    public void setSoundVolume(float value) { _soundVolume1 = value; }


    public string getLanguage() { return _gameProperty.Setting.Language; }
    public void setLanguage(string value) { _language1 = value; }


    public int getAppearance() { return _gameProperty.Setting.Appearance; }
    public void setAppearance(int value) { _appearance1 = value; }


    public int getResolution() { return _gameProperty.Setting.Resolution; }
    public void setResolution(int value) { _resolution1 = value; }

    public void saveSetting() {
        _gameProperty.Setting.MainVolume = _mainVolume1;
        _gameProperty.Setting.MusicVolume = _musicVolume1;
        _gameProperty.Setting.SoundVolume = _soundVolume1;
        _gameProperty.Setting.Language = _language1;
        _gameProperty.Setting.Appearance = _appearance1;
        _gameProperty.Setting.Resolution = _resolution1;
    }

    public void resetSetting() {
        _gameProperty.Setting.MainVolume = _mainVolume1  = _mainVolume;
        _gameProperty.Setting.MusicVolume = _musicVolume1  = _musicVolume;
        _gameProperty.Setting.SoundVolume = _soundVolume1  = _soundVolume;
        _gameProperty.Setting.Language = _language1 = _language;
        _gameProperty.Setting.Appearance = _appearance1 = _appearance;
        _gameProperty.Setting.Resolution = _resolution1 = _resolution;
    }

    public bool isSaveSetting()
    {
        return _gameProperty.Setting.MainVolume != _mainVolume1 ||
               _gameProperty.Setting.MusicVolume != _musicVolume1 ||
               _gameProperty.Setting.SoundVolume != _soundVolume1 ||
               _gameProperty.Setting.Language != _language1 ||
               _gameProperty.Setting.Appearance != _appearance1 ||
               _gameProperty.Setting.Resolution != _resolution1;
    }
}
