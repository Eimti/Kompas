﻿using KompasCore.Cards;
using KompasServer.Effects;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class TriggerRestriction
    {
        public Subeffect Subeffect { get; private set; }

        public const string ThisCardTriggered = "This Card Triggered"; //0,

        //todo deprecate this card in play to this card fits restriction
        public const string ThisCardInPlay = "This Card in Play"; //1,
        public const string AugmentedCardTriggered = "Augmented Card Triggered"; //10,

        public const string ThisCardFitsRestriction = "This Card Fits Restriction"; //100,
        public const string TriggererFitsRestriction = "Triggerer Fits Restriction"; //101,
        public const string AdjacentToRestriction = "Adjacent to Restriction";
        public const string CoordsFitRestriction = "Coords Fit Restriction"; //120,
        public const string XFitsRestriction = "X Fits Restriction"; //130,
        public const string StackableSourceFitsRestriction = "Stackable Source Fits Restriction";
        public const string EffectSourceIsThisCard = "Stackable Source is This Card"; //140,
        public const string EffectSourceIsTriggerer = "Stackable Source is Triggerer"; //149,

        public const string DistanceTriggererToSpaceConstant = "Distance from Triggerer to Space == Constant";

        public const string ControllerTriggered = "Controller Triggered"; //200,
        public const string EnemyTriggered = "Enemy Triggered"; //201,

        //note: turns are the exception to the rule in that they are the only triggers where the triggering event
        //(in their case, the turn passing)
        //happens before the trigger is called, 
        //instead of the trigger being called at the moment before it happens.
        //in short, note that the turn will pass, then the trigger for turn start is called.
        //this means that checking for friendly/enemy turn will check whose turn the current (just-changed-to) turn is.
        public const string FriendlyTurn = "Friendly Turn"; //300,
        public const string EnemyTurn = "Enemy Turn"; //301,

        public const string FromField = "From Field"; //400,
        public const string FromDeck = "From Deck"; //401,

        public const string MaxPerTurn = "Max Per Turn"; //500,
        public const string NotFromEffect = "Not From Effect"; //501,
        public const string MaxPerRound = "Max Per Round"; //502
        public const string MaxPerStack = "Max Per Stack";

        public static readonly string[] ReevalationRestrictions = { MaxPerTurn, MaxPerRound, MaxPerStack };

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction = new CardRestriction();
        public XRestriction xRestriction = new XRestriction();
        public SpaceRestriction spaceRestriction = new SpaceRestriction();
        public CardRestriction sourceRestriction = new CardRestriction();
        public int maxTimesPerTurn = 1;
        public int maxPerRound = 1;
        public int maxPerStack = 1;
        public int distance = 1;

        public GameCard ThisCard { get; private set; }

        public ServerTrigger ThisTrigger { get; private set; }

        public void Initialize(ServerSubeffect subeff, GameCard thisCard, ServerTrigger thisTrigger)
        {
            Subeffect = subeff;
            cardRestriction.Initialize(subeff);
            xRestriction.Initialize(subeff);
            spaceRestriction.Initialize(subeff);
            this.ThisCard = thisCard;
            this.ThisTrigger = thisTrigger;
        }

        private bool RestrictionValid(string restriction, ActivationContext context)
        {
            switch (restriction)
            {
                //card triggering stuff
                case ThisCardTriggered:        return context.Card == ThisCard;
                case ThisCardInPlay:           return ThisCard.Location == CardLocation.Field;
                case AugmentedCardTriggered:   return context.Card == ThisCard.AugmentedCard;
                case ThisCardFitsRestriction:  return cardRestriction.Evaluate(ThisCard);
                case TriggererFitsRestriction: return cardRestriction.Evaluate(context.Card);
                case StackableSourceFitsRestriction: return sourceRestriction.Evaluate(context.Stackable?.Source);
                
                //other non-card triggering things
                case CoordsFitRestriction:    return context.Space != null && spaceRestriction.Evaluate(context.Space.Value);
                case XFitsRestriction:        return context.X != null && xRestriction.Evaluate(context.X.Value);
                case EffectSourceIsTriggerer: return context.Stackable is Effect eff && eff.Source == context.Card;
                case AdjacentToRestriction:   return ThisCard.AdjacentCards.Any(c => cardRestriction.Evaluate(c));
                //TODO make these into just something to do with triggered card fitting restriction
                case ControllerTriggered:     return context.Triggerer == ThisCard.Controller;
                case EnemyTriggered:          return context.Triggerer != ThisCard.Controller;

                case DistanceTriggererToSpaceConstant:
                    if (context.Space == null) return false;
                    return context.Card.DistanceTo(context.Space.Value) == distance;

                //gamestate
                case FriendlyTurn:  return Subeffect.Game.TurnPlayer == ThisCard.Controller;
                case EnemyTurn:     return Subeffect.Game.TurnPlayer != ThisCard.Controller;
                case FromField:     return context.Card.Location == CardLocation.Field;
                case FromDeck:      return context.Card.Location == CardLocation.Deck;
                case NotFromEffect: return context.Stackable is Effect;

                //max
                case MaxPerRound: return ThisTrigger.serverEffect.TimesUsedThisRound < maxPerRound;
                case MaxPerTurn:  return ThisTrigger.serverEffect.TimesUsedThisTurn < maxTimesPerTurn;
                case MaxPerStack: return ThisTrigger.serverEffect.TimesUsedThisStack < maxPerStack;

                //misc
                default: throw new System.ArgumentException($"Invalid trigger restriction {restriction}");
            }
        }

        public bool Evaluate(ActivationContext context) => triggerRestrictions.All(r => RestrictionValid(r, context));

        /// <summary>
        /// Reevaluates the trigger to check that any restrictions that could change between it being triggered
        /// and it being ordered on the stack, are still true.
        /// </summary>
        /// <returns></returns>
        public bool Reevaluate(ActivationContext context)
            => triggerRestrictions.Intersect(ReevalationRestrictions).All(r => RestrictionValid(r, context));
    }
}