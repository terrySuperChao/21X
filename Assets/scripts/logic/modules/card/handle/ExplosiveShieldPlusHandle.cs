//±¬ÅÆÖ®¶Ü+
public class ExplosiveShieldPlusHandle : ExplosiveShieldHandle
{
    override
    protected int getNumber()
    {
        return RandomMgr.Instance.getRangeInt(0, 2) == 0 ? 20 : 10;
    }
}
