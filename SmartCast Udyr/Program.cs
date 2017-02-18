using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System;
using System.Drawing;

namespace SmartCast
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (ObjectManager.Player.Hero != Champion.Udyr)
                return;

            Utilities.Initialize();
            Settings.Initialize();
            Events.Initialize();
            Abilities.Initialize();
            SummonerSpells.Initialize();
            Items.Initialize();

            Chat.Print("SmartCast Udyr", Color.DarkGray);

            Game.OnTick += OnTick;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnTick(EventArgs args)
        {
            if (Orbwalker.UseOnTick)
                SmartCast();
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.UseOnUpdate)
                SmartCast();
        }

        private static void SmartCast()
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Modes.Combo.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Modes.Harass.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                Modes.LastHit.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                Modes.JungleClear.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                Modes.LaneClear.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                Modes.Flee.Execute();
        }
    }
}
