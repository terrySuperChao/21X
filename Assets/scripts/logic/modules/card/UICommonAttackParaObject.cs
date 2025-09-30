public class UICommonAttackParaObject:IUICommonAttackPara
{
    private IUser _attackUser;
    private IUser _defenseUser;
    private float _attackValue;
    private float _bloodValue;
    private float _defenseValue;
    private float _finalBloodValue;
    private float _finalDefenseValue;
    private bool _isMagicAttack;
    public UICommonAttackParaObject(IUser attackUser, IUser defenseUser, float attackValue,float bloodValue,float finalBloodValue,float defenseValue,float finalDefenseValue,bool isMagicAttack) {
        _attackUser = attackUser;
        _defenseUser = defenseUser;
        _attackValue = attackValue;
        _bloodValue = bloodValue;
        _finalBloodValue = finalBloodValue;
        _defenseValue = defenseValue;
        _finalDefenseValue = finalDefenseValue;
        _isMagicAttack = isMagicAttack;
    }
    public IUser getAttackUser() {
        return _attackUser;
    }

    public IUser getDefenseUser() {
        return _defenseUser;
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

    public bool isMagicAttack() {
        return _isMagicAttack;
    }
}
