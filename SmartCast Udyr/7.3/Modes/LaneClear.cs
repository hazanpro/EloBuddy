using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Settings;
using static SmartCast.SummonerSpells;
using static SmartCast.Utilities;

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

            int HealthWeak = Fight["MinionsAndMonsters.Weak"].Cast<Slider>().CurrentValue;

            SmiteSteal();
            SwitchStance(HealthWeak);
        }

        private static void SmiteSteal()
        {
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Super"]), Smites["Minion.Super"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Siege"]), Smites["Minion.Siege"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Melee"]), Smites["Minion.Melee"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMinion(Minions["Caster"]), Smites["Minion.Caster"].Cast<CheckBox>().CurrentValue);
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

        private static void SwitchStance(int HealthWeak)
        {
            if (Udyr.HasBuff(Buffs["R.Activation"]))
                return;
            else if (R.IsReady() && !TigerEffect())
                R.Cast();
            else if (!R.IsLearned && Q.IsReady())
                Q.Cast();
            else if (W.IsReady() && Udyr.Health <= HealthWeak && !TigerEffect() && !PhoenixEffect())
                W.Cast();

            Obj_AI_Minion target = targets.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsInRange(Udyr, Udyr.AttackRange)).FirstOrDefault();

            if (target == null)
                return;

            Orbwalker.ForcedTarget = target;
        }
    }
}
