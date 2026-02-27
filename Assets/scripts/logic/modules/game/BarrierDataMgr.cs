//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Pb;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class BarrierDealPoker {
    public BarrierDealType type;
    public IPoker poker;
    public int index;
}

public class BarrierDataMgr : Singleton<BarrierDataMgr>
{
    private Barrier _barrier;
    private List<IPoker> _npcList = new List<IPoker>();
    private List<IPoker> _playerList = new List<IPoker>();
    private List<IPoker> _otherList = new List<IPoker>();
    private BarrierPokerPile _pokerPile = new BarrierPokerPile();

    public Barrier newBarrier() {
        Barrier barrier = new Barrier();
        barrier.State = 0;
        barrier.MatchPointA = 0;
        barrier.MatchPointB = 0;
        barrier.PokerPosX = 0;
        barrier.PokerPosY = 0;
        barrier.FinalPoint = 0;
        barrier.BarrierId = 1;
        barrier.ChapterId = 1;
        barrier.RefreshNpcPokerNum = 1;
        barrier.RefreshPlayerPokerNum = 1;
        return barrier;
    }
    public void deserialized(GameData data)
    {
        this._barrier = data.Barrier;
        this._npcList.Clear();
        this._playerList.Clear();
        this._otherList.Clear();

        foreach (var value in this._barrier.NpcPokers) {
            this._npcList.Add(this._pokerPile.createPoker(value));
        }

        foreach (var value in this._barrier.PlayerPokers)
        {
            this._playerList.Add(this._pokerPile.createPoker(value));
        }

        foreach (var value in this._barrier.OtherPokers)
        {
            this._otherList.Add(this._pokerPile.createPoker(value));
        }

        this._pokerPile.init(this._barrier.PokerPile);
    }

    public void serialized(GameData data)
    {
        this._barrier.PokerPile.Clear();
        this._barrier.NpcPokers.Clear();
        this._barrier.PlayerPokers.Clear();
        this._barrier.OtherPokers.Clear();

        foreach (var value in _pokerPile.getRemainCards())
        {
            this._barrier.PokerPile.Add(value.getValue());
        }

        foreach(var value in this._npcList)
        {
            this._barrier.NpcPokers.Add(value.getValue());
        }

        foreach (var value in this._playerList)
        {
            this._barrier.PlayerPokers.Add(value.getValue());
        }

        foreach (var value in this._otherList)
        {
            this._barrier.OtherPokers.Add(value.getValue());
        }
        data.Barrier = this._barrier;
    }

    public BarrierDealPoker dealPoker(BarrierDealType type) 
    {
        IPoker poker = this._pokerPile.getPoker(0);
      
        BarrierDealPoker dealPoker = new BarrierDealPoker();
        dealPoker.poker = poker;
        dealPoker.type = type;
        dealPoker.index = 0;

        List<IPoker> list = this.getPokers(type);
        int x = list.FindIndex(p => p.getValue() == 0);
        if (x != -1)
        {
            list[x] = poker;
            dealPoker.index = x;
        }
        else {
            list.Add(poker);
            dealPoker.index = list.Count - 1;
        }
        
        return dealPoker;
    }

    public List<IPoker> getPokers(BarrierDealType type) {
        if (type == BarrierDealType.npc)
        {
            return this._npcList;
        }
        else if (type == BarrierDealType.player)
        {
            return this._playerList;
        }
        else if (type == BarrierDealType.other)
        {
            return this._otherList;
        }
        return this._npcList;
    }

    public void setMatchPoker(int matchPointA, int matchPointB, int pokerPosX, int pokerPosY) {
        this._barrier.PokerPosX = pokerPosX;
        this._barrier.PokerPosY = pokerPosY;
        this._barrier.MatchPointA = matchPointA;
        this._barrier.MatchPointB = matchPointB;
    }

    public int getMatchPointA()
    {
        return this._barrier.MatchPointA;
    }

    public int getMatchPointB()
    {
        return this._barrier.MatchPointB;
    }

    public int getPokerPosX()
    {
        return this._barrier.PokerPosX;
    }

    public int getPokerPosY()
    {
        return this._barrier.PokerPosY;
    }


    public BarrierState getState() {
        return (BarrierState)this._barrier.State;
    }

    public void setState(BarrierState state) {
        this._barrier.State = (int)state;
    }

    public int getFinalPoint() {
        return this._barrier.FinalPoint;
    }

    public int getBlackjack()
    {
        return this._barrier.Blackjack;
    }

    private List<int> getPokerValues() {
        List<int> values = new List<int>();
        values.Add(this._barrier.MatchPointA);
        values.Add(this._barrier.MatchPointB);
        for (int i = 0; i < this._otherList.Count; i++)
        {
            values.Add(this._otherList[i].getValue());
        }
        return values;
    }

    public int getMatchPoint() {
        return PokerPointMgr.Instance.getPokerPoint(this.getPokerValues());
    }

    public bool isBlackjack()
    {
        return PokerPointMgr.Instance.isBlackJack(this.getPokerValues());
    }

    public void clearMatch() {
        for (int i = 0; i < this._npcList.Count; i++) {
            if (this._npcList[i].getValue() == this._barrier.MatchPointA) {
                this._npcList[i].setValue(0);
                break;
            }
        }

        for (int i = 0; i < this._playerList.Count; i++){
            if (this._playerList[i].getValue() == this._barrier.MatchPointB){
                this._playerList[i].setValue(0);
                break;
            }
        }
        
        this._barrier.Blackjack = this.isBlackjack() ? 1 : 0;
        this._barrier.FinalPoint = this.getMatchPoint();
        this._barrier.MatchPointA = 0;
        this._barrier.MatchPointB = 0;
        this._barrier.PokerPosX = 0;
        this._barrier.PokerPosY = 0;
        this._otherList.Clear();
    }

    //±¬ÅÆµÄ¸ÅÂÊ
    public int getBustProbability()
    {
        List<int> list = new List<int>();
        list.Add(this._barrier.MatchPointA);
        list.Add(this._barrier.MatchPointB);
        for (int i = 0; i < this._otherList.Count; i++) {
            list.Add(this._otherList[i].getValue());
        }

        int number = 0;
        List<IPoker> cards = _pokerPile.getRemainCards();
        for (int i = 0; i < cards.Count; i++) {
            list.Add(cards[i].getValue());
            int point = PokerPointMgr.Instance.getPokerPoint(list);
            if (point > 21) {
                number++;
            }
            list.RemoveAt(list.Count - 1);
        }

        if (cards.Count > 0)
        {
            return (int)(Math.Ceiling((number * 1.0f) / cards.Count * 100));
        }
        else { 
            return number;
        }
    }

    public int getRefreshNpcPokerNum() {
        return this._barrier.RefreshNpcPokerNum;
    }

    public int setRefreshNpcPokerNum() {
        if (this._barrier.RefreshNpcPokerNum > 0) {
            this._barrier.RefreshNpcPokerNum -= 1;
        }

        int count = 0;
        for (int i = 0; i < this._npcList.Count; i++)
        {
            if (this._npcList[i].getValue() != this._barrier.MatchPointA)
            {
                count++;
                this._npcList[i].setValue(0);
            }
        }
        return count;
    }

    public int getRefreshPlayerPokerNum()
    {
        return this._barrier.RefreshPlayerPokerNum;
    }

    public int setRefreshPlayerPokerNum()
    {
        if (this._barrier.RefreshPlayerPokerNum > 0) {
            this._barrier.RefreshPlayerPokerNum -= 1;
        }

        int count = 0;
        for (int i = 0; i < this._playerList.Count; i++)
        {
            if (this._playerList[i].getValue() != this._barrier.MatchPointB)
            {
                count++;
                this._playerList[i].setValue(0);
            }
        }
        return count;
    }

    public int getChapterId() { 
        return this._barrier.ChapterId;
    }

    public int getBarrierId()
    {
        return this._barrier.BarrierId;
    }

    public void addBarrierId() {
        Chapter chapter = GameStaticConfigMgr.Instance.getChapterConfig().getChapter(this._barrier.ChapterId);
        if (chapter != null){
            this._barrier.BarrierId += 1;
            if (this._barrier.BarrierId > chapter.childTotal) {
                this._barrier.BarrierId = 1;
                this._barrier.ChapterId += 1;
            }
        }
    }
}
