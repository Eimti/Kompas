﻿using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TriggerDummySubeffect : ServerSubeffect
    {
        public TriggerDummySubeffect(ServerEffect eff)
        {
            this.ServerEffect = eff;
        }

        public override Task<ResolutionInfo> Resolve()
        {
            throw new System.NotImplementedException("Trigger Dummy Subeffect only exists so that card restriction of trigger restriction has an effect to point to." +
                "It should never resolve.");
        }
    }
}