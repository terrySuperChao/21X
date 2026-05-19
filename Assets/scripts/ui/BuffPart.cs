using UnityEngine;
using UnityEngine.UI;

public class BuffPart : MonoBehaviour
{
    public Text partName;
    public GameObject partImage;
    private IUser _user;
    private BuffType _buffType;
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

    public void setBuffType(BuffType buffType) {
        this._buffType = buffType;
    }

    public BuffType getBuffType() {
        return this._buffType;
    }

    public void setUser(IUser user) {
        this._user = user;
    }

    public IUser getUser() {
        return this._user;
    }
}   
