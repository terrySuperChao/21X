using System;
//
public class GameUtils
{
    public static ValueType SuitTransformValueType(PokerSuit suit)
    {
        if (suit == PokerSuit.club)
        {
            return ValueType.magic;
        }
        else if (suit == PokerSuit.diamond)
        {
            return ValueType.defense;
        }
        else if (suit == PokerSuit.spade)
        {
            return ValueType.attack;
        }
        else if (suit == PokerSuit.heart)
        {
            return ValueType.blood;
        }
        else
        {
            return ValueType.nil;
        }
    }

    public static string formatDescription(string description, float value)
    {
        string desc = description.Replace(" ", "");
        int index1 = desc.IndexOf("%s");
        if (index1 > -1)
        {
            int index2 = desc.IndexOf("%s%");
            if (index2 > -1)
            {
                value *= 100;
            }
            return desc.Replace("%s", value.ToString());
        }
        else
        {
            return desc;
        }
    }

    //保留一位小数，四舍五入
    public static float getNumberDigits(float number)
    {
        return number;
        //return (float)Math.Round((number * 10 + 0.5) / 10, 1);
    }
}