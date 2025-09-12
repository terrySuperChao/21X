using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PokerPileMgr.Instance.init();
        HandPokerMgr.Instance.init();
    }
}
