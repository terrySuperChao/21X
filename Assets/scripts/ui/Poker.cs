using UnityEngine;

public class Poker : MonoBehaviour
{
    private IPoker _poker;
    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.FLIPPOKER, this.flipOver);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.FLIPPOKER, this.flipOver);
    }

    public void loadPokerRes(IPoker poker) {
        _poker = poker;
        if (poker == null || poker.isBack()) return;
        //
        string suit = System.String.Format("{0:00}", (int)poker.getSuit()); 
        string rank = poker.getRank().ToString();
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/blt_game_poker_01_{rank}_{suit}");
        if (myTexture != null)
        {
            // 
            GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    private void flipOver(params System.Object[] obj) {
        if (_poker != null && _poker.isBack()) {
            _poker.setBack(false);
            this.loadPokerRes(_poker);
        }
    }

    public void loadBackPoker() {
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/blt_game_poker_00");
        if (myTexture != null)
        {
            // ���磬�����ص��������ø�UI Image���
            GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public IPoker getPoker() {
        return _poker;
    }
}
