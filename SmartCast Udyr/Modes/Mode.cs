using EloBuddy.SDK;
using EloBuddy;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Abilities;
using static SmartCast.Items;
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
            if (!W.IsReady() || Udyr.HealthPercent > 30)
                return;
            else if (Udyr.HealthPercent <= 30 && Enemies > 0)
                W.Cast();
            else if (Udyr.Health <= 150 && (Minions > 0 || Monsters > 0))
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
            List<AIHeroClient> Enemies = new List<AIHeroClient>();
            Enemies = EntityManager.Heroes.Enemies.Where(Enemy => Enemy.IsInRange(Udyr, 600)).ToList();

            List<Obj_AI_Minion> Minions = new List<Obj_AI_Minion>();
            Minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(Minion => Minion.IsInRange(Udyr, 300)).ToList();

            List<Obj_AI_Minion> Monsters = new List<Obj_AI_Minion>();
            Monsters = EntityManager.MinionsAndMonsters.Monsters.Where(Monster => Monster.IsInRange(Udyr, 300)).ToList();

            if (Enemies.Count != 0 || Minions.Count != 0 || Monsters.Count != 0)
            {
                CriticalShield(Enemies.Count, Minions.Count, Monsters.Count);
                UsePotion();
            }

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