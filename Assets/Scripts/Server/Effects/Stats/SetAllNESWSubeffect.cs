﻿using System.Linq;

public class SetAllNESWSubeffect : SetNESWSubeffect
{
    private (int, int, int, int) GetRealValues(GameCard c)
    {
        (int n, int e, int s, int w) = (
            NVal >= 0 ? NVal : c.N,
            EVal >= 0 ? EVal : c.E,
            SVal >= 0 ? SVal : c.S,
            WVal >= 0 ? WVal : c.W
        );
        return (n, e, s, w);
    }

    //default to making sure things are characters before changing their stats
    public CardRestriction cardRestriction = new CardRestriction()
    {
        restrictions = new string[]
        {
            CardRestriction.IsCharacter,
            CardRestriction.Board
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    public override bool Resolve()
    {
        var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
        foreach (ServerGameCard c in targets)
        {
            var (n, e, s, w) = GetRealValues(c);
            c.SetN(e, ServerEffect);
            c.SetE(e, ServerEffect);
            c.SetE(e, ServerEffect);
            c.SetE(e, ServerEffect);
        }
        return ServerEffect.ResolveNextSubeffect();
    }
}
