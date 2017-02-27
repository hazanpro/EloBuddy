using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System;
using System.Drawing;

namespace SmartCast
{
    internal class Program
    {
        internal static AIHeroClient Udyr { get; private set; }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            Udyr = ObjectManager.Player;

            if (Udyr.Hero != Champion.Udyr)
                return;

            Settings.Initialize();
            Spells.Initialize();
            Items.Initialize();
            
            Chat.Print("SmartCast Udyr", Color.DarkGray);

            Drawing.OnDraw += OnDraw;
            Game.OnTick += OnTick;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnDraw(EventArgs args)
        {
            Drawing.DrawCircle(Udyr.Position, Settings.FULL_RANGE, Color.DarkGray);
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
                Modes.Combo();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Modes.Harass();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                Modes.LastHit();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                Modes.JungleClear();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                Modes.LaneClear();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                Modes.Flee();
        }
    }
}