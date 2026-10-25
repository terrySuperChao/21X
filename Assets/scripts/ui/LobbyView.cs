using Miscalculation.CharacterLobby;
using Miscalculation.Motion.Common;
using Pb;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour, IBaseView
{
    public GameObject[] tabGameObject;
    public GameObject mainSkillName;
    public GameObject mainSkillDesc;
    public GameObject playerRoleDesc;
    public GameObject secondSkillList;
    public GameObject windowShadow;
    public GameObject expSlider;
    public GameObject expLevel;
    public GameObject startButton;

    public LobbyController controller;
    public SelectionDoodleGraphic[] chapterSelections;
    public SelectionDoodleGraphic[] difficultySelections;

    public Texture[] tabNormalTextures;//正常
    public Texture[] tabSelectTextures;//选择

    public Texture[] startNormalTextures;
    public Texture[] startHoverTextures;

    public Texture[] secondSkillTextures1; //技能1
    public Texture[] secondSkillTextures2; //技能2
    public Texture[] secondSkillTextures3; //技能3
    public Texture[] secondSkillTextures4; //技能4
    public Texture[] secondSkillTextures5; //技能5
    public Texture[] secondSkillTextures6; //技能6
    public Texture secondSkillLockTexture; //锁住

    private PlayerRole _selectRole = null;

    public void init()
    {
        this.controller.PlayEntrance();
    }

    public void beforeShow()
    {

    }

    public void refresh()
    {

    }

    public void afterShow()
    {
        this.switchTab(0);
        this.onHardClick(0);
    }

    private void switchTab(int index) {
        PlayerRoleConfig playerRoleConfig = GameStaticConfigMgr.Instance.getPlayerRoleConfig();
        List<PlayerRole> playerRoles = playerRoleConfig.getPlayerRole();
        for (int i = 0; i < this.tabGameObject.Length; i++)
        {
            PlayerRole playerRole = playerRoles[i];
            GameObject gameObject = this.tabGameObject[i].gameObject;
            Text text = gameObject.GetComponentInChildren<Text>();
            Button button = gameObject.GetComponentInChildren<Button>();
            RawImage rawImage = gameObject.GetComponentInChildren<RawImage>();
            text.text = playerRole.name;

            if (i == index) {
                button.interactable = false;
                button.GetComponent<Text>().fontSize = 36;
                button.GetComponent<RectTransform>().localPosition = new Vector3(0, 35, 0);
                button.GetComponent<HoverImage>().enabled = false;
                rawImage.texture = this.tabSelectTextures[i];
                this.updatePlayerRole(playerRole);
                this.updateDiff(playerRole.id);
                this._selectRole = playerRole;
            } else {
                button.interactable = true;
                button.GetComponent<Text>().fontSize = 25;
                button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                button.GetComponent<HoverImage>().enabled = true;
                rawImage.texture = this.tabNormalTextures[i];
            }

            RectTransform rawRect = rawImage.gameObject.GetComponent<RectTransform>();
            rawRect.sizeDelta = new Vector2(rawImage.texture.width, rawImage.texture.height);
        }

    }

    private void updatePlayerRole(PlayerRole playerRole) {
        if (playerRole.mainSkills.Count == 0)
        {
            this.mainSkillName.GetComponent<Text>().text = "";
            this.mainSkillDesc.GetComponent<Text>().text = "";
        }
        else {
            this.mainSkillName.GetComponent<Text>().text = playerRole.mainSkills[0].name;
            this.mainSkillDesc.GetComponent<Text>().text = playerRole.mainSkills[0].desc;
        }

        Texture[] textures;
        switch (playerRole.id) {
            case 1:
                textures = this.secondSkillTextures1;
                break;
            case 2:
                textures = this.secondSkillTextures2;
                break;
            case 3:
                textures = this.secondSkillTextures3;
                break;
            case 4:
                textures = this.secondSkillTextures4;
                break;
            case 5:
                textures = this.secondSkillTextures5;
                break;
            case 6:
                textures = this.secondSkillTextures6;
                break;
            default:
                textures = this.secondSkillTextures1;
                break;
        }

        IRoleStatistics roleStatistics = GameStatisticsMgr.Instance.getRoleStatistics(playerRole.id);
        for (int i = 0; i < playerRole.secondSkills.Count; i++)
        {
            Skill skill = playerRole.secondSkills[i];
            GameObject gameObject = this.secondSkillList.transform.GetChild(i).gameObject;
            gameObject.GetComponent<HoverSecondSkill>().setSecondSkill(skill);

            Text text = gameObject.GetComponentInChildren<Text>();
            RawImage rawImage = gameObject.GetComponent<RawImage>();

            if (roleStatistics.level >= skill.unLockLevel)
            {
                rawImage.texture = textures[i];
                text.text = skill.name;
            }else{
                rawImage.texture = this.secondSkillLockTexture;
                text.text = "Lv." + skill.unLockLevel + "解锁";
            }

            RectTransform rawRect = rawImage.gameObject.GetComponent<RectTransform>();
            rawRect.sizeDelta = new Vector2(rawImage.texture.width, rawImage.texture.height);
        }

        int expConfig = 0;
        if (playerRole.exp.Count > roleStatistics.level) {
            expConfig = playerRole.exp[roleStatistics.level];
        } else {
            expConfig = playerRole.exp[playerRole.exp.Count - 1];
        }

        this.playerRoleDesc.GetComponent<Text>().text = playerRole.desc;
        this.expLevel.GetComponent<Text>().text = "Lv." + roleStatistics.level;
        this.expSlider.GetComponent<Slider>().value = roleStatistics.exp / expConfig;
    }

    private void updateDiff(int playerRoleId) {
        GameProperty gameProperty = GamePropertyMgr.Instance.getGameProperty();

        int index = 0;
        for (int i = 0; i < gameProperty.GameData.DefeatRoles.Count; i++) {
            if (gameProperty.GameData.DefeatRoles[i].Id == playerRoleId) {
                index = i;
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onReturnClick() {
        UIMgr.Instance.showView("EntryView");
    }

    public void onStartGameClick() {
        if (GameDataMgr.Instance.getGameState() == GameState.playing)
        {
            UIMgr.Instance.showAlert("AlertView", "开始新的一局游戏，已保存的内容将被清除，是否确认继续?",
            () =>
            {
                this.gotoBarrierView();
            },
            () =>
            {

            });
        }
        else {
            this.gotoBarrierView();
        }
    }

    public void onTabClick(int parameter) {
        this.switchTab(parameter);
    }

    private void gotoBarrierView() {
        GameReqMgr.Instance.requestNewGame(this._selectRole.id);
        UIMgr.Instance.showView("BarrierView");
    }

    public void onSwitchLight() {
        bool lampOn = this.controller.Current.lampOn;
        this.windowShadow.SetActive(lampOn);
        this.controller.SetLamp(!lampOn);
    }

    public void onHardClick(int index) {
        this.selectOne(this.difficultySelections, index);
        this.selectHover(index);
    }

    void selectOne(SelectionDoodleGraphic[] values, int index)
    {
        if (values == null) return;
        for (int i = 0; i < values.Length; i++)
            if (values[i])
            {
                if (i == index)
                    values[i].Play();
                else
                    values[i].Hide();
            }
    }

    void selectHover(int index) {
        if (index < 0 || index >= this.startNormalTextures.Length || index >= this.startHoverTextures.Length) {
            return;
        }
        Texture texture = this.startNormalTextures[index];
        this.startButton.GetComponent<RawImage>().texture = texture;
        RectTransform rawRect = this.startButton.GetComponent<RectTransform>();
        rawRect.sizeDelta = new Vector2(texture.width, texture.height);
        HoverImage hoverImage = this.startButton.GetComponent<HoverImage>();
        hoverImage.updateHover(this.startHoverTextures[index]);
    }
}
