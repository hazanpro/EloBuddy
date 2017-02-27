using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using static EloBuddy.SDK.Spells.SummonerSpells;
using static SmartCast.Damages;
using static SmartCast.Program;
using static SmartCast.Settings;

namespace SmartCast.Logic
{
    internal class SummonerSpells
    {
        internal static Dictionary<string, string> Minion = GetMinions();
        internal static Dictionary<string, string> Monster = GetMonsters();

        #region Helpers

        private static Dictionary<string, string> GetMinions()
        {
            return new Dictionary<string, string>()
            {
                { "Melee", "SRU_ChaosMinionMelee" },
                { "Caster", "SRU_ChaosMinionRanged" },
                { "Siege", "SRU_ChaosMinionSiege" },
                { "Super", "SRU_ChaosMinionSuper" }
            };
        }

        private static Dictionary<string, string> GetMonsters()
        {
            return new Dictionary<string, string>()
            {
                { "Baron Nashor", "SRU_Baron" },
                { "Rift Herard", "SRU_RiftHerard" },
                { "Mountain Drake", "SRU_Dragon_Earth" },
                { "Rift Scutller", "Sru_Crab" },
                { "Blue Sentinel", "SRU_Blue" },
                { "Red Brambleback", "SRU_Red" },
                { "Gromp", "SRU_Gromp" },
                { "Crimson Raptor", "SRU_Razorbeak" },
                { "Raptor", "SRU_RazorbeakMini" },
                { "Greater Murk Wolf", "SRU_Murkwolf" },
                { "Murk Wolf", "SRU_MurkwolfMini" },
                { "Ancient Krug", "SRU_Krug" },
                { "Krug", "SRU_KrugMini" },
                { "Mini Krug", "SRU_KrugMiniMini" },
                { "Blast Cone", "SRU_Planet_Satchel" },
                { "Honey Fruit", "SRU_Planet_Health" },
                { "Scryer's Bloom", "SRU_Planet_Vision" }
            };
        }

        #endregion

        #region Ignite

        internal static void UseIgnite(Effect effect)
        {
            if (!Ignite.IsReady())
                return;
            else if (effect == Effect.Killsteal)
                KillstealByIgnite();
        }

        private static void KillstealByIgnite()
        {
            List<AIHeroClient> targets = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(Ignite.Range) && !Enemy.HasUndyingBuff()).ToList();

            if (targets == null || targets.Count == 0)
                return;

            AIHeroClient target = targets.FirstOrDefault();

            if (!Ignite.Killable(target))
                return;

            int allies = EntityManager.Heroes.Allies.Where(Ally => Ally.IsInRange(Udyr, FULL_RANGE) && !Ally.IsMe).Count();
            int enemies = targets.Count;

            if (Udyr.HealthPercent >= KILLSTEAL_HEALTH && allies == 0 && enemies == 1)
                return;

            Ignite.Cast(target);
        }

        #endregion

        #region Smite

        internal static void UseSmite(Effect effect, Mode mode = Mode.None)
        {
            if (!Smite.IsReady())
                return;
            else if (effect == Effect.Heal)
                HealBySmite();
            else if (mode == Mode.JungleClear && effect == Effect.Killsteal)
                KillstealMonsterBySmite();
            else if (mode == Mode.LaneClear && effect == Effect.Killsteal)
                KillstealMinionBySmite();
        }

        private static void KillstealMinionBySmite()
        {
            Dictionary<string, bool> minions = new Dictionary<string, bool>();

            minions.Add(Minion["Super"], SmiteMenu["Minion.Super"].Cast<CheckBox>().CurrentValue);
            minions.Add(Minion["Siege"], SmiteMenu["Minion.Siege"].Cast<CheckBox>().CurrentValue);
            minions.Add(Minion["Melee"], SmiteMenu["Minion.Melee"].Cast<CheckBox>().CurrentValue);
            minions.Add(Minion["Caster"], SmiteMenu["Minion.Caster"].Cast<CheckBox>().CurrentValue);

            foreach (KeyValuePair<string, bool> minion in minions)
            {
                Obj_AI_Minion target = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(Minion => Minion.Health).Where(Minion => Minion.BaseSkinName == minion.Key && minion.Value).FirstOrDefault();

                if (target == null)
                    continue;
                else if (Smite.Killable(target))
                    Smite.Cast(target);

                Smite.Cast(target);
                break;
            }
        }

        private static void KillstealMonsterBySmite()
        {
            Dictionary<string, bool> monsters = new Dictionary<string, bool>();

            monsters.Add(Monster["Baron Nashor"], SmiteMenu["Monster.BaronNashor"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Rift Herard"], SmiteMenu["Monster.RiftHerard"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Mountain Drake"], SmiteMenu["Monster.MountainDrake"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Blue Sentinel"], SmiteMenu["Monster.BlueSentinel"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Red Brambleback"], SmiteMenu["Monster.RedBrambleback"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Rift Scutller"], SmiteMenu["Monster.RiftScutller"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Gromp"], SmiteMenu["Monster.Gromp"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Crimson Raptor"], SmiteMenu["Monster.CrimsonRaptor"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Raptor"], SmiteMenu["Monster.Raptor"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Greater Murk Wolf"], SmiteMenu["Monster.GreaterMurkWolf"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Murk Wolf"], SmiteMenu["Monster.MurkWolf"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Ancient Krug"], SmiteMenu["Monster.AncientKrug"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Krug"], SmiteMenu["Monster.Krug"].Cast<CheckBox>().CurrentValue);
            monsters.Add(Monster["Mini Krug"], SmiteMenu["Monster.MiniKrug"].Cast<CheckBox>().CurrentValue);

            foreach (KeyValuePair<string, bool> monser in monsters)
            {
                Obj_AI_Minion target = EntityManager.MinionsAndMonsters.Monsters.OrderBy(Monster => Monster.Health).Where(Monster => Monster.BaseSkinName == monser.Key && monser.Value).FirstOrDefault();

                if (target == null)
                    continue;
                else if (Smite.Killable(target))
                    Smite.Cast(target);
                break;
            }
        }

        private static void HealBySmite()
        {
            if (Udyr.Health >= SMITE_HEALTH)
                return;

            Obj_AI_Minion target = EntityManager.MinionsAndMonsters.CombinedAttackable.OrderByDescending(Target => Target.MaxHealth).Where(Target => Target.IsValidTarget(Smite.Range)).FirstOrDefault();

            if (target != null)
                Smite.Cast(target);
        }

        #endregion

        #region Chilling Smite

        internal static void UseChillingSmite(Effect effect, Mode mode = Mode.None)
        {
            if (!Smite.IsReady() || !Smite.Type(SmiteType.Chilling))
                return;
            else if (effect == Effect.Killsteal)
                KillstealByChillingSmite();
            else if (effect == Effect.Slow && mode == Mode.Combo)
                ComboSlowByChillingSmite();
            else if (effect == Effect.Slow && mode == Mode.Flee)
                ComboSlowByChillingSmite();
        }

        private static void KillstealByChillingSmite()
        {
            List<AIHeroClient> targets = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(Smite.Range) && !Enemy.HasUndyingBuff()).ToList();

            if (targets == null || targets.Count == 0)
                return;

            AIHeroClient target = targets.FirstOrDefault();

            if (!Ignite.Killable(target))
                return;

            int allies = EntityManager.Heroes.Allies.Where(Ally => Ally.IsInRange(Udyr, FULL_RANGE) && !Ally.IsMe).Count();
            int enemies = targets.Count;

            if (Udyr.HealthPercent >= KILLSTEAL_HEALTH && allies == 0 && enemies == 1)
                return;

            Smite.Cast(target);
        }

        private static void ComboSlowByChillingSmite()
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(Smite.Range) && Enemy.Distance(Udyr) >= SLOW_DISTANCE && !Enemy.HasBuffOfType(BuffType.Stun) && !Enemy.IsFacing(Udyr)).FirstOrDefault();

            if (target != null)
                Smite.Cast(target);
        }

        private static void FleeSlowByChillingSmite()
        {
            if (Udyr.HealthPercent >= CRITICAL_CHAMPIONS_HEALTH)
                return;

            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => Enemy.IsValidTarget(Smite.Range) && !Enemy.HasBuffOfType(BuffType.Stun) && Enemy.IsFacing(Udyr)).FirstOrDefault();

            if (target == null)
                return;

            Smite.Cast(target);
        }

        #endregion

        #region Challenging Smite

        internal static void UseChallengingSmite(Mode mode)
        {
            if (!Smite.IsReady() || !Smite.Type(SmiteType.Challenging))
                return;
            else if (mode == Mode.Combo)
                BonusTrueDamageByChallengingSmite();
            else if (mode == Mode.Flee)
                DamageReduceByChallengingSmite();
        }

        private static void BonusTrueDamageByChallengingSmite()
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(Udyr.AttackRange) && !Enemy.HasUndyingBuff() && Enemy.HasBuffOfType(BuffType.Stun)).FirstOrDefault();

            if (target == null)
                return;

            Smite.Cast(target);
            Orbwalker.ForcedTarget = target;
        }

        private static void DamageReduceByChallengingSmite()
        {
            if (Udyr.HealthPercent >= CRITICAL_CHAMPIONS_HEALTH)
                return;

            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => Enemy.IsValidTarget(Smite.Range) && !Enemy.HasBuffOfType(BuffType.Stun) && Enemy.IsFacing(Udyr)).FirstOrDefault();

            if (target == null)
                return;

            Smite.Cast(target);
        }

        #endregion
    }
}