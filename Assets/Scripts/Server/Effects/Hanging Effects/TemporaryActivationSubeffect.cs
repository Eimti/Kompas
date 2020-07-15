﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryActivationSubeffect : TemporaryCardChangeSubeffect
{
    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var tempActivation = new HangingActivationEffect(ServerGame, TriggerRestriction, EndCondition,
            Target, this);
        return new List<(HangingEffect, GameCard)>() { (tempActivation, Target) };
    }
}
