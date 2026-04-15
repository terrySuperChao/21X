//印记
using Pb;
using System.Collections.Generic;
using UnityEngine;

public class ImprintDataMgr : Singleton<ImprintDataMgr>
{
    private Imprint _imprint;
    private List<IAssembleCard> _npcCards = new List<IAssembleCard>();
    private List<IAssembleCard> _playerCards = new List<IAssembleCard>();
    public Imprint newImprint() {
        Imprint imprint = new Imprint();
        for (int i = 0; i < 3; i++) {
            imprint.NpcCards.Add(new AssembleCard());
            imprint.PlayerCards.Add(new AssembleCard());
        }

        //初始升级需要的次数
        List<int> upgradeNumbers = new List<int>() { 3,1,1};
        for (int i = 0; i<upgradeNumbers.Count; i++) {
            imprint.NpcCards[i].UpgradeNumber = upgradeNumbers[i];
            imprint.PlayerCards[i].UpgradeNumber = upgradeNumbers[i];
        }

        return imprint;
    }
    public void deserialized(GameData data)
    {
        this._imprint = data.Imprint;
        if (this._imprint == null) {
            this._imprint = this.newImprint();
        }
       
        this._npcCards.Clear();
        this._playerCards.Clear();
        foreach (var value in this._imprint.NpcCards)
        {
            this._npcCards.Add(new AssembleCardObject(value.BaseDataId, value.TriggerId, value.Level,value.TriggerNumber,value.UpgradeNumber));
        }
        foreach (var value in this._imprint.PlayerCards)
        {
            this._playerCards.Add(new AssembleCardObject(value.BaseDataId, value.TriggerId, value.Level, value.TriggerNumber, value.UpgradeNumber));
        }
    }

    public void serialized(GameData data)
    {
        this._imprint.NpcCards.Clear();
        this._imprint.PlayerCards.Clear();
        foreach (var value in this._npcCards)
        {
            this._imprint.NpcCards.Add(this.newAssembleCard(value));
        }
        foreach (var value in this._playerCards)
        {
            this._imprint.PlayerCards.Add(this.newAssembleCard(value));
        }
        data.Imprint = this._imprint;
    }

    private AssembleCard newAssembleCard(IAssembleCard card) {
        AssembleCard assembleCard = new AssembleCard();
        assembleCard.BaseDataId = card.getBaseDataId();
        assembleCard.TriggerId = card.getTriggerId();
        assembleCard.Level = card.getLevel();
        assembleCard.TriggerNumber = card.getTriggerNumber();
        assembleCard.UpgradeNumber = card.getUpgradeNumber();
        return assembleCard;
    }

    public void setAssembleObject(int obj) { 
        this._imprint.AssembleObject = obj;
    }

    public int getAssembleObject() { 
        return this._imprint.AssembleObject;
    }

    public List<IAssembleCard> getAssembleCard() { 
        return this._imprint.AssembleObject == 0 ? this._npcCards : this._playerCards;
    }

    public List<IAssembleCard> getNpcAssembleCard()
    {
        return this._npcCards;
    }

    public List<IAssembleCard> getPlayerAssembleCard()
    {
        return this._playerCards;
    }

    public void addPart(int index, TargetPart targetType, int partId) {
        List<IAssembleCard> cards = this.getAssembleCard();
        if (cards.Count < index) {
            return;
        }
        if (targetType == TargetPart.basePart) {
            cards[index].setBaseDataId(partId);
        }
        else {
            cards[index].setTriggerId(partId);
        }
    }

    public bool addTriggerNumber(bool isNpc,int triggerId) {
        List<IAssembleCard> list = isNpc ? this._npcCards : this._playerCards;

        IAssembleCard assembleCard = list.Find(card => card.getTriggerId() == triggerId);
        if (assembleCard != null) {
            assembleCard.addTriggerNumber();
        }

        return assembleCard != null && assembleCard.getTriggerNumber() == assembleCard.getUpgradeNumber();
    }

    public void upgradeBasePartId(bool isNpc, int triggerId,int basePartId) {
        List<IAssembleCard> list = isNpc ? this._npcCards : this._playerCards;
        IAssembleCard assembleCard = list.Find(card => card.getTriggerId() == triggerId);
        if (assembleCard != null)
        {
            assembleCard.setBaseDataId(basePartId);
        }
    }
}
