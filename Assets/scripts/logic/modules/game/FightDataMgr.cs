//
using Pb;
using System.Collections.Generic;

public class FightDataMgr : Singleton<FightDataMgr>
{
    private const int MAXSLOT = 3;
    private Fight _fight;
    private List<IPoker> _npcPokers = new List<IPoker>();
    private List<ICard> _npcCards = new List<ICard>();
    private List<IPoker> _playerPokers = new List<IPoker>();
    private List<ICard> _playerCards = new List<ICard>();
    private FightPokerPile _pokerPile = new FightPokerPile();

    public Fight newFight() {
        Fight fight = new Fight();
        fight.NpcAsset = new AssetInfo();
        fight.NpcAsset.Extra = new ExtraInfo();

        fight.PlayerAsset = new AssetInfo();
        fight.PlayerAsset.Extra = new ExtraInfo();
        return fight;
    }

    public void deserialized(GameData data)
    {
        this._fight = data.Fight;
        this._npcCards.Clear();
        foreach (var value in this._fight.NpcAsset.Cards)
        {
            this._npcCards.Add(CardConfig.getCard(value));
        }

        this._npcPokers.Clear();
        foreach (var value in this._fight.NpcAsset.Pokers)
        {
            this._npcPokers.Add(this._pokerPile.createPoker(value));
        }

        this._playerCards.Clear();
        foreach (var value in this._fight.PlayerAsset.Cards)
        {
            this._playerCards.Add(CardConfig.getCard(value));
        }
        this._playerPokers.Clear();
        foreach (var value in this._fight.PlayerAsset.Pokers)
        {
            this._playerPokers.Add(this._pokerPile.createPoker(value));
        }
        this._pokerPile.init(this._fight.PokerPile);

        //
        if (this._fight.NpcAsset.IsFilp == 0 && this._npcPokers.Count > 0) {
            this._npcPokers[0].setBack(true);
        }
    }

    public void serialized(GameData data)
    {
        this._fight.PokerPile.Clear();
        foreach (var value in this._pokerPile.getRemainCards())
        {
            this._fight.PokerPile.Add(value.getValue());
        }

        this._fight.NpcAsset.Cards.Clear();
        foreach (var value in this._npcCards)
        {
            this._fight.NpcAsset.Cards.Add(value.getId());
        }

        this._fight.NpcAsset.Pokers.Clear();
        foreach (var value in this._npcPokers)
        {
            this._fight.NpcAsset.Pokers.Add(value.getValue());
        }

        this._fight.PlayerAsset.Cards.Clear();
        foreach (var value in this._playerCards)
        {
            this._fight.PlayerAsset.Cards.Add(value.getId());
        }

        this._fight.PlayerAsset.Pokers.Clear();
        foreach (var value in this._playerPokers)
        {
            this._fight.PlayerAsset.Pokers.Add(value.getValue());
        }
        data.Fight = this._fight;
    }

    //
    public void initEntry() {
        this._fight.State = 0;
        this._fight.NpcAsset.State = 0;
        this._fight.NpcAsset.Hp = 100;
        this._fight.NpcAsset.MaxHP = 100;
        this._fight.NpcAsset.Magic = 0;
        this._fight.NpcAsset.MaxMagic = 50;
        this._fight.NpcAsset.Defense = 0;
        this._fight.NpcAsset.Attack = 0;
        this._fight.NpcAsset.IsFilp = 0;
        this._npcCards.Clear();
        this._npcPokers.Clear();


        this._fight.PlayerAsset.State = 0;
        this._fight.PlayerAsset.Hp = 100;
        this._fight.PlayerAsset.MaxHP = 100;
        this._fight.PlayerAsset.Magic = 0;
        this._fight.PlayerAsset.MaxMagic = 50;
        this._fight.PlayerAsset.Defense = 0;
        this._fight.PlayerAsset.Attack = 0;
        this._fight.PlayerAsset.IsFilp = 1;
        this._playerCards.Clear();
        this._playerPokers.Clear();
    }

    public List<IPoker> getPokers(FightDealType type)
    {
        if (type == FightDealType.npc)
        {
            return this._npcPokers;
        }
        else if (type == FightDealType.player)
        {
            return this._playerPokers;
        }
        return this._npcPokers;
    }

    public IPoker dealPoker(FightDealType type, int suit) {
        int index = 0;

        if (suit > 0) {
            index = this._pokerPile.getRemainCards().FindIndex(poker => poker.getSuit() == (PokerSuit)suit);
        }

        if (index < 0) { 
            index = 0;
        }

        IPoker poker = this._pokerPile.getPoker(index);
        List<IPoker> list = this.getPokers(type);
        list.Add(poker);

        //npc
        if (list.Count == 1 && this.getAssetInfo(type).IsFilp == 0){
            poker.setBack(true);
        }

        return poker;
    }

    public FightPokerPile getPokerPile() {
        return this._pokerPile;
    }

    public int getRemainCards() {
        return this._pokerPile.getRemainCards().Count;
    }

    public void shuffle()
    {
        this._pokerPile.shuffle();
    }

    public AssetInfo getAssetInfo(FightDealType type) {
        if (type == FightDealType.npc)
        {
            return this._fight.NpcAsset;
        }
        else {
            return this._fight.PlayerAsset;
        }
    }

    public List<ICard> getCards(FightDealType type) {
        if (type == FightDealType.npc)
        {
            return this._npcCards;
        }
        else
        {
            return this._playerCards;
        }
    }

    public bool addCard(FightDealType type,ICard card) {
        List<ICard> list = this.getCards(type);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].getType() == card.getType())
            {
                list[i] = card;
                return true;
            }
        }
        
        if (list.Count < MAXSLOT)
        {
            list.Add(card);
            return true;
        }
        else
        {
            return false;
        }
    }

    public int getRound() {
        return this._fight.Round;
    }

    public int addRound()
    {
        return ++this._fight.Round;
    }

    public void setState(int state)
    {
        this._fight.State = state;
    }

    public int getState()
    {
        return this._fight.State;
    }

    public void setUserState(FightDealType type, UserState state) {
        AssetInfo info = this.getAssetInfo(type);
        info.State = (int)state;
    }

    public void setIsFilp(FightDealType type,int filpState) {
        this.getAssetInfo(type).IsFilp = filpState;
    }

    //
    public void addCardId(FightDealType type,int cardId) {
        ICard card = CardConfig.getCard(cardId);
        if (card != null)
        {
            if (type == FightDealType.npc)
            {
                this._npcCards.Add(card);
            }
            else if (type == FightDealType.player)
            {
                this._playerCards.Add(card);
            }
        }
    }
}
