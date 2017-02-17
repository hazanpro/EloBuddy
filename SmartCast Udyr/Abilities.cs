using EloBuddy;
using EloBuddy.SDK;
using static SmartCast.Utilities;

namespace SmartCast
{
    internal class Abilities
    {
        internal static Spell.Active Q { get; private set; }
        internal static Spell.Active W { get; private set; }
        internal static Spell.Active E { get; private set; }
        internal static Spell.Active R { get; private set; }

        internal static void Initialize()
        {
            Q = new Spell.Active(SpellSlot.Q, 0, DamageType.Physical);
            W = new Spell.Active(SpellSlot.W, 0);
            E = new Spell.Active(SpellSlot.E, 0);
            R = new Spell.Active(SpellSlot.R, 250, DamageType.Magical);
        }

        internal static float Damage(Obj_AI_Base target, SpellSlot slot, bool quick = false)
        {
            if (slot != SpellSlot.Q && slot != SpellSlot.R)
                return 0;
            else if (slot == SpellSlot.Q && !Q.IsLearned)
                return 0;
            else if (slot == SpellSlot.R && !R.IsLearned)
                return 0;

            float AttackSpeed = Udyr.AttackSpeedMod;
            float PhysicalDamage = 0;
            float MagicalDamage = 0;
            float TotalDamage = 0;

            if (Passive["Stacks"] < 3)
                AttackSpeed += AttackSpeed * 0.1f;

            if (slot == SpellSlot.Q)
            {
                AttackSpeed += AttackSpeed * new float[] { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f }[Q.Level - 1];
                PhysicalDamage += new int[] { 15, 40, 65, 90, 115 }[Q.Level - 1] * 2 + (0.15f * Udyr.FlatPhysicalDamageMod);

                if (quick == false)
                    PhysicalDamage += Udyr.TotalAttackDamage * (int)(AttackSpeed * 5) * (0.15f * Udyr.FlatPhysicalDamageMod);
            }
            else if (slot == SpellSlot.R)
            {
                MagicalDamage += new int[] { 10, 20, 30, 40, 50 }[R.Level - 1] + (0.25f * Udyr.FlatMagicDamageMod);

                if (quick == false)
                {
                    MagicalDamage *= 4;
                }

                PhysicalDamage += Udyr.TotalAttackDamage;
                MagicalDamage += new int[] { 40, 80, 120, 160, 200 }[R.Level - 1] + (0.45f * Udyr.FlatMagicDamageMod);

                if (quick == false)
                {
                    int Hits = (int)(AttackSpeed * 4);
                    int HitCount = 1;

                    while (Hits-- > 0)
                    {
                        PhysicalDamage += Udyr.TotalAttackDamage;

                        if (HitCount % 3 == 0)
                            MagicalDamage += new int[] { 40, 80, 120, 160, 200 }[R.Level - 1] + (0.45f * Udyr.FlatMagicDamageMod);
                    }
                }
            }

            TotalDamage += Udyr.CalculateDamageOnUnit(target, DamageType.Physical, PhysicalDamage, true, false);
            TotalDamage += Udyr.CalculateDamageOnUnit(target, DamageType.Magical, MagicalDamage, true, false);

            return TotalDamage;
        }
    }
}