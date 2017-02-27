using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Spells;

namespace SmartCast
{
    internal class Settings
    {
        internal static Menu Main { get; private set; }
        internal static Menu SmiteMenu { get; private set; }

        internal static int FULL_RANGE = 600;
        internal static int HALF_RANGE = 300;
        internal static int CHAMPIONS_HEALTH = 65;
        internal static int BIG_MINIONS_AND_MONSTERS_HEALTH = 1500;
        internal static int SMALL_MINIONS_AND_MONSTERS_HEALTH = 1000;
        internal static int LANE_MINIONS_AND_MONSTERS_HEALTH = 500;
        internal static int CRITICAL_CHAMPIONS_HEALTH = 30;
        internal static int CRITICAL_MINIONS_AND_MONSTERS_HEALTH = 150;
        internal static int SMITE_HEALTH = 100;
        internal static int KILLSTEAL_HEALTH = 40;
        internal static int POTIONS_HEALTH = 250;
        internal static int SLOW_DISTANCE = 450;

        internal static void Initialize()
        {
            Main = MainMenu.AddMenu("SmartCast", "Udyr", "SmartCast Udyr");
            Main.AddLabel("Dynamically detect the kind of Udyr (Tiger / Phoenix / Hybrid)");
            Main.AddLabel("Written by hazanpro for EloBuddy community");

            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Smite))
                LoadSmiteMenu();
        }

        private static void LoadSmiteMenu()
        {
            SmiteMenu = Main.AddSubMenu("Smite");
            SmiteMenu.AddGroupLabel("Minions");
            SmiteMenu.Add("Minion.Melee", new CheckBox("Melee", false));
            SmiteMenu.Add("Minion.Caster", new CheckBox("Caster", false));
            SmiteMenu.Add("Minion.Siege", new CheckBox("Siege", true));
            SmiteMenu.Add("Minion.Super", new CheckBox("Super", true));
            SmiteMenu.AddSeparator();
            SmiteMenu.AddGroupLabel("Monsters");
            SmiteMenu.Add("Monster.Gromp", new CheckBox("Gromp", true));
            SmiteMenu.Add("Monster.AncientKrug", new CheckBox("Ancient Krug", true));
            SmiteMenu.Add("Monster.Krug", new CheckBox("Krug", false));
            SmiteMenu.Add("Monster.MiniKrug", new CheckBox("Mini Krug", false));
            SmiteMenu.Add("Monster.GreaterMurkWolf", new CheckBox("Greater Murk Wolf", true));
            SmiteMenu.Add("Monster.MurkWolf", new CheckBox("Murk Wolf", false));
            SmiteMenu.Add("Monster.CrimsonRaptor", new CheckBox("Crimson Raptor", true));
            SmiteMenu.Add("Monster.Raptor", new CheckBox("Raptor", false));
            SmiteMenu.Add("Monster.BlueSentinel", new CheckBox("Blue Sentinel", true));
            SmiteMenu.Add("Monster.RedBrambleback", new CheckBox("Red Brambleback", true));
            SmiteMenu.Add("Monster.RiftScutller", new CheckBox("Rift Scutller", true));
            SmiteMenu.Add("Monster.MountainDrake", new CheckBox("Mountain Drake", true));
            SmiteMenu.Add("Monster.RiftHerard", new CheckBox("Rift Herard", true));
            SmiteMenu.Add("Monster.BaronNashor", new CheckBox("Baron Nashor", true));
        }
    }
}