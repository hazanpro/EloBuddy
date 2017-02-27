using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;
using static SmartCast.Damages;
using static SmartCast.Program;
using static SmartCast.Settings;
using static SmartCast.Spells;
using static SmartCast.Logic.SummonerSpells;

namespace SmartCast.Logic
{
    internal class Abilities
    {
        #region Basic

        internal static void BasicAttack(Unit unit)
        {
            if (unit == Unit.Enemy)
                BasicAttackEnemy();
            else if (unit == Unit.Minion)
                BasicAttackMinion();
            else if (unit == Unit.Monster)
                BasicAttackMonster();
        }

        private static void BasicAttackEnemy()
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target != null)
                Orbwalker.ForcedTarget = target;
        }

        private static void BasicAttackMinion()
        {
            Obj_AI_Minion target = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsValidTarget(HALF_RANGE)).FirstOrDefault();

            if (target != null)
                Orbwalker.ForcedTarget = target;
        }

        private static void BasicAttackMonster()
        {
            Obj_AI_Minion target = EntityManager.MinionsAndMonsters.Monsters.OrderByDescending(Monster => Monster.MaxHealth).Where(Monster => Monster.IsValidTarget(HALF_RANGE)).FirstOrDefault();

            if (target != null)
                Orbwalker.ForcedTarget = target;
        }

        #endregion

        #region Attack

        internal static void Attack(Mode mode, bool quickCast = false)
        {
            if (!Q.IsReady() && !R.IsReady())
                return;
            else if (mode == Mode.Combo || mode == Mode.Harass)
                ComboHarassAttack(quickCast);
            else if (mode == Mode.LastHit)
                LastHitAttack(quickCast);
            else if (mode == Mode.JungleClear)
                JungleAttack(quickCast);
            else if (mode == Mode.LaneClear)
                LaneAttack(quickCast);
            
        }

        private static void ComboHarassAttack(bool quickCast = false)
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasUndyingBuff()).FirstOrDefault();
            
            if (target == null)
                return;
            
            Stance stance = GetStance(target, quickCast);

            switch (stance)
            {
                case Stance.Tiger:
                    CastQ(quickCast, target);
                    break;
                case Stance.Phoenix:
                    CastR(quickCast, target);
                    break;
                case Stance.Hybrid:
                    CastQ(quickCast, target);
                    CastR(true, target);
                    break;
            }
            
            Orbwalker.ForcedTarget = target;
        }

        private static void LastHitAttack(bool quickCast = false)
        {
            Obj_AI_Minion target = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsValidTarget(HALF_RANGE)).FirstOrDefault();

            if (target == null)
                return;
            else if (!InQStance)
                CastQ(quickCast);
        }

        private static void JungleAttack(bool quickCast = false)
        {
            List<Obj_AI_Minion> targets = EntityManager.MinionsAndMonsters.Monsters.OrderByDescending(Monster => Monster.MaxHealth).Where(Monster => Monster.IsValidTarget(HALF_RANGE)).ToList();

            if (targets == null || targets.Count == 0)
                return;

            Obj_AI_Minion target = targets.FirstOrDefault();

            if (targets.Count == 1)
            {
                Stance stance = GetStance(target, quickCast);

                switch (stance)
                {
                    case Stance.Tiger:
                        CastQ(quickCast);
                        break;
                    case Stance.Phoenix:
                        CastR(quickCast);
                        break;
                    case Stance.Hybrid:
                        CastQ(quickCast);
                        CastR(true);
                        break;
                }
            }
            else if (targets.Count > 1)
            {
                CastR(quickCast);
                CastQ(quickCast);
            }

            Orbwalker.ForcedTarget = target;
        }

        private static void LaneAttack(bool quickCast = false)
        {
            Obj_AI_Minion target = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(Minion => Minion.Health).Where(Minion => Minion.IsValidTarget(HALF_RANGE)).FirstOrDefault();

            if (target == null)
                return;

            CastR(quickCast);
            CastQ(quickCast);
            Orbwalker.ForcedTarget = target;
        }

        private static void CastQ(bool quickCast = false, AIHeroClient target = null)
        {
            if (!Q.IsReady())
                return;
            else if (NextBasicIsConeFlame)
                return;
            else if (target != null && HasEActive && Stunable(target))
                return;
            else if (!quickCast && HasRActive)
                return;

            Q.Cast();
        }

        private static void CastR(bool quickCast = false, AIHeroClient target = null)
        {
            if (!R.IsReady())
                return;
            else if (NextBasicIsPunchBleed)
                return;
            else if (target != null && HasEActive && Stunable(target))
                return;
            else if (!quickCast && HasQActive)
                return;

            R.Cast();
        }

        #endregion

        #region Shield

        internal static void Shield(Mode mode)
        {
            if (!W.IsReady())
                return;
            else if (mode == Mode.Combo)
                ComboShield();
            else if (mode == Mode.Harass)
                HarassShield();
            else if (mode == Mode.JungleClear)
                JungleShield();
            else if (mode == Mode.LaneClear)
                LaneShield();
            else if (mode == Mode.Flee)
                FleeShield();
        }

        internal static void CriticalShield()
        {
            if (!W.IsReady())
                return;

            if (Udyr.HealthPercent > CRITICAL_CHAMPIONS_HEALTH && Udyr.Health > CRITICAL_MINIONS_AND_MONSTERS_HEALTH)
                return;

            int enemies = EntityManager.Heroes.Enemies.Where(Enemy => Enemy.IsInRange(Udyr, FULL_RANGE)).Count();
            int minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(Minion => Minion.IsInRange(Udyr, HALF_RANGE)).Count();
            int monsters = EntityManager.MinionsAndMonsters.Monsters.Where(Monster => Monster.IsInRange(Udyr, HALF_RANGE)).Count();

            if (enemies > 0 || minions > 0 || monsters > 0)
            {
                if (enemies > 0 && Udyr.HealthPercent <= CRITICAL_CHAMPIONS_HEALTH)
                    W.Cast();
                else if ((minions > 0 || monsters > 0) && Udyr.Health <= CRITICAL_MINIONS_AND_MONSTERS_HEALTH)
                    W.Cast();
            }
        }

        private static void ComboShield()
        {
            if (NextBasicIsPunchBleed || NextBasicIsConeFlame)
                return;
            else if ((HasQActive || HasRActive) && Udyr.HealthPercent >= CHAMPIONS_HEALTH)
                return;

            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return;

            bool quickCast = Udyr.HealthPercent < CHAMPIONS_HEALTH;
            Stance stance = GetStance(target, quickCast);

            bool castQ = stance == Stance.Tiger && Q.IsReady();
            bool castR = stance == Stance.Phoenix && R.IsReady();
            bool castQR = stance == Stance.Hybrid && (Q.IsReady() || R.IsReady()) && Udyr.HealthPercent >= CHAMPIONS_HEALTH;

            if (castQ || castR || castQR)
                return;

            W.Cast();
        }

        private static void HarassShield()
        {
            if (NextBasicIsPunchBleed || NextBasicIsConeFlame)
                return;

            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Health).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasUndyingBuff()).FirstOrDefault();

            if (target == null)
                return;

            Stance stance = GetStance(target, true);

            bool castQ = stance == Stance.Tiger && Q.IsReady();
            bool castR = stance == Stance.Phoenix && R.IsReady();
            bool castQR = stance == Stance.Hybrid && (Q.IsReady() || R.IsReady()) && Udyr.HealthPercent >= CHAMPIONS_HEALTH;

            if (castQ || castR || castQR)
                return;

            W.Cast();
        }

        private static void JungleShield()
        {
            if (NextBasicIsPunchBleed || NextBasicIsConeFlame)
                return;

            List<Obj_AI_Minion> targets = EntityManager.MinionsAndMonsters.Monsters.OrderByDescending(Monster => Monster.MaxHealth).Where(Monster => Monster.IsValidTarget(HALF_RANGE)).ToList();

            if (targets == null || targets.Count == 0)
                return;

            string monster = targets.FirstOrDefault().BaseSkinName;

            if (monster == Monster["Baron Nashor"])
                W.Cast();
            else if ((monster == Monster["Rift Herard"] || monster == Monster["Mountain Drake"]) && Udyr.Health <= BIG_MINIONS_AND_MONSTERS_HEALTH)
                W.Cast();
            else if (Udyr.Health <= SMALL_MINIONS_AND_MONSTERS_HEALTH)
                W.Cast();
        }

        private static void LaneShield()
        {
            if (NextBasicIsPunchBleed || NextBasicIsConeFlame)
                return;

            if (Udyr.Health <= LANE_MINIONS_AND_MONSTERS_HEALTH)
                W.Cast();
        }

        private static void FleeShield()
        {
            if (HasEActive)
                return;

            W.Cast();
        }

        #endregion

        #region Stun

        internal static void Stun(Mode mode)
        {
            if (!E.IsReady() && !InEStance)
                return;

            switch(mode)
            {
                case Mode.Combo:
                    ComboStun();
                    break;
                case Mode.Harass:
                    HarassStun();
                    break;
                case Mode.Flee:
                    FleeStun();
                    break;
            }
        }

        internal static void Escape()
        {
            if (E.IsReady())
                E.Cast();
        }

        private static void ComboStun()
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
        }

        private static void HarassStun()
        {
            if (NextBasicIsStunCycle)
                return;

            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => Enemy.IsValidTarget(FULL_RANGE) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
        }

        private static void FleeStun()
        {
            AIHeroClient target = EntityManager.Heroes.Enemies.OrderBy(Enemy => Enemy.Distance(Udyr)).Where(Enemy => Enemy.IsValidTarget(Udyr.AttackRange) && !Enemy.HasBuffOfType(BuffType.Stun) && Stunable(Enemy)).FirstOrDefault();

            if (target == null)
                return;
            else if (E.IsReady())
                E.Cast();

            Orbwalker.ForcedTarget = target;
        }

        private static bool Stunable(AIHeroClient target)
        {
            return !target.IsEStunned() && StunTimer[target.Hero] + 6 < Game.Time;
        }

        #endregion
    }
}