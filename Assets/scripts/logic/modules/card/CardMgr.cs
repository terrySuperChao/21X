using System;
using System.Collections.Generic;
using UnityEngine;

public class CardMgr: Singleton<CardMgr>
{
    private const int MAXSLOT = 3;
    private int _round = 1;
    private Dictionary<string, List<ICard>> _cardDic = new Dictionary<string, List<ICard>>();
    public void init()
    {
        _round = 1;
        _cardDic.Clear();
    }

    public void addCard(IUser user, ICard card)
    {
        if (user == null || card == null){
            return;
        }

        if (_cardDic.ContainsKey(user.getUserId()))
        {
            List<ICard> list = _cardDic[user.getUserId()];
            for (int i = 0; i < list.Count; i++) {
                if (list[i].getType() == card.getType()) {
                    list[i] = card;
                    return;
                }
            }
            if (list.Count < MAXSLOT) {
                _cardDic[user.getUserId()].Add(card);
            }
        }
        else
        {
            _cardDic[user.getUserId()] = new List<ICard> { card };
        }
    }

    public List<ICard> getRandomCard(IUser user) {
        List<ICard> config = CardConfig.getConfig();
        List<ICard> list0 = new List<ICard>();
        List<ICard> list1 = new List<ICard>();
        List<ICard> list2 = new List<ICard>();
        List<ICard> userCards = new List<ICard>();

        if (_cardDic.ContainsKey(user.getUserId()))
        {
            userCards = _cardDic[user.getUserId()];
        }

        //判断满槽了
        int slot = MAXSLOT;
        for (int i = 0; i < userCards.Count; i++) {
            if (userCards[i].getLevel() > 1) {
                slot--;
            }
        }
        if (slot == 0) return list0;


        for (int i = 0; i < config.Count; i++) {
            if (config[i].getLevel() == 1) {
                list1.Add(config[i]);
            }
            else {
                list2.Add(config[i]);
            }
        }

        for (int i = 0; i < userCards.Count; i++) {
            if (userCards[i].getLevel() == 1)
            {
                for (int j = 0; j < list1.Count; j++) {
                    if (userCards[i].getType() == list1[j].getType() && 
                        userCards[i].getLevel() == list1[j].getLevel()) {
                        list1.RemoveAt(j);
                        break;
                    }
                }
            }
            else {
                for (int j = 0; j < list1.Count; j++)
                {
                    if (userCards[i].getType() == list1[j].getType())
                    {
                        list1.RemoveAt(j);
                        break;
                    }
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    if (userCards[i].getType() == list2[j].getType())
                    {
                        list2.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < list1.Count; i++)
        {
            for (int j = 0; j < list2.Count; j++)
            {
                if (list1[i].getType() == list2[j].getType())
                {
                    list2.RemoveAt(j);
                    break;
                }
            }
        }

        for (int i = 0; i < list2.Count; i++)
        {
            list1.Add(list2[i]);
        }
        
        for (int i = 0; i < 3; i++) {
            if (list1.Count == 0) break;
            int index = RandomMgr.Instance.getRangeInt(0,list1.Count);
            ICard card = list1[index];
            list0.Add(card);
            list1.RemoveAt(index);
        }
        return list0;
    }

    public List<ICard> getCards(IUser user) {
        if (!_cardDic.ContainsKey(user.getUserId()))
        {
            return null;
        }
        else {
            return _cardDic[user.getUserId()];
        }
    }

    public int getRound() {
        return _round;
    }

    public void addRound() {
        _round++;
    }

    //
    public void handle(ICardHandlePara para, CardHandleType type) {
        if (!_cardDic.ContainsKey(para.getUser().getUserId())) return;
        List<ICard> cards = _cardDic[para.getUser().getUserId()];
        List<ICardHandle> handles = CardConfig.getHandle();
        for (int i = 0; i < cards.Count; i++) {
            para.setCard(cards[i]);
            int index = cards[i].getId() - 1;
            if (index < handles.Count) {
                switch (type) {
                    case CardHandleType.dealPoker:
                        handles[index].dealPokerHandle(para);
                        break;
                    case CardHandleType.roundBegin:
                        handles[index].roundBeginHandle(para);
                        break;
                    case CardHandleType.roundAddValue:
                        handles[index].roundAddValueHandle(para);
                        break;
                    case CardHandleType.roundAddMagic:
                        handles[index].roundAddMagicHandle(para);
                        break;
                    case CardHandleType.roundAttackBegin:
                        handles[index].roundAttackBeginHandle(para);
                        break;
                    case CardHandleType.roundAttack:
                        handles[index].roundAttackHandle(para);
                        break;
                    case CardHandleType.roundMagicAttack:
                        handles[index].roundMagicAttackHandle(para);
                        break;
                    case CardHandleType.roundSubDefense:
                        handles[index].roundSubDefenseHandle(para);
                        break;
                    case CardHandleType.roundSubBlood:
                        handles[index].roundSubBloodHandle(para);
                        break;
                    case CardHandleType.roundAttackAfter:
                        handles[index].roundAttackAfterHandle(para);
                        break;
                    case CardHandleType.roundEnd:
                        handles[index].roundEndHandle(para);
                        break;
                }
            }
        }
    }
}
