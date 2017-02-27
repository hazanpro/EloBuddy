using EloBuddy.SDK;
using System.Linq;
using static SmartCast.Damages;
using static SmartCast.Program;
using static SmartCast.Settings;
using static SmartCast.Logic.Abilities;
using static SmartCast.Logic.ActiveItems;
using static SmartCast.Logic.SummonerSpells;

namespace SmartCast
{
    internal class Modes
    {
        internal static void Combo()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Enemy))
                return;

            UsePotions(Effect.Heal);
            UseIgnite(Effect.Killsteal);
            UseChillingSmite(Effect.Killsteal);
            UseChillingSmite(Effect.Slow, Mode.Combo);
            Stun(Mode.Combo);
            Shield(Mode.Combo);
            Attack(Mode.Combo);
            UseChallengingSmite(Mode.Combo);
            BasicAttack(Unit.Enemy);
        }

        internal static void Harass()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Enemy))
                return;

            UsePotions(Effect.Heal);
            UseIgnite(Effect.Killsteal);
            UseChillingSmite(Effect.Killsteal);
            Stun(Mode.Harass);
            Shield(Mode.Harass);
            Attack(Mode.Harass);
            BasicAttack(Unit.Enemy);
        }

        internal static void LastHit()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Minion))
                return;

            UsePotions(Effect.Heal);
            Shield(Mode.LastHit);
            Attack(Mode.LastHit);
        }

        internal static void JungleClear()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Monster))
                return;

            UsePotions(Effect.Heal);
            UseSmite(Effect.Killsteal, Mode.JungleClear);
            UseSmite(Effect.Heal);
            Shield(Mode.JungleClear);
            Attack(Mode.JungleClear);
            BasicAttack(Unit.Monster);
        }

        internal static void LaneClear()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Minion))
                return;

            UsePotions(Effect.Heal);
            UseSmite(Effect.Killsteal, Mode.LaneClear);
            UseSmite(Effect.Heal);
            Shield(Mode.LaneClear);
            Attack(Mode.LaneClear);
            BasicAttack(Unit.Minion);
        }

        internal static void Flee()
        {
            CriticalShield();

            if (!IsUnitsAround(Unit.Enemy))
                return;

            UsePotions(Effect.Heal);
            UseIgnite(Effect.Killsteal);
            UseChillingSmite(Effect.Killsteal);
            Stun(Mode.Flee);
            UseChillingSmite(Effect.Slow, Mode.Flee);
            UseChallengingSmite(Mode.Flee);
            Shield(Mode.Flee);
            Escape();
        }

        private static bool IsUnitsAround(Unit unit)
        {
            if (unit == Unit.Enemy)
                return EntityManager.Heroes.Enemies.Where(Enemy => Enemy.IsInRange(Udyr, FULL_RANGE)).Any();
            else if (unit == Unit.Minion)
                return EntityManager.MinionsAndMonsters.EnemyMinions.Where(Minion => Minion.IsInRange(Udyr, HALF_RANGE)).Any();
            else if (unit == Unit.Monster)
                return EntityManager.MinionsAndMonsters.Monsters.Where(Monster => Monster.IsInRange(Udyr, HALF_RANGE)).Any();

            return false;
        }
    }
}