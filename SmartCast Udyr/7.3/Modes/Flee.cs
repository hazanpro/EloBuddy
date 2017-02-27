using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.SummonerSpells;
using static SmartCast.Utilities;

namespace SmartCast.Modes
{
    internal class Flee : Mode
    {
        private static List<AIHeroClient> targets;

        internal static void Execute()
        {
            targets = new List<AIHeroClient>();
            targets = GetTarget(Target.Enemy);

            if (targets == null || targets.Count == 0)
                return;
            else if (KillSteal())
                return;
            else if (Stun())
                return;
            else if (Shield())
                return;
            else if (Escape())
                return;
        }

        private static bool KillSteal()
        {
            if (Ignite != null && Ignite.IsReady())
            {
                AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Ignite.Range) && !Enemy.HasUndyingBuff()).FirstOrDefault();

                if (target != null && target.Health <= SummonerSpells.Damage(target, Ignite.Slot))
                    Ignite.Cast(target);
            }
            else if (Smite != null && Smite.IsReady() && Smite.Name == Spells["Chilling Smite"])
            {
                AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Smite.Range)).FirstOrDefault();

                if (target != null && target.Health <= SummonerSpells.Damage(target, Smite.Slot))
                    Smite.Cast(target);
            }

            return false;
        }

        private static bool Stun()
        {
            if (!E.IsReady() && !Udyr.HasBuff(Buffs["E.Stance"]))
                return false;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => IsValid(Enemy, Udyr.AttackRange) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return false;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Shield()
        {
            if (!W.IsReady() || Udyr.HasBuff(Buffs["E.Activation"]))
                return false;
            else if (W.IsReady())
                W.Cast();

            return false;
        }

        private static bool Escape()
        {
            if (!E.IsReady())
                return false;

            E.Cast();
            return false;
        }
    }
}
