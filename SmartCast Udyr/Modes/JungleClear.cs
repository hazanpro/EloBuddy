using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Utilities;
using static SmartCast.SummonerSpells;

namespace SmartCast.Modes
{
    internal class JungleClear : Mode
    {
        private static List<Obj_AI_Minion> targets;

        internal static void Execute()
        {
            targets = new List<Obj_AI_Minion>();
            targets = GetTarget(Target.Monster);

            if (targets == null || targets.Count == 0)
                return;

            SmiteSteal();
            SwitchStance();
        }

        private static void SmiteSteal()
        {
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Baron Nashor"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Rift Herard"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Mountain Drake"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Blue Sentinel"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Red Brambleback"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Rift Scutller"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Gromp"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Crimson Raptor"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Raptor"]), false);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Geater Murk Wolf"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Murk Wolf"]), false);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Ancient Krug"]), true);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Krug"]), false);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Mini Krug"]), false);
        }

        private static Obj_AI_Minion GetMonster(string name)
        {
            Obj_AI_Minion target = targets.OrderBy(Monster => Monster.Health).Where(Monster => Monster.BaseSkinName == name).FirstOrDefault();

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
            else if (R.IsReady() && !TigerEffect() && targets.Count > 1)
                R.Cast();
            else if (!R.IsLearned && Q.IsReady())
                Q.Cast();

            string Name = targets.OrderByDescending(Monster => Monster.MaxHealth).FirstOrDefault().BaseSkinName;

            if (W.IsReady() && !TigerEffect() && !PhoenixEffect())
            {
                if (Name == Monsters["Baron Nashor"])
                    W.Cast();
                else if (Udyr.Health <= 1500 && (Name == Monsters["Rift Herard"] || Name == Monsters["Mountain Drake"]))
                    W.Cast();
                else if (Udyr.Health <= 1000)
                    W.Cast();
            }

            if (Q.IsReady() && !PhoenixEffect() && Q.Level >= R.Level)
                Q.Cast();

            Obj_AI_Minion target = targets.OrderByDescending(Monster => Monster.MaxHealth).OrderBy(Monster => Monster.Health).Where(Monster => Monster.IsInRange(Udyr, Udyr.AttackRange)).FirstOrDefault();

            if (target == null)
                return;

            Orbwalker.ForcedTarget = target;
        }
    }
}