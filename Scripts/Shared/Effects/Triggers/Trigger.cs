﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Trigger
{
    public TriggerCondition TriggerCondition;
    public Effect effToTrigger;
    public TriggerRestriction triggerRestriction;

    public static Trigger FromJson(TriggerCondition c, string json, Effect parent)
    {
        Trigger toReturn = null;

        switch (c)
        {
            case TriggerCondition.TurnStart:
                toReturn = JsonUtility.FromJson<Trigger>(json);
                break;
        }

        if(toReturn != null)
        {
            toReturn.TriggerCondition = c;
            toReturn.effToTrigger = parent;
            if(toReturn.triggerRestriction != null)
            {
                toReturn.triggerRestriction.thisTrigger = toReturn;
                toReturn.triggerRestriction.thisCard = parent.thisCard;
            }
        }

        return toReturn;
    }

    protected void TriggerEffect(Effect trigger, int? x)
    {
        if (x.HasValue) effToTrigger.X = x.Value;
        effToTrigger.serverGame.PushToStack(effToTrigger, effToTrigger.thisCard.ControllerIndex);
    }

    protected bool CheckTriggerRestrictions(Effect trigger, int? x)
    {
        if (trigger.TimesUsedThisTurn >= trigger.MaxTimesCanUsePerTurn) return false;
        return triggerRestriction.Evaluate();
    }

    public virtual void TriggerIfValid(Effect trigger, int? x)
    {
    }
}
