using EloBuddy;
using EloBuddy.SDK;
using static SmartCast.Utilities;

namespace SmartCast
{
    internal class SummonerSpells
    {
        internal static Spell.Targeted Ignite { get; private set; }
        internal static Spell.Targeted Smite { get; private set; }

        internal static void Initialize()
        {
            InitializeSummonerSpell(SpellSlot.Summoner1);
            InitializeSummonerSpell(SpellSlot.Summoner2);
        }

        private static void InitializeSummonerSpell(SpellSlot slot)
        {
            string SummonerSpell = Player.GetSpell(slot).Name;

            if (SummonerSpell == Spells["Ignite"])
                Ignite = new Spell.Targeted(slot, 600, DamageType.True);
            else if (SummonerSpell.Contains("Smite"))
                Smite = new Spell.Targeted(slot, 500, DamageType.True);
        }

        internal static float Damage(Obj_AI_Base target, SpellSlot slot, bool quick = false)
        {
            if (slot != SpellSlot.Summoner1 && slot != SpellSlot.Summoner2)
                return 0;
            else if (Player.GetSpell(slot).Name != Spells["Ignite"] && !Player.GetSpell(slot).Name.Contains("Smite"))
                return 0;

            int[] Smite = new int[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };

            if (Player.GetSpell(slot).Name.Contains("Smite") && (target.IsMonster || target.IsMonster))
                return Udyr.CalculateDamageOnUnit(target, DamageType.True, Smite[Udyr.Level], false, true);

            float TotalDamage = 0;

            if (Player.GetSpell(slot).Name == Spells["Ignite"])
            {
                int DamagePerSecond = 10 + (4 * Udyr.Level);
                float HPRegenRate = target.HPRegenRate - (target.HPRegenRate * 0.4f);

                TotalDamage = (DamagePerSecond * 5) - (HPRegenRate * 5);
            }
            else if (Player.GetSpell(slot).Name == Spells["Chilling Smite"])
            {
                TotalDamage = 20 + (8 * Udyr.Level);
            }
            else if (Player.GetSpell(slot).Name == Spells["Challenging Smite"])
            {
                TotalDamage = 54 + (6 * Udyr.Level);

                if (quick == false)
                    TotalDamage *= (int)(Udyr.AttackSpeedMod * 3);
            }

            return Udyr.CalculateDamageOnUnit(target, DamageType.True, TotalDamage, false, true);
        }
    }
}