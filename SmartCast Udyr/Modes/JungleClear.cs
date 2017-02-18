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
    internal class JungleClear : Mode
    {
        private static List<Obj_AI_Minion> targets;

        internal static void Execute()
        {
            targets = new List<Obj_AI_Minion>();
            targets = GetTarget(Target.Monster);

            if (targets == null || targets.Count == 0)
                return;

            int HealthStrong = Fight["MinionsAndMonsters.Strong"].Cast<Slider>().CurrentValue;
            int HealthWeak = Fight["MinionsAndMonsters.Weak"].Cast<Slider>().CurrentValue;

            SmiteSteal();
            SwitchStance(HealthStrong, HealthWeak);
        }

        private static void SmiteSteal()
        {
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Baron Nashor"]), Smites["Monster.BaronNashor"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Rift Herard"]), Smites["Monster.RiftHerard"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Mountain Drake"]), Smites["Monster.MountainDrake"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Blue Sentinel"]), Smites["Monster.BlueSentinel"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Red Brambleback"]), Smites["Monster.RedBrambleback"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Rift Scutller"]), Smites["Monster.RiftScutller"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Gromp"]), Smites["Monster.Gromp"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Crimson Raptor"]), Smites["Monster.CrimsonRaptor"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Raptor"]), Smites["Monster.Raptor"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Greater Murk Wolf"]), Smites["Monster.GreaterMurkWolf"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Murk Wolf"]), Smites["Monster.MurkWolf"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Ancient Krug"]), Smites["Monster.AncientKrug"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Krug"]), Smites["Monster.Krug"].Cast<CheckBox>().CurrentValue);
            if (Smite.IsReady())
                Killable(GetMonster(Monsters["Mini Krug"]), Smites["Monster.MiniKrug"].Cast<CheckBox>().CurrentValue);
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

        private static void SwitchStance(int HealthStrong, int HealthWeak)
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
                else if (Udyr.Health <= HealthStrong && (Name == Monsters["Rift Herard"] || Name == Monsters["Mountain Drake"]))
                    W.Cast();
                else if (Udyr.Health <= HealthWeak)
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