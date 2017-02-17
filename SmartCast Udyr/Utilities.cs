using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;

namespace SmartCast
{
    internal class Utilities
    {
        internal static AIHeroClient Udyr { get; private set; }
        internal static bool TigerPunch { get; set; }
        internal static int PhoenixHits { get; set; }
        internal static Dictionary<Champion, float> StunTimer { get; set; }
        internal static Dictionary<string, int> Passive { get; set; }
        internal static Dictionary<string, string> Buffs { get; private set; }
        internal static Dictionary<string, string> Spells { get; private set; }
        internal static Dictionary<string, string> Minions { get; private set; }
        internal static Dictionary<string, string> Monsters { get; private set; }

        internal static void Initialize()
        {
            Udyr = ObjectManager.Player;
            TigerPunch = false;
            PhoenixHits = 0;

            InitializeStunTimer();
            InitializePassive();
            InitializeBuffs();
            InitializeSpells();
            InitializeMinions();
            InitializeMonsters();
        }

        private static void InitializeStunTimer()
        {
            StunTimer = new Dictionary<Champion, float>(EntityManager.Heroes.Enemies.Count);

            foreach (AIHeroClient Enemy in EntityManager.Heroes.Enemies)
                StunTimer.Add(Enemy.Hero, 0);
        }

        private static void InitializePassive()
        {
            Passive = new Dictionary<string, int>(3);

            Passive.Add("Stacks", 0);
            Passive.Add("MovementSpeed", 0);
            Passive.Add("AttackSpeed", 0);
        }

        private static void InitializeBuffs()
        {
            Buffs = new Dictionary<string, string>();

            Buffs.Add("Passive", "udyrmonkeyagilitybuff");

            Buffs.Add("Q.Stance", "UdyrTigerStance");
            Buffs.Add("W.Stance", "UdyrTurtleStance");
            Buffs.Add("E.Stance", "UdyrBearStance");
            Buffs.Add("R.Stance", "UdyrPhoenixStance");

            Buffs.Add("Q.Activation", "udyrtigerpunch");
            Buffs.Add("W.Activation", "udyrturtleactivation");
            Buffs.Add("E.Activation", "udyrbearactivation");
            Buffs.Add("R.Activation", "UdyrPhoenixActivation");

            Buffs.Add("Q.Punch", "UdyrTigerPunchBleed");
            Buffs.Add("E.Stun", "udyrbearstuncheck");
        }

        private static void InitializeSpells()
        {
            Spells = new Dictionary<string, string>();

            Spells.Add("Ignite", "SummonerDot");
            Spells.Add("Smite", "SummonerSmite");
            Spells.Add("Chilling Smite", "S5_SummonerSmitePlayerGanker");
            Spells.Add("Challenging Smite", "S5_SummonerSmiteDuel");
        }

        private static void InitializeMinions()
        {
            Minions = new Dictionary<string, string>();

            Minions.Add("Melee", "SRU_ChaosMinionMelee");
            Minions.Add("Caster", "SRU_ChaosMinionRanged");
            Minions.Add("Siege", "SRU_ChaosMinionSiege");
            Minions.Add("Super", "SRU_ChaosMinionSuper");
        }

        private static void InitializeMonsters()
        {
            Monsters = new Dictionary<string, string>();

            // Baron
            Monsters.Add("Rift Herard", "SRU_RiftHerard");
            Monsters.Add("Baron Nashor", "SRU_Baron");

            // Dragon
            Monsters.Add("Mountain Drake", "SRU_Dragon_Earth");

            // Buffs
            Monsters.Add("Blue Sentinel", "SRU_Blue");
            Monsters.Add("Red Brambleback", "SRU_Red");

            // River
            Monsters.Add("Rift Scutller", "Sru_Crab");

            // Gromp
            Monsters.Add("Gromp", "SRU_Gromp");

            // Raptors
            Monsters.Add("Crimson Raptor", "SRU_Razorbeak");
            Monsters.Add("Raptor", "SRU_RazorbeakMini");

            // Murk Wolfs
            Monsters.Add("Geater Murk Wolf", "SRU_Murkwolf");
            Monsters.Add("Murk Wolf", "SRU_MurkwolfMini");

            // Krugs
            Monsters.Add("Ancient Krug", "SRU_Krug");
            Monsters.Add("Krug", "SRU_KrugMini");
            Monsters.Add("Mini Krug", "SRU_KrugMiniMini");

            // Planets
            Monsters.Add("Blast Cone", "SRU_Planet_Satchel");
            Monsters.Add("Honey Fruit", "SRU_Planet_Health");
            Monsters.Add("Scryer's Bloom", "SRU_Planet_Vision");
        }
    }
}