﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : KompasObject
{
    public const int spacesOnBoard = 7;

    //private CharacterCard[,] characters = new CharacterCard[spacesOnBoard, spacesOnBoard];
    //private SpellCard[,] spells = new SpellCard[spacesOnBoard, spacesOnBoard];
    private Card[,] cards = new Card[spacesOnBoard, spacesOnBoard];
    /// <summary>
    /// Whether all cards, only chars, only spells, or only augs are visible
    /// </summary>
    private int visibleCards = 0;

    //helper methods
    #region helper methods
    public bool ValidIndices(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 7 && y < 7;
    }

    //get game data
    public Card GetCardAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y];
    }

    public CharacterCard GetCharAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y] as CharacterCard;
    }

    public SpellCard GetSpellAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y] as SpellCard; //returns null if the card there isn't a spell
    }

    public List<AugmentCard> GetAugmentsAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        //if the card at x y is null, or not a character card, returns null
        return (cards[x, y] as CharacterCard)?.Augments;
    }

    public int GetNumCardsOnBoard()
    {
        int i = 0;
        foreach (Card card in cards){
            if (card != null) i++;
        }
        return i;
    }

    public void ResetCardsForTurn()
    {
        foreach(Card c in cards)
        {
            c?.ResetForTurn();
        }
    }
    #endregion

    #region game mechanics
    public void RemoveFromBoard(Card toRemove)
    {
        if (toRemove == null || toRemove.Location != CardLocation.Field) return;

        if (toRemove is CharacterCard || toRemove is SpellCard)
            cards[toRemove.BoardX, toRemove.BoardY] = null;
        else if (toRemove is AugmentCard augToRemove)
            augToRemove.Detach();
    }

    public void RemoveFromBoard(int x, int y) { RemoveFromBoard(GetCardAt(x, y)); }

    //playing. these methods don't check whether it's client or server. that's the Game methods' jobs. 
    // these just do it (they're called by ClientNetworkController when ordered by the server)
    /// <summary>
    /// Actually summons the card. DO NOT call directly from player interaction
    /// </summary>
    public void Summon(CharacterCard toSummon, int toX, int toY, int owner)
    {
        cards[toX, toY] = toSummon;
        toSummon.SetLocation(CardLocation.Field);
        toSummon.MoveTo(toX, toY);
        toSummon.ChangeController(owner);
    }

    /// <summary>
    /// Actually augments the card. DO NOT call directly from player interaction
    /// </summary>
    public void Augment(AugmentCard toAugment, int toX, int toY, int owner)
    {
        GetCharAt(toX, toY).AddAugment(toAugment);
        toAugment.SetLocation(CardLocation.Field);
        toAugment.MoveTo(toX, toY);
        toAugment.ChangeController(owner);
    }

    /// <summary>
    /// Actually casts the card. DO NOT call directly from player interaction
    /// </summary>
    public void Cast(SpellCard toCast, int toX, int toY, int owner)
    {
        cards[toX, toY] = toCast;
        toCast.SetLocation(CardLocation.Field);
        toCast.MoveTo(toX, toY);
        toCast.ChangeController(owner);
    }

    /// <summary>
    /// Calls the appropriate summon/augment/cast method for the card
    /// </summary>
    /// <param name="toPlay">Card to be played</param>
    /// <param name="toX">X coordinate to play the card to</param>
    /// <param name="toY">Y coordinate to play the card to</param>
    public void Play(Card toPlay, int toX, int toY, int owner)
    {
        if (toPlay is CharacterCard charToPlay) Summon(charToPlay, toX, toY, owner);
        else if (toPlay is AugmentCard augmentToPlay) Augment(augmentToPlay, toX, toY, owner);
        else if (toPlay is SpellCard spellToPlay) Cast(spellToPlay, toX, toY, owner);
        else Debug.Log("Can't play a card that isn't a character, augment, or spell.");

        int i = GetNumCardsOnBoard();
        if (i > game.MaxCardsOnField) game.MaxCardsOnField = i;

        toPlay.gameObject.transform.localScale = new Vector3(1f / 9f, 1f / 9f, 1);
    }

    public void Play(Card toPlay, int toX, int toY)
    {
        Play(toPlay, toX, toY, toPlay.ControllerIndex);
    }

    //movement
    public void Swap(Card card, int toX, int toY)
    {
        if (!ValidIndices(toX, toY) || card == null) return;
        if (card is AugmentCard) throw new NotImplementedException();

        Card temp = null;
        int tempX;
        int tempY;
        temp                            = cards[toX, toY];
        cards[toX, toY]                 = card;
        cards[card.BoardX, card.BoardY] = temp;
        
        tempX = card.BoardX;
        tempY = card.BoardY;

        //set N if cards are characters?
        //if (card is CharacterCard charCard) charCard.N -= charCard.DistanceTo(toX, toY);
        //if (temp is CharacterCard charTemp) charTemp.N -= charTemp.DistanceTo(tempX, tempY);
        //note 

        //then let the cards know they've been moved
        card.MoveTo(toX, toY);
        if (temp != null) temp.MoveTo(tempX, tempY);
    }

    public void Move(Card card, int toX, int toY)
    {
        if (!ValidIndices(toX, toY)) return;

        if (card is AugmentCard augCard)
        {
            augCard.Detach();
            GetCharAt(toX, toY).AddAugment(augCard);
        }
        else Swap(card, toX, toY);
    }

    public void PutCardsBack()
    {
        foreach(Card card in cards)
        {
            if(card != null) card.PutBack();
        }
    }

    public bool ExistsCardOnBoard(CardRestriction restriction)
    {
        foreach(Card c in cards)
        {
            if (c != null && restriction.Evaluate(c)) return true;
        }

        return false;
    }

    public bool CanSummonTo(int playerIndex, int x, int y)
    {
        foreach(Card c in cards)
        {
            if (c != null && c.IsAdjacentTo(x, y) && c.ControllerIndex == playerIndex) return true;
        }

        return false;
    }

    public void DiscardSimples()
    {
        foreach(Card c in cards)
        {
            if(c != null && c is SpellCard spellC && spellC.SpellSubtype == SpellCard.SpellType.Simple)
            {
                spellC.Discard();
            }
        }
    }
    #endregion game mechanics

    #region cycling visible cards
    private void WhichCardsVisible(bool charsActive, bool spellsActive, bool augsActive)
    {
        //TODO check if this works
        foreach (Card card in cards)
        {
            if (card == null) continue;

            if (card is CharacterCard charCard)
            {
                card.gameObject.SetActive(charsActive);
                foreach (AugmentCard augment in charCard.Augments)
                    augment.gameObject.SetActive(augsActive);
            }
            else if (card is SpellCard) card.gameObject.SetActive(spellsActive);
        }
    }

    public void CycleVisibleCards()
    {
        visibleCards = ++visibleCards % 4;

        switch (visibleCards)
        {
            case 0:
                //set all cards visible
                WhichCardsVisible(true, true, true);
                break;
            case 1:
                //hide all but characters
                WhichCardsVisible(true, false, false);
                break;
            case 2:
                //hide all but non-aug spells
                WhichCardsVisible(false, true, false);
                break;
            case 3:
                //hide all but augs
                WhichCardsVisible(false, false, true);
                break;
        }
    }
    #endregion

    public override void OnClick()
    {
        if (game.targetMode != Game.TargetMode.SpaceTarget) return;
        //if someone wants a space target, get the x/y coordinates clicked
        Vector3 intersection = transform.InverseTransformPoint(game.mouseCtrl.GetRayIntersectBoard());
        int xIntersection = PosToGridIndex(intersection.x);
        int yIntersection = PosToGridIndex(intersection.y);
        //then, if the game is a clientgame, request a space target
        (game as ClientGame)?.clientNotifier.RequestSpaceTarget(xIntersection, yIntersection);
    }


}