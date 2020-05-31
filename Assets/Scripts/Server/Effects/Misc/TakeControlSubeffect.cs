﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlSubeffect : ServerSubeffect
{
    public int ControllerIndexOffset = 0;

    //TODO abstract this logic into a parent class with other player offset things
    private Player NewController => ServerGame.Players[EffectController.index + ControllerIndexOffset % ServerGame.Players.Length];

    public override void Resolve()
    {
        ServerGame.ChangeControl(Target, NewController);
        ServerEffect.ResolveNextSubeffect();
    }
}