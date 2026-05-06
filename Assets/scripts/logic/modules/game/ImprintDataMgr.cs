//ӡ��
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

        List<int> upgradeNumbers = new List<int>() { 3, 1, 1 };
        for (int i = 0; i < upgradeNumbers.Count; i++) {
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
            this._npcCards.Add(new AssembleCardObject(value.TriggerId,value.BaseEffectId, value.AdvancedEffectId, value.TriggerNumber, value.UpgradeNumber));
        }
        foreach (var value in this._imprint.PlayerCards)
        {
            this._playerCards.Add(new AssembleCardObject(value.TriggerId, value.BaseEffectId, value.AdvancedEffectId, value.TriggerNumber, value.UpgradeNumber));
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
        assembleCard.TriggerId = card.getTriggerId();
        assembleCard.BaseEffectId = card.getBaseEffectId();
        assembleCard.AdvancedEffectId = card.getAdvancedEffectId();
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
        return this.getAssembleCard(this._imprint.AssembleObject == 0);
    }

    public List<IAssembleCard> getAssembleCard(bool isNpc)
    {
        return isNpc ? this._npcCards : this._playerCards;
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
        if (targetType == TargetPart.baseEffect)
        {
            cards[index].setBaseEffectId(partId);
        }
        else if (targetType == TargetPart.advancedEffect) {
            cards[index].setAdvancedEffectId(partId);
        }
        else
        {
            cards[index].setTriggerId(partId);
        }
    }

    public bool setAdvancedEffectId(bool isNpc, int triggerId, int advancedEffectId) {
        List<IAssembleCard> list = this.getAssembleCard(isNpc);
        IAssembleCard assembleCard = list.Find(card => card.getTriggerId() == triggerId);
        if (assembleCard != null)
        {
            assembleCard.setAdvancedEffectId(advancedEffectId);
            return true;
        }
        else {
            return false;
        }
    }

    public IAssembleCard getAssembleCard(bool isNpc, int triggerId) {
        List<IAssembleCard> list = this.getAssembleCard(isNpc);
        IAssembleCard assembleCard = list.Find(card => card.getTriggerId() == triggerId);
        return assembleCard;
    }

    public bool hasAdvancedEffect(bool isNpc,int advancedEffectId) {
        List<IAssembleCard> list = this.getAssembleCard(isNpc);
        IAssembleCard assembleCard = list.Find(card => card.getAdvancedEffectId() == advancedEffectId);
        return assembleCard != null;
    }
}
