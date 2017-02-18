using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using System.Collections.Generic;
using static SmartCast.Abilities;
using static SmartCast.Settings;
using static SmartCast.SummonerSpells;
using static SmartCast.Utilities;

namespace SmartCast.Modes
{
    internal class Combo : Mode
    {
        private static List<AIHeroClient> targets;

        internal static void Execute()
        {
            targets = new List<AIHeroClient>();
            targets = GetTarget(Target.Enemy);

            if (targets == null || targets.Count == 0)
                return;

            int Range = Fight["Enemies.Range"].Cast<Slider>().CurrentValue;
            int Health = Fight["Enemies.Health"].Cast<Slider>().CurrentValue;

            if (KillSteal())
                return;
            else if (Slow())
                return;
            else if (Stun(Range))
                return;
            else if (Shield(Health))
                return;
            else if (Attack(Range))
                return;
            else if (Damage())
                return;
            else if (Basic(Range))
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
                int Distance = Smites["Chilling.Distance"].Cast<Slider>().CurrentValue;
                AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Smite.Range) && Enemy.Distance(Udyr) > Distance && !Enemy.HasBuffOfType(BuffType.Stun)).FirstOrDefault();

                if (target != null && target.Health <= SummonerSpells.Damage(target, Smite.Slot))
                    Smite.Cast(target);
            }

            return false;
        }

        private static bool Slow()
        {
            if (Smite == null || !Smite.IsReady() || Smite.Name != Spells["Chilling Smite"])
                return false;

            int Distance = Smites["Chilling.Distance"].Cast<Slider>().CurrentValue;
            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Smite.Range) && Enemy.Distance(Udyr) > Distance && !Enemy.HasBuffOfType(BuffType.Stun) && !Enemy.IsFacing(Udyr)).FirstOrDefault();

            if (target == null)
                return false;

            Smite.Cast(target);
            return false;
        }

        private static bool Stun(int Range)
        {
            if (!E.IsReady() && !Udyr.HasBuff(Buffs["E.Stance"]))
                return false;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => IsValid(Enemy, Range) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return false;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Shield(int Health)
        {
            if (!W.IsReady() || TigerEffect() && PhoenixEffect())
                return false;

            bool TigerUdyr = W.Level <= Q.Level && R.Level < W.Level;
            bool PhoenixUdyr = W.Level <= R.Level && Q.Level < W.Level;
            bool HybridUdyr = W.Level <= Q.Level && W.Level <= R.Level;

            bool CastQ = TigerUdyr && Q.IsReady();
            bool KeepWithQ = Udyr.HasBuff(Buffs["Q.Activation"]);

            bool CastR = PhoenixUdyr && R.IsReady();
            bool KeepWithR = Udyr.HasBuff(Buffs["R.Activation"]);

            bool CastQorR = HybridUdyr && (Q.IsReady() || R.IsReady()) && Udyr.HealthPercent > Health;
            bool KeepWithQorR = Udyr.HasBuff(Buffs["Q.Activation"]) || Udyr.HasBuff(Buffs["R.Activation"]);

            if (CastQ || KeepWithQ || CastR || KeepWithR || CastQorR || KeepWithQorR)
                return false;

            W.Cast();
            return true;
        }

        private static bool Attack(int Range)
        {
            if (!Q.IsReady() && !R.IsReady())
                return false;

            bool TigerUdyr = W.Level <= Q.Level && R.Level < W.Level;
            bool PhoenixUdyr = W.Level <= R.Level && Q.Level < W.Level;
            bool HybridUdyr = W.Level <= Q.Level && W.Level <= R.Level;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Range) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return false;
            else if (Q.IsReady() && (TigerUdyr || HybridUdyr) && !PhoenixEffect())
                Q.Cast();
            else if (R.IsReady() && (PhoenixUdyr || HybridUdyr) && !TigerEffect())
                R.Cast();

            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Damage()
        {
            if (Smite == null || !Smite.IsReady() || Smite.Name != Spells["Challenging Smite"])
                return false;

            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Smite.Range) && !Enemy.HasUndyingBuff() && Enemy.HasBuffOfType(BuffType.Stun)).FirstOrDefault();

            if (target == null)
                return false;

            Smite.Cast(target);
            Orbwalker.ForcedTarget = target;
            return true;
        }

        private static bool Basic(int Range)
        {
            AIHeroClient target = targets.OrderBy(Enemy => Enemy.Health).Where(Enemy => IsValid(Enemy, Range) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return false;

            Orbwalker.ForcedTarget = target;
            return true;
        }
    }
}
