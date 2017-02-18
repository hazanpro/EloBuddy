using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartCast.SummonerSpells;
using static SmartCast.Utilities;

namespace SmartCast
{
    internal class Settings
    {
        internal static Menu Main { get; private set; }
        internal static Menu Danger { get; private set; }
        internal static Menu Fight { get; private set; }
        internal static Menu Smites { get; private set; }

        internal static void Initialize()
        {
            Main = MainMenu.AddMenu("SmartCast", "Udyr", "SmartCast Udyr");
            Main.AddLabel("Dynamically detect the kind of Udyr (Tiger / Phoenix / Hybrid)");
            Main.AddLabel("Written by hazanpro for EloBuddy");

            Danger = Main.AddSubMenu("Critical Shield", "Danger");
            Danger.AddGroupLabel("Enemies");
            Danger.Add("Enemies.Range", new Slider("[W] Range", 600, 550, 650));
            Danger.Add("Enemies.Health", new Slider("[W] Health (%)", 30, 25, 35));
            Danger.AddSeparator();
            Danger.AddGroupLabel("Minions and Monsters");
            Danger.Add("MinionsAndMonsters.Range", new Slider("[W] Range", 300, 250, 350));
            Danger.Add("MinionsAndMonsters.Health", new Slider("[W] Health", 150, 100, 200));

            Fight = Main.AddSubMenu("Fighting Modes", "Fight");
            Fight.AddGroupLabel("Enemies");
            Fight.Add("Enemies.Range", new Slider("[E] Range", 600, 550, 650));
            Fight.Add("Enemies.Health", new Slider("[W] Health (%)", 65, 60, 70));
            Fight.AddSeparator();
            Fight.AddGroupLabel("Minions and Monsters");
            Fight.AddLabel("[W] Health Vs. Baron activated automatically");
            Fight.Add("MinionsAndMonsters.Strong", new Slider("[W] Health Vs. Herard & Drake", 1500, 1000, 2000));
            Fight.Add("MinionsAndMonsters.Weak", new Slider("[W] Health Vs. Others", 1000, 500, 1500));

            Smites = Main.AddSubMenu("Smite", "Smites");
            Smites.AddGroupLabel("Minions");
            Smites.Add("Minion.Melee", new CheckBox("Melee", false));
            Smites.Add("Minion.Caster", new CheckBox("Caster", false));
            Smites.Add("Minion.Siege", new CheckBox("Siege", true));
            Smites.Add("Minion.Super", new CheckBox("Super", true));
            Smites.AddSeparator();
            Smites.AddGroupLabel("Monsters");
            Smites.Add("Monster.Gromp", new CheckBox("Gromp", true));
            Smites.Add("Monster.AncientKrug", new CheckBox("Ancient Krug", true));
            Smites.Add("Monster.Krug", new CheckBox("Krug", false));
            Smites.Add("Monster.MiniKrug", new CheckBox("Mini Krug", false));
            Smites.Add("Monster.GreaterMurkWolf", new CheckBox("Greater Murk Wolf", true));
            Smites.Add("Monster.MurkWolf", new CheckBox("Murk Wolf", false));
            Smites.Add("Monster.CrimsonRaptor", new CheckBox("Crimson Raptor", true));
            Smites.Add("Monster.Raptor", new CheckBox("Raptor", false));
            Smites.Add("Monster.BlueSentinel", new CheckBox("Blue Sentinel", true));
            Smites.Add("Monster.RedBrambleback", new CheckBox("Red Brambleback", true));
            Smites.Add("Monster.RiftScutller", new CheckBox("Rift Scutller", true));
            Smites.Add("Monster.MountainDrake", new CheckBox("Mountain Drake", true));
            Smites.Add("Monster.RiftHerard", new CheckBox("Rift Herard", true));
            Smites.Add("Monster.BaronNashor", new CheckBox("Baron Nashor", true));
            Smites.AddSeparator();
            Smites.AddGroupLabel("Chilling Smite");
            Smites.AddLabel("Chilling Smite will cast only if enemy is killable or trying to escape from you");
            Smites.Add("Chilling.Distance", new Slider("Enemy Distance", 450, 350, 450));
            Smites.AddSeparator();
            Smites.AddGroupLabel("Challenging Smite");
            Smites.AddLabel("Challenging Smite will cast only if enemy is stunned and in attack range of you");
        }
    }
}