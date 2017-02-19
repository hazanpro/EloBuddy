using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Drawing;
using static SmartCast.Settings;
using static SmartCast.Utilities;

namespace SmartCast
{
    internal class Events
    {
        internal static void Initialize()
        {
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || args == null)
                return;

            if (sender.IsMe && Udyr.HasBuff(Buffs["R.Stance"]))
                PhoenixHits++;
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender == null || args == null)
                return;

            if (sender.IsMe && args.Buff.Name == Buffs["Passive"])
            {
                if (Passive["Stacks"] < 3)
                {
                    Passive["Stacks"]++;
                    Passive["MovementSpeed"] += 5;
                    Passive["AttackSpeed"] += 10;
                }
            }
            else if (sender.IsEnemy && !sender.IsMinion && !sender.IsMonster && args.Buff.Name == Buffs["E.Stun"])
                StunTimer[(sender as AIHeroClient).Hero] = Game.Time;
            else if (sender.IsMe && args.Buff.Name == Buffs["Q.Stance"])
                TigerPunch = true;
            else if (sender.IsEnemy && args.Buff.Name == Buffs["Q.Punch"])
                TigerPunch = false;
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender == null || args == null)
                return;

            if (sender.IsMe && args.Buff.Name == Buffs["Passive"])
            {
                Passive["Stacks"]--;
                Passive["MovementSpeed"] -= 5;
                Passive["AttackSpeed"] -= 10;
            }
            else if (sender.IsMe && args.Buff.Name == Buffs["R.Stance"])
                PhoenixHits = 0;
        }
        
        private static void OnDraw(EventArgs args)
        {
            int Range = Fight["Enemies.Range"].Cast<Slider>().CurrentValue;
            Drawing.DrawCircle(Udyr.Position, Range, Color.DarkGray);
        }
    }
}
