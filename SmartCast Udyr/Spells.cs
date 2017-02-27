using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using static SmartCast.Program;

namespace SmartCast
{
    internal static class Spells
    {
        internal static Spell.Active Q { get; private set; }
        internal static Spell.Active W { get; private set; }
        internal static Spell.Active E { get; private set; }
        internal static Spell.Active R { get; private set; }

        internal static Dictionary<Champion, float> StunTimer { get; set; }

        internal static bool InQStance { get; private set; }
        internal static bool InWStance { get; private set; }
        internal static bool InEStance { get; private set; }
        internal static bool InRStance { get; private set; }

        internal static bool HasQActive { get; private set; }
        internal static bool HasWActive { get; private set; }
        internal static bool HasEActive { get; private set; }
        internal static bool HasRActive { get; private set; }

        internal static bool NextBasicIsPunchBleed { get; private set; }
        internal static bool NextBasicIsStunCycle { get; private set; }
        internal static bool NextBasicIsConeFlame { get; private set; }

        private static int PhoenixHits = 0;

        internal static void Initialize()
        {
            Q = new Spell.Active(SpellSlot.Q, 0, DamageType.Physical);
            W = new Spell.Active(SpellSlot.W, 0);
            E = new Spell.Active(SpellSlot.E, 0);
            R = new Spell.Active(SpellSlot.R, 250, DamageType.Magical);

            StunTimer = new Dictionary<Champion, float>(EntityManager.Heroes.Enemies.Count);

            foreach (AIHeroClient Enemy in EntityManager.Heroes.Enemies)
                StunTimer.Add(Enemy.Hero, 0);

            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
        }

        internal static bool IsEStunned(this AIHeroClient target)
        {
            return target.HasBuff("udyrbearstuncheck");
        }

        private static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args == null)
                return;
            else if (sender.IsMe && InRStance)
                NextBasicIsConeFlame = PhoenixHits++ % 3 == 0;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args == null)
                return;
            else if (sender.IsMe)
                UpdateStance(args.Slot);
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender == null || args == null)
                return;
            else if (sender.IsMe)
                UpdateActive();
            else if (sender.IsEnemy)
                BuffGained(sender, args.Buff.Name);
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender == null || args == null)
                return;
            else if (sender.IsMe)
                UpdateActive();
        }

        private static void UpdateStance(SpellSlot slot)
        {
            if (slot == SpellSlot.Q || slot == SpellSlot.W || slot == SpellSlot.E || slot == SpellSlot.R)
            {
                InQStance = Udyr.HasBuff("UdyrTigerStance");
                InWStance = Udyr.HasBuff("UdyrTurtleStance");
                InEStance = Udyr.HasBuff("UdyrBearStance");
                InRStance = Udyr.HasBuff("UdyrPhoenixStance");

                NextBasicIsPunchBleed = InQStance;
                NextBasicIsStunCycle = false;
                NextBasicIsConeFlame = InRStance;
                PhoenixHits = 0;
            }
        }

        private static void UpdateActive()
        {
            HasQActive = Udyr.HasBuff("udyrtigerpunch");
            HasWActive = Udyr.HasBuff("udyrturtleactivation");
            HasEActive = Udyr.HasBuff("udyrbearactivation");
            HasRActive = Udyr.HasBuff("UdyrPhoenixActivation");
    }

        private static void BuffGained(Obj_AI_Base target, string buff)
        {
            if (buff == "UdyrTigerPunchBleed")
                NextBasicIsPunchBleed = false;
            else if (buff == "udyrbearstuncheck" && !target.IsMinion && !target.IsMonster)
            {
                NextBasicIsStunCycle = true;
                StunTimer[(target as AIHeroClient).Hero] = Game.Time;
            }
        }
    }
}