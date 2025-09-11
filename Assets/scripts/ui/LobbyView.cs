using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onStartClick() {
        EventDispatcher.Instance.emit("startGame", GameMode.Common);
    }

    public void onFightClick() {
        EventDispatcher.Instance.emit("startGame", GameMode.Fight);
    }

    public void onCloseClick() { 
        
    }
}
