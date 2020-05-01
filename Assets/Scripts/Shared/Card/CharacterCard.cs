﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public int SpacesMoved;

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
        protected set
        {
            n = value > 0 ? value : 0;
        }
    }
    public virtual int E
    {
        get
        {
            if (e < 0) return 0;
            return e;
        }
        protected set
        {
            e = value > 0 ? value : 0;
        }
    }
    public int S
    {
        get
        {
            if (s < 0) return 0;
            return s;
        }
        protected set { s = value > 0 ? value : 0 ; }
    }
    public int W
    {
        get
        {
            if (w < 0) return 0;
            return w;
        }
        protected set { w = value > 0 ? value : 0; }
    }
    #endregion stats

    //get other information
    public override int Cost => S;
    public override string StatsString => "N: " + N + "\t\tE: " + E + "\t\tS: " + S + "\t\tW: " + W;
    
    public SerializableCharCard GetSerializableVersion()
    {
        SerializableCharCard serializableChar = new SerializableCharCard
        {
            cardName = CardName,
            effText = EffText,
            n = n,
            e = e,
            s = s,
            w = w,
            subtypes = subtypes,
            
            location = location,
            owner = ControllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = SubtypeText
        };
        return serializableChar;
    }

    //set information
    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects)
    {
        if (!(serializedCard is SerializableCharCard serializedChar)) return;

        baseN = n = serializedChar.n;
        baseE = e = serializedChar.e;
        baseS = s = serializedChar.s;
        baseW = w = serializedChar.w;

        SpacesMoved = 0;

        base.SetInfo(serializedCard, game, owner, effects);
    }
    /// <summary>
    /// Should only be called by Game, which does any triggering or notifying as necessary
    /// </summary>
    public void SetNESW(int n, int e, int s, int w)
    {
        N = n;
        E = e;
        S = s;
        W = w;
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
        SpacesMoved = 0;
    }
}