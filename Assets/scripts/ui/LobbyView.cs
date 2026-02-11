using Pb;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour,IBaseView
{
    public GameObject tabContainer;
    public GameObject mainSkillName;
    public GameObject mainSkillDesc;
    public GameObject secondSkillContainer;
    public GameObject diffContainer;
    public GameObject playerRoleDesc;
    public GameObject secondSkillPop;
    public GameObject secondSkillPopDesc;
    public Sprite tabClickSprite;
    public Sprite tabNormalSprite;

    private PlayerRole _selectPlayerRole = null;

    public void init()
    {
        
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
        int childCount = this.tabContainer.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject gameObject = this.tabContainer.transform.GetChild(i).gameObject;
            if (i >= playerRoles.Count)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                gameObject.GetComponentInChildren<Text>().text = playerRoles[i].name;
                if (i == index)
                {
                    gameObject.GetComponent<Button>().interactable = false;
                    gameObject.GetComponent<Image>().sprite = this.tabClickSprite;
                    this.updatePlayerRole(playerRoles[i]);
                    this.updateDiff(playerRoles[i].id);
                }
                else
                {
                    gameObject.GetComponent<Button>().interactable = true;
                    gameObject.GetComponent<Image>().sprite = this.tabNormalSprite;
                }
            }
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
        this._selectPlayerRole = playerRole;
    }

    private void updateDiff(int playerRoleId) {
        GameProperty gameProperty = GamePropertyMgr.Instance.getGameProperty();

        int index = 0;
        for (int i = 0; i < gameProperty.GameData.DefeatPlayerRoles.Count; i++){
            if (gameProperty.GameData.DefeatPlayerRoles[i].Id == playerRoleId) {
                index = i;
                break;
            }
        }

        int childCount = this.diffContainer.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject gameObject = this.diffContainer.transform.GetChild(i).gameObject;
            if (i > index)
            {
                
            }
            else
            {
                
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
        if (GamePropertyMgr.Instance.getGameData().GameState == (int)GameState.playing)
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
        if (parameter < this._selectPlayerRole.secondSkills.Count) {
            Skill skill = this._selectPlayerRole.secondSkills[parameter];
            this.secondSkillPopDesc.GetComponent<Text>().text = skill.name + "\n" + skill.desc;

            Transform obj1Transform = this.secondSkillContainer.transform.GetChild(parameter);
            Transform obj2Transform = this.secondSkillPop.transform.parent.transform;

            Vector3 vector = this.secondSkillPop.transform.position;
            vector.x = obj1Transform.transform.position.x;
            this.secondSkillPop.transform.position = vector;
        }
    }

    private void gotoBarrierView() {
        GamePropertyMgr.Instance.getGameData().GameState = (int)GameState.playing;
        GamePropertyMgr.Instance.getGameData().PageIndex = (int)PageIndex.BarrierView;
        GamePropertyMgr.Instance.save();
        UIMgr.Instance.showView("BarrierView");
    }
}
