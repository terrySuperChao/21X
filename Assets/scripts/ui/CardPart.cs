using UnityEngine;
using UnityEngine.UI;

public class CardPart : MonoBehaviour
{
    public Text partName;
    public GameObject partImage;
    private IPart _partInfo;
    private ICard _card;

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
        this.partName.text = partInfo.getName();
        this._partInfo = partInfo;
        Texture2D myTexture = Resources.Load<Texture2D>(partInfo.getImage());
        if (myTexture != null)
        {
            // ���磬�����ص��������ø�UI Image���
            this.partImage.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public IPart getPartInfo() { 
        return this._partInfo;
    }

    public void setCard(ICard card) { 
        this._card = card;
    }

    public ICard getCard() { 
        return this._card;
    }
}   
