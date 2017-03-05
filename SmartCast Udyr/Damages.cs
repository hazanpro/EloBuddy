using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Collections.Generic;
using static SmartCast.Program;
using static SmartCast.Spells;

namespace SmartCast
{
    internal static class Damages
    {
        private static Dictionary<string, string> SummonerSpell = GetSummonerSpells();

        #region Enums

        internal enum Effect
        {
            Killsteal,
            Heal,
            Slow
        }

        internal enum Mode
        {
            Combo,
            Harass,
            LastHit,
            JungleClear,
            LaneClear,
            Flee,
            None
        }

        internal enum SmiteType
        {
            Normal,
            Chilling,
            Challenging
        }

        internal enum Stance
        {
            Tiger,
            Phoenix,
            Hybrid
        }

        internal enum Unit
        {
            Ally,
            Enemy,
            Minion,
            Monster
        }

        #endregion

        #region Extensions

        internal static bool Type(this Spell.SpellBase spell, SmiteType type)
        {
            if (spell.Slot != SpellSlot.Summoner1 && spell.Slot != SpellSlot.Summoner2)
                return false;

            if (type == SmiteType.Normal && spell.Name == SummonerSpell["Smite"])
                return true;
            else if (type == SmiteType.Chilling && spell.Name == SummonerSpell["Chilling Smite"])
                return true;
            else if (type == SmiteType.Challenging && spell.Name == SummonerSpell["Challenging Smite"])
                return true;

            return false;
        }

        internal static float Damage(this Spell.SpellBase spell, Obj_AI_Base target, bool quickCast = false)
        {
            if (target == null)
                return 0;

            if (spell.Slot == SpellSlot.Summoner1 || spell.Slot == SpellSlot.Summoner2)
                return SummonerSpellDamage(target, spell.Slot, quickCast);
            else if (spell.Slot == SpellSlot.Q)
                return QDamage(target, quickCast);
            else if (spell.Slot == SpellSlot.R)
                return RDamage(target, quickCast);

            return 0;
        }

        internal static bool Killable(this Spell.SpellBase spell, Obj_AI_Base target, bool quickCast = false)
        {
            if (target == null)
                return false;

            if (spell.Slot == SpellSlot.Summoner1 || spell.Slot == SpellSlot.Summoner2)
                return target.Health <= spell.Damage(target, quickCast);
            else if (spell.Slot == SpellSlot.Q)
                return target.Health <= Q.Damage(target, quickCast);
            else if (spell.Slot == SpellSlot.R)
                return target.Health <= R.Damage(target, quickCast);

            return false;
        }

        #endregion

        #region Helpers

        private static Dictionary<string, string> GetSummonerSpells()
        {
            return new Dictionary<string, string>()
            {
                { "Ignite", "SummonerDot" },
                { "Smite", "SummonerSmite" },
                { "Chilling Smite", "S5_SummonerSmitePlayerGanker" },
                { "Challenging Smite", "S5_SummonerSmiteDuel" }
            };
        }

        private static int GetPassiveStacks()
        {
            string passive = "udyrmonkeyagilitybuff";
            return Udyr.HasBuff(passive) ? Udyr.GetBuff(passive).Count : 0;
        }

        private static float SummonerSpellDamage(Obj_AI_Base target, SpellSlot slot, bool quickCast)
        {
            if (Player.GetSpell(slot).Name == SummonerSpell["Ignite"])
                return IgniteDamage(target);
            else if (Player.GetSpell(slot).Name == SummonerSpell["Smite"])
                return SmiteDamage(target);
            else if (Player.GetSpell(slot).Name == SummonerSpell["Chilling Smite"])
                return ChillingSmiteDamage(target);
            else if (Player.GetSpell(slot).Name == SummonerSpell["Challenging Smite"])
                return ChallengingSmiteDamage(target, quickCast);

            return 0;
        }

        internal static Stance GetStance(Obj_AI_Base target, bool quickCast = false)
        {
            float damageQ = Q.Damage(target, quickCast);
            float damageR = R.Damage(target, quickCast);

            float difference = Math.Abs(damageQ - damageR);

            if (difference < 50)
                return Stance.Hybrid;

            return damageQ > damageR ? Stance.Tiger : Stance.Phoenix;
        }

        #endregion

        #region Abilities

        private static float QDamage(Obj_AI_Base target, bool quickCast)
        {
            if (!Q.IsLearned)
                return 0;

            float damage = 0;
            float physicalDamage = 0;
            float attackSpeed = 1 / Udyr.AttackDelay;

            if (GetPassiveStacks() < 3)
                attackSpeed += attackSpeed * 0.1f;

            attackSpeed += attackSpeed * new float[] { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f }[Q.Level - 1];
            physicalDamage += new int[] { 15, 40, 65, 90, 115 }[Q.Level - 1] * 2 + (0.15f * Udyr.FlatPhysicalDamageMod);

            if (quickCast == false)
                physicalDamage += Udyr.TotalAttackDamage * (int)(attackSpeed * 5) * (0.15f * Udyr.FlatPhysicalDamageMod);

            damage = Udyr.CalculateDamageOnUnit(target, DamageType.Physical, physicalDamage, true, false);

            return damage;
        }

        private static float RDamage(Obj_AI_Base target, bool quickCast)
        {
            if (!R.IsLearned)
                return 0;

            float damage = 0;
            float physicalDamage = 0;
            float magicalDamage = 0;
            float attackSpeed = 1 / Udyr.AttackDelay;

            if (GetPassiveStacks() < 3)
                attackSpeed += attackSpeed * 0.1f;

            magicalDamage += new int[] { 10, 20, 30, 40, 50 }[R.Level - 1] + (0.25f * Udyr.FlatMagicDamageMod);

            if (quickCast == false)
                magicalDamage *= 4;

            physicalDamage += Udyr.TotalAttackDamage;
            magicalDamage += new int[] { 40, 80, 120, 160, 200 }[R.Level - 1] + (0.45f * Udyr.FlatMagicDamageMod);

            if (quickCast == false)
            {
                int Hits = (int)(attackSpeed * 4);
                int HitCount = 1;

                while (Hits-- > 0)
                {
                    physicalDamage += Udyr.TotalAttackDamage;

                    if (HitCount % 3 == 0)
                        magicalDamage += new int[] { 40, 80, 120, 160, 200 }[R.Level - 1] + (0.45f * Udyr.FlatMagicDamageMod);
                }
            }

            damage += Udyr.CalculateDamageOnUnit(target, DamageType.Physical, physicalDamage, true, false);
            damage += Udyr.CalculateDamageOnUnit(target, DamageType.Magical, magicalDamage, true, false);

            return damage;
        }

        #endregion

        #region Summoner Spells

        private static float IgniteDamage(Obj_AI_Base target)
        {
            int damagePerSecond = 10 + (4 * Udyr.Level);
            float regenRate = target.HPRegenRate - (target.HPRegenRate * 0.4f);

            return (damagePerSecond * 5) - (regenRate * 5);
        }

        private static float SmiteDamage(Obj_AI_Base target)
        {
            float damage = 0;
            int[] Smite = new int[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
            int trueDamage = Smite[Udyr.Level - 1];

            damage = Udyr.CalculateDamageOnUnit(target, DamageType.True, trueDamage, false, true);

            return damage;
        }

        private static float ChillingSmiteDamage(Obj_AI_Base target)
        {
            if (target.IsMinion || target.IsMonster)
                return SmiteDamage(target);

            float damage = 0;
            int trueDamage = 20 + (8 * Udyr.Level);

            damage = Udyr.CalculateDamageOnUnit(target, DamageType.True, trueDamage, false, true);

            return damage;
        }

        private static float ChallengingSmiteDamage(Obj_AI_Base target, bool quickCast)
        {
            if (target.IsMinion || target.IsMonster)
                return SmiteDamage(target);

            float damage = 0;
            float trueDamage = 54 + (6 * Udyr.Level);
            float attackSpeed = 1 / Udyr.AttackDelay;

            if (quickCast == false)
                trueDamage *= (int)attackSpeed * 3;

            damage = Udyr.CalculateDamageOnUnit(target, DamageType.True, trueDamage, false, true);

            return damage;
        }

        #endregion
    }
}
