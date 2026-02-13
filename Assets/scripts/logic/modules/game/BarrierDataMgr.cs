//ÅÆ¶Ñ
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Pb;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrierDataMgr : Singleton<BarrierDataMgr>
{
    private Barrier _barrier;
    private List<IPoker> _npcList = new List<IPoker>();
    private List<IPoker> _playerList = new List<IPoker>();
    private List<IPoker> _otherList = new List<IPoker>();
    private BarrierPokerPile _pokerPile = new BarrierPokerPile();
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

    public IPoker dealPoker(BarrierDealType type) 
    {

        IPoker poker = this._pokerPile.getPoker(0);
        if (type == BarrierDealType.npc)
        {
            this._npcList.Add(poker);
        }
        else if (type == BarrierDealType.player)
        {
            this._playerList.Add(poker);
        }
        else if (type == BarrierDealType.other) {
            this._otherList.Add(poker);
        }
        return poker;
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

    public int getMatchPoint() {
        List<int> values = new List<int> ();
        values.Add(this._barrier.MatchPointA);
        values.Add(this._barrier.MatchPointB);
        for (int i = 0; i < this._otherList.Count; i++) {
            values.Add(this._otherList[i].getValue());
        }
        return PokerPointMgr.Instance.getPokerPoint(values);
    }

    public void clearMatch() {
        this._barrier.FinalPoint = this.getMatchPoint();

        for (int i = 0; i < this._npcList.Count; i++) {
            if (this._npcList[i].getValue() == this._barrier.MatchPointA) { 
                this._npcList.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < this._playerList.Count; i++)
        {
            if (this._playerList[i].getValue() == this._barrier.MatchPointB)
            {
                this._playerList.RemoveAt(i);
                break;
            }
        }
        this._otherList.Clear();
        this._barrier.MatchPointA = 0;
        this._barrier.MatchPointB = 0;
        this._barrier.PokerPosX = 0;
        this._barrier.PokerPosY = 0;
    }
}
