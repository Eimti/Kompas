﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableCard
{
    //card type
    public char cardType;

    //perma-values
    public string cardName;
    public string effText;
    public string subtypeText;
    public string[] subtypes;
    public SerializableEffect[] effects;

    //in game values
    public CardLocation location;
    public int owner;
    public int BoardX;
    public int BoardY;
    public int index;

    /// <summary>
    /// Flip board position
    /// </summary>
    public void Invert()
    {
        BoardX = 6 - BoardX;
        BoardY = 6 - BoardY;
        owner = 1 - owner;
    }

}