using Miscalculation.CharacterLobby;
using Miscalculation.Motion.Common;
using Pb;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour, IBaseView
{
    public GameObject[] tabGameObject;
    public GameObject mainSkillName;
    public GameObject mainSkillDesc;
    public GameObject secondSkillContainer;
    public GameObject playerRoleDesc;
    public GameObject secondSkillPop;
    public GameObject secondSkillPopDesc;
    public GameObject windowShadow;
    public LobbyController controller;
    public SelectionDoodleGraphic[] chapterSelections;
    public SelectionDoodleGraphic[] difficultySelections;

    public Texture[] tabNormalTextures;//正常
    public Texture[] tabSelectTextures;//选择
    public Texture[] tabFloatTextures;//悬浮

   
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
    }

    private void switchTab(int index) {
        PlayerRoleConfig playerRoleConfig = GameStaticConfigMgr.Instance.getPlayerRoleConfig();
        List<PlayerRole> playerRoles = playerRoleConfig.getPlayerRole();
        for (int i = 0; i < this.tabGameObject.Length; i++)
        {
            GameObject gameObject = this.tabGameObject[i].gameObject;
            Text text = gameObject.GetComponentInChildren<Text>();
            Button button = gameObject.GetComponentInChildren<Button>();
            RawImage rawImage = gameObject.GetComponentInChildren<RawImage>();
            text.text = playerRoles[i].name;
            //button.interactable = (i != index);
            
            if (i == index){
                rawImage.texture = this.tabSelectTextures[i];
                this.updatePlayerRole(playerRoles[i]);
                this.updateDiff(playerRoles[i].id);
            }else{
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

        int childCount = this.secondSkillContainer.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject gameObject = this.secondSkillContainer.transform.GetChild(i).gameObject;
            if (i >= playerRole.secondSkills.Count)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                gameObject.GetComponentInChildren<Text>().text = playerRole.secondSkills[i].name;
            }
        }

        this.playerRoleDesc.GetComponent<Text>().text = playerRole.desc;
        this._selectRole = playerRole;
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

    public void onSecondSkillClick(int parameter) {
        this.secondSkillPop.SetActive(true);
        if (parameter < this._selectRole.secondSkills.Count) {
            Skill skill = this._selectRole.secondSkills[parameter];
            this.secondSkillPopDesc.GetComponent<Text>().text = skill.name + "\n" + skill.desc;

            Transform obj1Transform = this.secondSkillContainer.transform.GetChild(parameter);
            Transform obj2Transform = this.secondSkillPop.transform.parent.transform;

            Vector3 vector = this.secondSkillPop.transform.position;
            vector.x = obj1Transform.transform.position.x;
            this.secondSkillPop.transform.position = vector;
        }
    }

    private void gotoBarrierView() {
        GameReqMgr.Instance.requestNewGame(this._selectRole.id);
        UIMgr.Instance.showView("BarrierView");
    }

    public void onSwitchLight(){
        bool lampOn = this.controller.Current.lampOn;
        this.windowShadow.SetActive(lampOn);
        this.controller.SetLamp(!lampOn);
    }

    public void onHardClick(int index) {
        this.SelectOne(difficultySelections, index);
    }

    void SelectOne(SelectionDoodleGraphic[] values, int index)
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
}
