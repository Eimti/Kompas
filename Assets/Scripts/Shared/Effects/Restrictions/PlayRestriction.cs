﻿using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class PlayRestriction
    {
        public GameCard Card { get; private set; }

        public const string PlayedByCardOwner = "Played By Card Owner";
        public const string FromHand = "From Hand";
        public const string StandardPlayRestriction = "Adjacent to Friendly Card";
        public const string StandardSpellRestriction = "Not Adjacent to Other Spell";
        public const string FriendlyTurnIfNotFast = "Friendly Turn";
        public const string HasCostInPips = "Has Cost in Pips";
        public const string FastOrNothingIsResolving = "Nothing is Resolving";
        public const string CheckUnique = "Check Unique";

        public const string NotNormally = "Cannot be Played Normally";
        public const string MustNormally = "Must be Played Normally";
        public const string OnBoardCardFriendlyOrAdjacent = "On Board Card";
        public const string OnCardFittingRestriction = "On Card that Fits Restriction";

        public const string DefaultNormal = "Default Normal Restrictions";
        public const string DefaultEffect = "Default Effect Restrictions";
        public static readonly string[] DefaultNormalRestrictions =
            { PlayedByCardOwner, FromHand, StandardPlayRestriction, StandardSpellRestriction, 
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] DefaultEffectRestrictions = { StandardSpellRestriction, StandardPlayRestriction, CheckUnique };

        public const string AugNormal = "Augment Normal Restrictions";
        public const string AugEffect = "Augment Effect Restrictions";
        public static readonly string[] AugmentNormalRestrictions =
            { PlayedByCardOwner, FromHand, OnBoardCardFriendlyOrAdjacent, StandardSpellRestriction, 
            FriendlyTurnIfNotFast, HasCostInPips, FastOrNothingIsResolving, CheckUnique };
        public static readonly string[] AugmentEffectRestrictions = { StandardSpellRestriction, OnBoardCardFriendlyOrAdjacent, CheckUnique };

        public List<string> normalRestrictions = null;
        public List<string> effectRestrictions = null;

        public CardRestriction onCardRestriction;

        public void SetInfo(GameCard card)
        {
            Card = card;

            normalRestrictions = normalRestrictions ?? new List<string> { DefaultNormal };
            effectRestrictions = effectRestrictions ?? new List<string> { DefaultEffect };

            if (normalRestrictions.Contains(DefaultNormal)) normalRestrictions.AddRange(DefaultNormalRestrictions);
            if (normalRestrictions.Contains(AugNormal)) normalRestrictions.AddRange(AugmentNormalRestrictions);

            if (effectRestrictions.Contains(DefaultEffect)) effectRestrictions.AddRange(DefaultEffectRestrictions);
            if (effectRestrictions.Contains(AugEffect)) effectRestrictions.AddRange(AugmentEffectRestrictions);

            onCardRestriction = onCardRestriction ?? new CardRestriction();
            onCardRestriction.Initialize(card, card.Controller, null);
            Debug.Log($"Finished setting info for play restriction of card {card.CardName}");
        }

        private bool RestrictionValid(string r, int x, int y, Player player, bool normal)
        {
            switch (r)
            {
                case DefaultNormal:
                case DefaultEffect:
                case AugNormal:
                case AugEffect:
                    return true; //they're covered by the restrictions added on their behalf

                case PlayedByCardOwner: return player == Card.Owner;
                case FromHand: return Card.Location == CardLocation.Hand;
                case StandardPlayRestriction: return Card.Game.ValidStandardPlaySpace(x, y, Card.Controller);
                case StandardSpellRestriction: return Card.Game.ValidSpellSpaceFor(Card, x, y);
                case HasCostInPips: return Card.Controller.Pips >= Card.Cost;
                case FriendlyTurnIfNotFast: return Card.Fast || Card.Game.TurnPlayer == Card.Controller;
                case FastOrNothingIsResolving: return Card.Fast || Card.Game.NothingHappening;

                case OnBoardCardFriendlyOrAdjacent:
                    var cardThere = Card.Game.boardCtrl.GetCardAt(x, y);
                    return cardThere != null 
                        && (cardThere.Controller == Card.Controller || cardThere.AdjacentCards.Any(c => c.Controller == Card.Controller));
                case OnCardFittingRestriction:
                    onCardRestriction.Initialize(Card, player, null);
                    return onCardRestriction.Evaluate(Card.Game.boardCtrl.GetCardAt(x, y));
                case NotNormally: return !normal;
                case MustNormally: return normal;
                case CheckUnique: return !(Card.Unique && Card.AlreadyCopyOnBoard);

                default: throw new System.ArgumentException($"You forgot to check play restriction {r}", "r");
            }
        }

        public bool EvaluateNormalPlay(int x, int y, Player player, bool checkCanAffordCost = false)
            => (!checkCanAffordCost || player.Pips >= Card.Cost) 
            && normalRestrictions.All(r => RestrictionValid(r, x, y, player, true));

        public bool EvaluateEffectPlay(int x, int y, Effect effect, Player controller)
            => effectRestrictions.All(r => RestrictionValid(r, x, y, controller, false));

        public bool EvaluateEffectPlay(int x, int y, Effect effect)
            => EvaluateEffectPlay(x, y, effect, effect.Controller);
    }
}