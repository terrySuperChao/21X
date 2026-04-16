using UnityEngine;
using UnityEngine.UI;

public class CardPart : MonoBehaviour
{
    public Text partName;
    public GameObject partImage;
    private IPart _partInfo;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void loadPartImage(IPart partInfo) {
        Texture2D myTexture = Resources.Load<Texture2D>($"UI/pokers/blt_game_poker_01_2_01");
        if (myTexture != null)
        {
            // 例如，将加载的纹理设置给UI Image组件
            this.partImage.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }

        this.partName.text = partInfo.getName();
        this._partInfo = partInfo;
    }

    public IPart getPartInfo() { 
        return this._partInfo;
    }
}   
