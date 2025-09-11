using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Root : MonoBehaviour
{

    public GameObject lobbyView;
    public GameObject gameView;
    public GameObject fightView;
    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on("returnToLobby", this.onLobby);
        EventDispatcher.Instance.on("startGame", this.onStart);
        InvokeRepeating("repeatHandleMessage", 0.0f, 0.1f);
        onLoadLobbyView();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy(){
        EventDispatcher.Instance.off("startGame", this.onStart);
        EventDispatcher.Instance.off("returnToLobby", this.onLobby);
    }

    private void onLobby(params System.Object[] obj) {
        deleteGameObject("GameView");
        onLoadLobbyView();
    }

    private void onStart(params System.Object[] obj) {
        //每局开始处理
        IUser user1 = UserMgr.Instance.getUser();
        IUser user2 = NpcMgr.Instance.getUser();
        PokerPileMgr.Instance.shuffle();
        HandPokerMgr.Instance.resetHandPoker();
        GameCtrl.Instance.clearCacheMessage();
        PlayPokerMgr.Instance.clearPlayer();
        PlayPokerMgr.Instance.addPlayer(user1);
        PlayPokerMgr.Instance.addPlayer(user2);
        PlayPokerMgr.Instance.startPlayPoker();

        deleteGameObject("LobbyView");
        onLoadGameView(obj);
    }


    private void onLoadLobbyView() {
        GameObject lvGameObject = Instantiate(lobbyView, new Vector3(0, 0, 0), Quaternion.identity);
        lvGameObject.transform.parent = gameObject.transform;
        lvGameObject.name = "LobbyView";
    }

    private void onLoadGameView(params System.Object[] obj) {
        GameMode mode = (GameMode)obj[0];
        GameObject gView = mode == GameMode.Common ? gameView : fightView;
        GameObject gvGameObject = Instantiate(gView, new Vector3(0, 0, 0), Quaternion.identity);
        gvGameObject.transform.parent = gameObject.transform;
        gvGameObject.name = "GameView";
    }

    private void deleteGameObject(string name) {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (child.gameObject.name == name)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
                break;
            }
        }
    }

    private void repeatHandleMessage()
    {
        GameCtrl.Instance.handleMessage();
    }
}
