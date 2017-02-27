using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Utilities;

namespace SmartCast.Modes
{
    internal class LastHit : Mode
    {
        private static List<Obj_AI_Minion> targets;

        internal static void Execute()
        {
            targets = new List<Obj_AI_Minion>();
            targets = GetTarget(Target.Minion);

            if (targets == null || targets.Count == 0)
                return;

            SwitchStance();
        }

        private static void SwitchStance()
        {
            if (Udyr.HasBuff(Buffs["Q.Stance"]) || Udyr.HasBuff(Buffs["R.Activation"]))
                return;
            else if (Q.IsReady())
                Q.Cast();

            Obj_AI_Minion target = targets.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsInRange(Udyr, Udyr.AttackRange)).FirstOrDefault();

            if (target == null)
                return;

            Orbwalker.ForcedTarget = target;
        }
    }
}
