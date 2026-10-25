using UnityEngine;

public class Poker : MonoBehaviour
{
    //正面
    public GameObject foreNode;
    public GameObject logo;
    public GameObject career;
    public GameObject selected;
    public GameObject rank;
    public GameObject suit;

    //背面
    public GameObject backNode;
    public GameObject background;

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

        //21x_card_num_c_2
        //21x_card_num_d_2
        //21x_card_num_h_2
        //21x_card_num_s_2
        string resName = "";
        string suitName = "";
        string rankName = "";
        switch (poker.getSuit()) {
            case PokerSuit.diamond:
                resName = "21x_card_num_c";
                suitName = "21x_card_suit_c_01";
                break;
            case PokerSuit.heart: 
                resName = "21x_card_num_d";
                suitName = "21x_card_suit_c_02";
                break;
            case PokerSuit.spade: 
                resName = "21x_card_num_h";
                suitName = "21x_card_suit_c_03";
                break;
            case PokerSuit.club: 
                resName = "21x_card_num_s";
                suitName = "21x_card_suit_c_04";
                break;
        }

        Texture2D logoTexture = Resources.Load<Texture2D>($"UI/pokers/{resName}_01");
        if (logoTexture != null)
        {
            this.logo.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(logoTexture, new Rect(0.0f, 0.0f, logoTexture.width, logoTexture.height), new Vector2(0.5f, 0.5f));
        }

        Texture2D suitTexture = Resources.Load<Texture2D>($"UI/pokers/{suitName}");
        if (suitTexture != null)
        {
            this.suit.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(suitTexture, new Rect(0.0f, 0.0f, suitTexture.width, suitTexture.height), new Vector2(0.5f, 0.5f));
        }

        switch (poker.getRank()) {
            case 11:
                rankName = "_j";
                break;
            case 12:
                rankName = "_q";
                break;
            case 13:
                rankName = "_k";
                break;
            case 14:
                rankName = "_a";
                break;
            default:
                rankName = "_"+poker.getRank();
                break;
        }

        Texture2D rankTexture = Resources.Load<Texture2D>($"UI/pokers/{resName+ rankName}");
        if (rankTexture != null){
            this.rank.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(rankTexture, new Rect(0.0f, 0.0f, rankTexture.width, rankTexture.height), new Vector2(0.5f, 0.5f));
        }
        this.backNode.SetActive(false);
        this.foreNode.SetActive(true);

        /*
        string suit = System.String.Format("{0:00}", (int)poker.getSuit()); 
        string rank = poker.getRank().ToString();
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/blt_game_poker_01_{rank}_{suit}");
        if (myTexture != null)
        {
            // 
            GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }*/
    }

    private void flipOver(params System.Object[] obj) {
        if (_poker != null && _poker.isBack()) {
            _poker.setBack(false);
            this.loadPokerRes(_poker);
        }
    }

    public void loadBackPoker() {
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/21x_card_back_01");
        if (myTexture != null){
            this.background.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
        this.backNode.SetActive(true);
        this.foreNode.SetActive(false);
    }

    public IPoker getPoker() {
        return _poker;
    }
}
