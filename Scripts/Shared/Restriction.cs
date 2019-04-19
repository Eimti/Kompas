﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Restriction
{
    [System.NonSerialized] public Subeffect subeffect;

    public virtual bool Evaluate(bool actuallyTargetThis)
    {
        Debug.Log("Parent restriction always returns true");
        return true;
    }

    public virtual bool Evaluate(Card card, bool actuallyTargetThis)
    {
        Debug.Log("If there is no override method, the parent will return true for Evaluate");
        return true;
    }
}
