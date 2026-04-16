using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public GameObject money;
    public GameObject diamond;
    public GameObject hp;
    public GameObject magic;
    public GameObject attack;
    public GameObject defense;
      
    private void showPlayerInfo() {
         this.money.GetComponent<Text>().text = PlayerDataMgr.Instance.getMoney().ToString();
         this.diamond.GetComponent<Text>().text = PlayerDataMgr.Instance.getDiamond().ToString();
         this.hp.GetComponent<Text>().text = PlayerDataMgr.Instance.getHP() + "/" + PlayerDataMgr.Instance.getMaxHP();
         this.magic.GetComponent<Text>().text = PlayerDataMgr.Instance.getMagic() + "/" + PlayerDataMgr.Instance.getMaxMagic();
         this.attack.GetComponent<Text>().text = "0";
         this.defense.GetComponent<Text>().text = "0";
    }

    // Start is called before the first frame update
    void Start()
    {
        this.showPlayerInfo();
        EventDispatcher.Instance.on(GameConst.UPDATE_PLAYER_INFO, this.updatePlayerInfoHandle);
    }

    private void OnDestroy(){
        EventDispatcher.Instance.off(GameConst.UPDATE_PLAYER_INFO, this.updatePlayerInfoHandle);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void onPopClick()
    {
        UIMgr.Instance.showView("PopView");
    }

    public void updatePlayerInfoHandle(params System.Object[] obj)
    {
        this.showPlayerInfo();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
