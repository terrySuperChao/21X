using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Unity.VisualScripting;
using System.Runtime.ConstrainedExecution;
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

    public static string formatDescription(string description, List<float> values)
    {
        string desc = description.Replace(" ", "");
        string[] list = desc.Split("%s");
        if (list.Length <= 1)
        {
            return desc;
        }
        
        string str = list[0];
        for (int i = 1; i < list.Length; i++) {
            
            if (list[i] != "" &&list[i].Substring(0, 1) == "%")
            {
                str += "<color=red>" + (values[i - 1] * 100) + "%</color>" + list[i].Substring(1, list[i].Length - 1);
            }
            else {
                str += "<color=red>" + (values[i - 1]) + "</color>" + list[i];
            }   
        }
        
        return str;
    }

    public static string formatDescription(string description, float value)
    {
        string desc = description.Replace(" ", "");
        int index1 = desc.IndexOf("%s");
        if (index1 > -1)
        {
            string oldStr = "";
            string newStr = "";
            int index2 = desc.IndexOf("%s%");
            if (index2 > -1)
            {
                value *= 100;
                oldStr = "%s%";
                newStr = value + "%";
            }
            else
            {
                oldStr = "%s";
                newStr = value + "";
            }
            return desc.Replace(oldStr, "<color=red>" + newStr + "</color>");
        }
        else {
            return desc;
        }
    }

    //保留一位小数，四舍五入
    public static float getNumberDigits(float number)
    {
        return (float)Math.Round((number * 10 + 0.5) / 10, 1);
    }

    //对比逻辑中的数字
    public static bool compareNumber(string compareStr, float currentNum)
    {
        string targetStr = extractNumbersWithDecimal(compareStr);
        int index = compareStr.IndexOf(targetStr);
        if (index == -1) return false;

        bool success = false;
        float targetNum = float.Parse(targetStr);
        string symbol = compareStr.Substring(0, index).Trim();
        UnityEngine.Debug.Log(string.Format("targetNum={0},currentNum={1}", targetNum, currentNum));
        switch (symbol)
        {
            case ">":
                if (currentNum > targetNum)
                {
                    success = true;
                }
                break;
            case ">=":
                if (currentNum >= targetNum)
                {
                    success = true;
                }
                break;
            case "=":
                if (currentNum == targetNum)
                {
                    success = true;
                }
                break;
            case "<=":
                if (currentNum <= targetNum)
                {
                    success = true;
                }
                break;
            case "<":
                if (currentNum < targetNum)
                {
                    success = true;
                }
                break;
            default:
                if (currentNum >= targetNum)
                {
                    success = true;
                }
                break;
        }
        return success;
    }

    //正则表达式（保留小数点）
    public static string extractNumbersWithDecimal(string input)
    {
        return Regex.Replace(input, @"[^\d.]", "");
    }
}