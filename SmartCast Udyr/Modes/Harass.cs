using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.SummonerSpells;
using static SmartCast.Utilities;

namespace SmartCast.Modes
{
    internal class Harass : Mode
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
            else if (Attack())
                return;
            else if (Basic())
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
                AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Smite.Range) && Enemy.Distance(Udyr) > 450 && !Enemy.HasBuffOfType(BuffType.Stun)).FirstOrDefault();

                if (target != null && target.Health <= SummonerSpells.Damage(target, Smite.Slot))
                    Smite.Cast(target);
            }

            return false;
        }

        private static bool Stun()
        {
            if (!E.IsReady() && !Udyr.HasBuff(Buffs["E.Stance"]))
                return false;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => IsValid(Enemy, 600) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return false;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Shield()
        {
            if (!W.IsReady() || TigerEffect() && PhoenixEffect())
                return false;

            bool TigerUdyr = W.Level <= Q.Level && R.Level < W.Level;
            bool PhoenixUdyr = W.Level <= R.Level && Q.Level < W.Level;
            bool HybridUdyr = W.Level <= Q.Level && W.Level <= R.Level;

            bool CastQ = TigerUdyr && Q.IsReady();
            bool CastR = PhoenixUdyr && R.IsReady();
            bool CastQorR = HybridUdyr && (Q.IsReady() || R.IsReady()) && Udyr.HealthPercent > 65;

            if (CastQ || CastR || CastQorR)
                return false;

            W.Cast();
            return true;
        }

        private static bool Attack()
        {
            if (!Q.IsReady() && !R.IsReady())
                return false;

            bool TigerUdyr = W.Level <= Q.Level && R.Level < W.Level;
            bool PhoenixUdyr = W.Level <= R.Level && Q.Level < W.Level;
            bool HybridUdyr = W.Level <= Q.Level && W.Level <= R.Level;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, 600) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return false;
            else if (Q.IsReady() && (TigerUdyr || HybridUdyr) && !PhoenixEffect())
                Q.Cast();
            else if (R.IsReady() && (PhoenixUdyr || HybridUdyr) && !TigerEffect())
                R.Cast();

            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Basic()
        {
            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, 600) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return false;

            Orbwalker.ForcedTarget = target;
            return true;
        }
    }
}