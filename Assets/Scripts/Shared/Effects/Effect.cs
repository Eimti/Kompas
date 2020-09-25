﻿using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
    /// <summary>
    /// Effects will only be resolved on server. Clients will just get to know what effects they can use
    /// </summary>
    public abstract class Effect : IStackable
    {
        public Game Game => Source.Game;

        public readonly int EffectIndex;
        public GameCard Source { get; }
        public abstract Player Controller { get; set; }

        //subeffects
        public abstract Subeffect[] Subeffects { get; }
        //current subeffect that's resolving
        public int SubeffectIndex { get; protected set; }
        public Subeffect CurrSubeffect => Subeffects[SubeffectIndex];

        //Targets
        public List<GameCard> Targets { get; } = new List<GameCard>();
        public List<(int x, int y)> Coords { get; private set; } = new List<(int x, int y)>();
        public List<GameCard> Rest { get; private set; } = new List<GameCard>();
        /// <summary>
        /// X value for card effect text (not coordinates)
        /// </summary>
        public int X = 0;

        //Triggering and Activating
        public abstract Trigger Trigger { get; }
        public ActivationRestriction ActivationRestriction { get; }
        public string Blurb { get; }
        public ActivationContext CurrActivationContext { get; protected set; }
        public int TimesUsedThisTurn { get; protected set; }
        public int TimesUsedThisRound { get; protected set; }
        public int TimesUsedThisStack { get; set; }

        private int negations = 0;
        public bool Negated
        {
            get => negations > 0;
            set
            {
                if (value) negations++;
                else negations--;
            }
        }

        public Effect(ActivationRestriction restriction, GameCard source, string blurb, int effIndex, Player owner)
        {
            Source = source ?? throw new System.ArgumentNullException($"Effect cannot be attached to null card");
            Controller = owner;
            ActivationRestriction = restriction;
            ActivationRestriction.Initialize(this);
            Blurb = blurb;
            EffectIndex = effIndex;
            TimesUsedThisTurn = 0;
        }

        public void ResetForTurn(Player turnPlayer)
        {
            TimesUsedThisTurn = 0;
            if (turnPlayer == Source.Controller) TimesUsedThisRound = 0;
        }

        public void Reset()
        {
            TimesUsedThisRound = 0;
            TimesUsedThisTurn = 0;
        }

        public virtual void Negate() => Negated = true;

        public virtual void AddTarget(GameCard card) => Targets.Add(card);

        public virtual bool CanBeActivatedBy(Player controller)
        {
            return Trigger == null
                && controller.index == Source.ControllerIndex
                && !Negated
                && ActivationRestriction.Evaluate(controller);
        }
    }
}