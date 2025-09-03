using System;

public class PokerObject : IPoker
{
    private int _id = 0;
    private int _value = 0;
    private bool _isBack = false;
    public int getId() {
        return _id;
    }
    public void setId(int id) {
        _id = id;
    }
    public int getValue(){
        return _value;
    }
    public void setValue(int value) {
        _value = value;
    }

    public int getRank() {
        return _value % 100;
    }

    public int getSuit() { 
        return (_value - _value % 100)/100;
    }

    public bool isBack() {
        return _isBack;
    }

    public void setBack(bool isBack) {
        _isBack = isBack;
    }
}
