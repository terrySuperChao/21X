using System.Collections.Generic;
public interface IUIPokerPara
{
    public IUser getUser();
    public IPoker getPoker();
    public List<IPoker> getPokers();
    public float getValue();
    public float getFinalValue();
    public float getMult();
}
