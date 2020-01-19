﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet {

    public static int id = 0;

    public enum Command { Nothing, Confirm,
        Play, Augment, Move, EndTurn, Attack,
        Topdeck, Discard, Rehand, Reshuffle, AddAsFriendly, AddAsEnemy, AddToDeck, Delete,
        IncrementEnemyDeck, IncrementEnemyHand, DecrementEnemyDeck, DecrementEnemyHand,
        Target, Response, TargetAccepted, SpaceTargetAccepted,
        PlayerSetX, DeclineAnotherTarget, EnableDecliningTarget, DisableDecliningTarget, SpaceTarget, SetEffectsX,
        Draw, SetNESW, SetPips, SetEnemyPips, PutBack, GetAttackTarget, YoureFirst, YoureSecond, RequestBoardTarget, RequestDeckTarget, RequestDiscardTarget, RequestHandTarget,
        DiscardSimples,
        TestTargetEffect }

    /// <summary>
    /// Contains the command that is sent.
    /// <para>Possible commands include: Summon, Cast, Augment, SetNESW, SetMyPips, SetYourPips, MoveChar, MoveSpell</para>
    /// </summary>
    public Command command;
    
    public int cardID;
    public int packetID;

    public int[] args;

    public string CardName { get { return Game.CardNames[args[1]]; } }
    public int CardIDToBe { get { return cardID; } }

    public int Pips { get { return args[0]; } }
    public int EffectX { get => args[0]; }

    public CardLocation Location { get { return (CardLocation)args[0]; } }

    public int X { get { return args[2]; } }
    public int Y { get { return args[3]; } }

    public int N { get { return args[0]; } }
    public int E { get { return args[1]; } }
    public int S { get { return args[2]; } }
    public int W { get { return args[3]; } }

    public int EffIndex { get => args[0]; }
    public int SubeffIndex { get => args[1]; }

    #region constuctors 
    /// <summary>
    /// Use only for confirm packet
    /// </summary>
    /// <param name="packetID"></param>
    public Packet(int packetID)
    {
        command = Command.Confirm;
        this.packetID = packetID;
        args = new int[4];
    }

    public Packet(Command command)
    {
        this.command = command;
        if (command != Command.Nothing)
        {
            packetID = id;
            id = (id + 1) % 1000;
        }

        args = new int[4];
    }

    //used only for adding cards to deck
    public Packet(Command command, string cardName) : this(command)
    {
        try {
            args[1] = Game.CardNameIndices[cardName];
        }
        catch(KeyNotFoundException e)
        {
            Debug.Log("Could not find card with name \"" + cardName + "\", length " + cardName.Length + "\n" + e.StackTrace);
        }
    }

    public Packet(Command command, int num) : this(command)
    {
        args[0] = num;
    }

    public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe) : this(command, cardName)
    {
        args[0] = cardLocation;
        cardID = cardIDtoBe;
    }

    public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe, int x, int y, bool invert) : this(command, cardName, cardLocation, cardIDtoBe)
    {
        if (invert)
        {
            args[2] = 6 - x;
            args[3] = 6 - y;
        }
        else
        {
            args[2] = x;
            args[3] = y;
        }
    }

    public Packet(Command command, Card card) : this(command)
    {
        if(card != null) cardID = card.ID;
    }

    public Packet(Command command, Card card, int num) : this(command, card)
    {
        args[0] = num;
    }

    /// <summary>
    /// Used for choosing coordinates/spaces
    /// </summary>
    public Packet(Command command, int x, int y, bool invert = false) : this(command)
    {
        //this is used for the target packet
        args[0] = x;
        args[1] = y;

        if (invert)
        {
            args[2] = 6 - x;
            args[3] = 6 - y;
        }
        else
        {
            args[2] = x;
            args[3] = y;
        }
    }

    public Packet(Command command, Card card, int x, int y, bool invert = false) : this(command, card)
    {
        //this is used for the target packet
        args[0] = x;
        args[1] = y;

        if (invert)
        {
            args[2] = 6 - x;
            args[3] = 6 - y;
        }
        else
        {
            args[2] = x;
            args[3] = y;
        }
    }

    public Packet(Command command, Card card, int n, int e, int s, int w) : this(command, card)
    {
        args[0] = n;
        args[1] = e;
        args[2] = s;
        args[3] = w;
    }
#endregion 


    public void Invert()
    {
        if(args == null)
        {
            Debug.Log("args is null");
            return;
        }
        args[2] = 6 - args[2];
        args[3] = 6 - args[3];
    }

    public void InvertForController(int playerFrom)
    {
        if (playerFrom == 1 && command != Command.SetNESW) Invert();
    }

}