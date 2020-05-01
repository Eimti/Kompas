﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAllSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        bool found = false;
        foreach (KeyValuePair<int, Card> pair in ServerGame.cards)
        {
            if (cardRestriction.Evaluate(pair.Value))
            {
                ServerEffect.targets.Add(pair.Value);
                found = true;
            }
        }

        if (found) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}
