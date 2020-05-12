﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAllNESWSubeffect : ServerSubeffect
{
    public int NMod = 0;
    public int EMod = 0;
    public int SMod = 0;
    public int WMod = 0;
    //default to making sure things are characters before changing their stats
    public BoardRestriction BoardRestriction = new BoardRestriction()
    {
        restrictionsToCheck = new CardRestriction.CardRestrictions[]
        {
            CardRestriction.CardRestrictions.IsCharacter
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        BoardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        foreach(KeyValuePair<int, Card> card in ServerGame.cards)
        {
            if (BoardRestriction.Evaluate(card.Value))
            {
                ServerGame.AddToStats(card.Value as CharacterCard, NMod, EMod, SMod, WMod);
            }
        }

        ServerEffect.ResolveNextSubeffect();
    }
}
