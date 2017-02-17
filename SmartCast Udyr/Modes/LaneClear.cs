using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Utilities;
using static SmartCast.SummonerSpells;

namespace SmartCast.Modes
{
    internal class LaneClear : Mode
    {
        private static List<Obj_AI_Minion> targets;

        internal static void Execute()
        {
            targets = new List<Obj_AI_Minion>();
            targets = GetTarget(Target.Minion);

            if (targets == null || targets.Count == 0)
                return;

            SmiteSteal();
            SwitchStance();
        }

        private static void SmiteSteal()
        {
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Super"]), true);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Siege"]), true);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Melee"]), false);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Caster"]), false);
        }

        private static Obj_AI_Minion GetMinion(string name)
        {
            Obj_AI_Minion target = targets.OrderBy(Minion => Minion.Health).Where(Minion => Minion.BaseSkinName == name).FirstOrDefault();

            if (target == null)
                return null;

            return target;
        }

        private static void Killable(Obj_AI_Minion target, bool smite)
        {
            if (target == null || smite == false)
                return;
            else if (target.Health <= SummonerSpells.Damage(target, Smite.Slot))
                Smite.Cast(target);
        }

        private static void SwitchStance()
        {
            if (Udyr.HasBuff(Buffs["R.Activation"]))
                return;
            else if (R.IsReady() && !TigerEffect())
                R.Cast();
            else if (!R.IsLearned && Q.IsReady())
                Q.Cast();
            else if (W.IsReady() && Udyr.Health <= 1000 && !TigerEffect() && !PhoenixEffect())
                W.Cast();

            Obj_AI_Minion target = targets.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsInRange(Udyr, Udyr.AttackRange)).FirstOrDefault();

            if (target == null)
                return;

            Orbwalker.ForcedTarget = target;
        }
    }
}