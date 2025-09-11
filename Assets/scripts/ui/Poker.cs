using System.Collections;
using System.Collections.Generic;
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
        // 加载名为"MyImage"的图片
        string suit = System.String.Format("{0:00}", poker.getSuit());
        string rank = poker.getRank().ToString();
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/blt_game_poker_01_{rank}_{suit}");
        if (myTexture != null)
        {
            // 例如，将加载的纹理设置给UI Image组件
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
            // 例如，将加载的纹理设置给UI Image组件
            GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
