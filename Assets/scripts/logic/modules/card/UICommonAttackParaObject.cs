public class UICommonAttackParaObject:IUICommonAttackPara
{
    private IUser _attackUser;
    private float _attackValue;
    private float _bloodValue;
    private float _defenseValue;
    private float _finalBloodValue;
    private float _finalDefenseValue;
    public UICommonAttackParaObject(IUser attackUser,float attackValue,float bloodValue,float finalBloodValue,float defenseValue,float finalDefenseValue) {
        _attackUser = attackUser;
        _attackValue = attackValue;
        _bloodValue = bloodValue;
        _finalBloodValue = finalBloodValue;
        _defenseValue = defenseValue;
        _finalDefenseValue = finalDefenseValue;
    }
    public IUser getAttackUser() {
        return _attackUser;
    }

    public float getAttack()
    {
        return _attackValue;
    }

    public float getBlood() {
        return _bloodValue;
    }

    public float getDefense() {
        return _defenseValue;
    }

    public float getFinalBlood() {
        return _finalBloodValue;
    }

    public float getFinalDefense() {
        return _finalDefenseValue;
    }
}
