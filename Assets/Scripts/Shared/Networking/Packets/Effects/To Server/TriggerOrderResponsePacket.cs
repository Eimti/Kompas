﻿using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Linq;
using Boo.Lang;
using KompasCore.Effects;

namespace KompasCore.Networking
{
    public class TriggerOrderResponsePacket : Packet
    {
        public int[] cardIds;
        public int[] effIndices;
        public int[] orders;

        public TriggerOrderResponsePacket() : base(ChooseTriggerOrder) { }

        public TriggerOrderResponsePacket(int[] cardIds, int[] effIndices, int[] orders) : this()
        {
            this.cardIds = cardIds;
            this.effIndices = effIndices;
            this.orders = orders;
        }

        public override Packet Copy() => new TriggerOrderResponsePacket(cardIds, effIndices, orders);
    }
}

namespace KompasServer.Networking
{
    public class TriggerOrderResponseServerPacket : TriggerOrderResponsePacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            for(int i = 0; i < cardIds.Length; i++)
            {
                var card = serverGame.GetCardWithID(cardIds[i]);
                if (card == null) continue;
                if (card.Effects.ElementAt(effIndices[i]).Trigger is ServerTrigger trigger) 
                    trigger.Order = orders[i];
            }
            serverGame.EffectsController.CheckForResponse();
        }
    }
}