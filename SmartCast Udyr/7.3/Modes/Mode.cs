using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Items;
using static SmartCast.Settings;
using static SmartCast.Utilities;

namespace SmartCast.Modes
{
    internal class Mode
    {
        internal enum Target
        {
            Enemy,
            Minion,
            Monster
        }

        private static void CriticalShield(int Enemies, int Minions, int Monsters)
        {
            int HealthVsEnemies = Danger["Enemies.Health"].Cast<Slider>().CurrentValue;
            int HealthVsMinionsAndMonsters = Danger["MinionsAndMonsters.Health"].Cast<Slider>().CurrentValue;

            if (!W.IsReady())
                return;
            else if (Udyr.HealthPercent <= HealthVsEnemies && Enemies > 0)
                W.Cast();
            else if (Udyr.Health <= HealthVsMinionsAndMonsters && (Minions > 0 || Monsters > 0))
                W.Cast();
        }

        internal static bool IsValid(AIHeroClient enemy, float range)
        {
            if (enemy.IsDead || enemy.IsInvulnerable || !enemy.IsTargetable || !enemy.IsValidTarget(range))
                return false;

            return true;
        }

        internal static bool Stunable(AIHeroClient target)
        {
            return !target.HasBuff(Buffs["E.Stun"]) && StunTimer[target.Hero] + 6 < Game.Time;
        }

        internal static dynamic GetTarget(Target target)
        {
            int RangeVsEnemies = Danger["Enemies.Range"].Cast<Slider>().CurrentValue;
            int RangeVsMinionsAndMonsters = Danger["MinionsAndMonsters.Range"].Cast<Slider>().CurrentValue;

            List<AIHeroClient> Enemies = new List<AIHeroClient>();
            Enemies = EntityManager.Heroes.Enemies.Where(Enemy => Enemy.IsInRange(Udyr, RangeVsEnemies)).ToList();

            List<Obj_AI_Minion> Minions = new List<Obj_AI_Minion>();
            Minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(Minion => Minion.IsInRange(Udyr, RangeVsMinionsAndMonsters)).ToList();

            List<Obj_AI_Minion> Monsters = new List<Obj_AI_Minion>();
            Monsters = EntityManager.MinionsAndMonsters.Monsters.Where(Monster => Monster.IsInRange(Udyr, RangeVsMinionsAndMonsters)).ToList();

            if (Enemies.Count != 0 || Minions.Count != 0 || Monsters.Count != 0)
            {
                CriticalShield(Enemies.Count, Minions.Count, Monsters.Count);
                UsePotion();
            }

            int RangeEnemies = Fight["Enemies.Range"].Cast<Slider>().CurrentValue;
            int RangeMinionsAndMonsters = Fight["MinionsAndMonsters.Range"].Cast<Slider>().CurrentValue;

            if (target == Target.Enemy && RangeVsEnemies != RangeEnemies)
                Enemies = EntityManager.Heroes.Enemies.Where(Enemy => Enemy.IsInRange(Udyr, RangeEnemies)).ToList();
            else if (target == Target.Minion && RangeVsMinionsAndMonsters != RangeMinionsAndMonsters)
                Minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(Minion => Minion.IsInRange(Udyr, RangeMinionsAndMonsters)).ToList();
            else if (target == Target.Monster && RangeVsMinionsAndMonsters != RangeMinionsAndMonsters)
                Monsters = EntityManager.MinionsAndMonsters.Monsters.Where(Monster => Monster.IsInRange(Udyr, RangeMinionsAndMonsters)).ToList();

            if (target == Target.Enemy)
                return Enemies;
            else if (target == Target.Minion)
                return Minions;
            else if (target == Target.Monster)
                return Monsters;

            return null;
        }

        internal static bool TigerEffect()
        {
            return Udyr.HasBuff(Buffs["Q.Stance"]) && TigerPunch;
        }

        internal static bool PhoenixEffect()
        {
            return Udyr.HasBuff(Buffs["R.Stance"]) && (PhoenixHits == 0 || PhoenixHits % 3 == 0);
        }

        internal static void UsePotion()
        {
            if (Udyr.Health > 250)
                return;

            if (HealthPotion.IsOwned(Udyr) && HealthPotion.IsReady())
                HealthPotion.Cast();
            else if(RefillablePotion.IsOwned(Udyr) && RefillablePotion.IsReady())
                RefillablePotion.Cast();
            else if(HuntersPotion.IsOwned(Udyr) && HuntersPotion.IsReady())
                HuntersPotion.Cast();
            else if(CorruptingPotion.IsOwned(Udyr) && CorruptingPotion.IsReady())
                CorruptingPotion.Cast();
        }
    }
}
