//ÅÆ¶Ñ
using Pb;
using Google.Protobuf.Collections;
using System.Collections.Generic;

public class BarrierDataMgr : Singleton<BarrierDataMgr>
{
    private Barrier _barrier;
    private List<IPoker> _npcList = new List<IPoker>();
    private List<IPoker> _playerList = new List<IPoker>();
    private BarrierPokerPile _pokerPile = new BarrierPokerPile();
    public void deserialized(GameData data)
    {
        this._barrier = data.Barrier;
        RepeatedField<int> npcPokers = this._barrier.NpcPokers;
        for (int i = 0; i < npcPokers.Count; i++) {
            this._npcList.Add(this._pokerPile.createPoker(npcPokers[i]));
        }

        RepeatedField<int> playerPokers = this._barrier.PlayerPokers;
        for (int i = 0; i < playerPokers.Count; i++)
        {
            this._playerList.Add(this._pokerPile.createPoker(npcPokers[i]));
        }

        this._pokerPile.init(this._barrier.PokerPile);
    }

    public void serialized(GameData data)
    {
        this._barrier.PokerPile.Clear();
        List<IPoker> remainCards = _pokerPile.getRemainCards();
        for (int i = 0; i < remainCards.Count; i++)
        {
            this._barrier.PokerPile.Add(remainCards[i].getValue());
        }

        this._barrier.PlayerPokers.Clear();
        for (int i = 0; i < this._playerList.Count; i++)
        {
            this._barrier.PlayerPokers.Add(this._playerList[i].getValue());
        }

        this._barrier.NpcPokers.Clear();
        for (int i = 0; i < this._npcList.Count; i++)
        {
            this._barrier.NpcPokers.Add(this._npcList[i].getValue());
        }
        data.Barrier = this._barrier;
    }

    public IPoker dealNpcPoker() 
    {
        IPoker poker = this._pokerPile.getPoker(0);
        this._npcList.Add(poker);
        return poker;
    }

    public IPoker dealPlayerPoker() {
        IPoker poker = this._pokerPile.getPoker(0);
        this._playerList.Add(poker);
        return poker;
    }

    public List<IPoker> getNpcPokers() {
        return this._npcList;
    }

    public List<IPoker> getPlayerPokers()
    {
        return this._playerList;
    }

    public void setMatchPoker(int matchPointA, int matchPointB, int offsetX, int offsetY) {
        this._barrier.OffsetX = offsetX;
        this._barrier.OffsetY = offsetY;
        this._barrier.MatchPointA = matchPointA;
        this._barrier.MatchPointB = matchPointB;
    }

    public BarrierState getState() {
        return (BarrierState)this._barrier.State;
    }
}
