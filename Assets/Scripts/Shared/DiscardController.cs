﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardController : MonoBehaviour {

    public Game game;
    public Player Owner;

    private List<GameCard> discard = new List<GameCard>();
    public List<GameCard> Discard { get { return discard; } }

    //info about discard
    public int DiscardSize() { return discard.Count; }
    public GameCard GetLastDiscarded() { return discard[discard.Count - 1]; }

    public GameCard CardAt(int index, bool remove)
    {
        if (index >= discard.Count) return null;
        GameCard card = discard[index];
        if (remove) discard.RemoveAt(index);
        return card;
    }

    //adding/removing cards
	public virtual bool AddToDiscard(GameCard card, IStackable stackSrc = null)
    {
        Debug.Assert(card != null);
        Debug.Log("Adding to discard: " + card.CardName);
        card.Remove(stackSrc);
        discard.Add(card);
        card.Controller = Owner;
        card.Location = CardLocation.Discard;
        return true;
    }

    public int IndexOf(GameCard card)
    {
        return discard.IndexOf(card);
    }

    public void RemoveFromDiscard(GameCard card)
    {
        Debug.Assert(card != null);
        card.ResetCard();
        discard.Remove(card);
    }

    public void RemoveFromDiscardAt(int index)
    {
        Debug.Assert(index < discard.Count);
        discard.RemoveAt(index);
    }

    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(GameCard c in discard)
        {
            if (cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }

    public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction)
    {
        List<GameCard> cards = new List<GameCard>();

        foreach(GameCard c in discard)
        {
            if (cardRestriction.Evaluate(c)) cards.Add(c);
        }

        return cards;
    }
}
