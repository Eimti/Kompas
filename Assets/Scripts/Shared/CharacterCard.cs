﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card {

    //game mechanic data
    private int n;
    protected int e;
    private int s;
    private int w;
    private int baseN;
    private int baseE;
    private int baseS;
    private int baseW;

    //stat getters TODO take into account tags here
    //reminder: don't need separate setters because you don't notify because you'll only change stats when server tells you to
    //all of these will return 0 if their value is < 0
    #region stats
    public int N
    {
        get {
            if (n < 0) return 0;
            return n;
        }
        set
        {
            n = value;
        }
    }
    public virtual int E
    {
        get
        {
            if (e < 0) return 0;
            return e;
        }
        set
        {
            e = value;
        }
    }
    public int S
    {
        get
        {
            if (s < 0) return 0;
            return s;
        }
        set { s = value; }
    }
    public int W
    {
        get
        {
            if (w < 0) return 0;
            return w;
        }
        set { w = value; }
    }
    #endregion stats

    //get other information
    public override int Cost { get { return S; } }
    public string GetStatsString() { return "N: " + N + "\t\tE: " + E + "\t\tS: " + S + "\t\tW: " + W; }
    
    public SerializableCharCard GetSerializableVersion()
    {
        int index = -1;
        if (location == CardLocation.Hand) index = game.Players[controllerIndex].handCtrl.IndexOf(this);
        else if(location == CardLocation.Discard) index = game.Players[controllerIndex].discardCtrl.IndexOf(this);

        SerializableCharCard serializableChar = new SerializableCharCard
        {
            cardName = cardName,
            effText = effText,
            n = n,
            e = e,
            s = s,
            w = w,
            subtypes = subtypes,
            
            location = location,
            owner = controllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = subtypeText,
            index = index
            
        };
        return serializableChar;
    }

    //set information
    public override void SetInfo(SerializableCard serializedCard, Game game, int ownerIndex)
    {
        if (!(serializedCard is SerializableCharCard serializedChar)) return;

        baseN = n = serializedChar.n;
        baseE = e = serializedChar.e;
        baseS = s = serializedChar.s;
        baseW = w = serializedChar.w;

        base.SetInfo(serializedCard, game, ownerIndex);
    }
    /// <summary>
    /// For convenience of not having to type out four lines
    /// </summary>
    public void SetNESW(int n, int e, int s, int w)
    {
        N = n;
        E = e;
        S = s;
        W = w;
    }

    /// <summary>
    /// Resets this card's N to its printed/base N
    /// </summary>
    public void ResetN()
    {
        N = baseN;
    }
    
    public override void ResetCard()
    {
        n = baseN;
        e = baseE;
        s = baseS;
        w = baseW;
    }

    public override void ResetForTurn()
    {
        base.ResetForTurn();
        ResetN();
    }
}