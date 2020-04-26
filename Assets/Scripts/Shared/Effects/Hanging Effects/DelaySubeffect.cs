﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaySubeffect : Subeffect
{
    public int NumTimesToDelay = 0;
    public int IndexToResume;
    public TriggerCondition TriggerCondition;
    public TriggerRestriction TriggerRestriction = new TriggerRestriction();

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        TriggerRestriction.Initialize(this);
    }

    public override void Resolve()
    {
        var eff = new DelayedHangingEffect(ServerGame, TriggerRestriction, TriggerCondition,
            NumTimesToDelay, Effect, IndexToResume);
        ServerGame.RegisterHangingEffect(TriggerCondition, eff);
        Effect.EffectImpossible();
    }
}
