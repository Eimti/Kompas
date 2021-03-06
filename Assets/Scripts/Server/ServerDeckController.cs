﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerDeckController : DeckController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        public override Player Owner => owner;
        public ServerPlayer owner;

        protected override bool AddCard(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            if (base.AddCard(card))
            {
                EffectsController.TriggerForCondition(Trigger.ToDeck, context);
                owner.ServerNotifier.NotifyDeckCount(Deck.Count);
                return true;
            }
            return false;
        }

        public override bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            if (base.PushBottomdeck(card, stackSrc))
            {
                EffectsController.TriggerForCondition(Trigger.Bottomdeck, context);
                ServerNotifier.NotifyBottomdeck(card, wasKnown);
                return true;
            }
            return false;
        }

        public override bool PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            if (base.PushTopdeck(card, stackSrc))
            {
                EffectsController.TriggerForCondition(Trigger.Topdeck, context);
                ServerNotifier.NotifyTopdeck(card, wasKnown);
                return true;
            }
            return false;
        }

        public override bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            if (base.ShuffleIn(card, stackSrc))
            {
                EffectsController.TriggerForCondition(Trigger.Reshuffle, context);
                ServerNotifier.NotifyReshuffle(card, wasKnown);
                return true;
            }
            return false;
        }

        public override bool RemoveFromDeck(GameCard card)
        {
            if(base.RemoveFromDeck(card))
            {
                owner.ServerNotifier.NotifyDeckCount(Deck.Count);
                return true;
            }
            return false;
        }
    }
}