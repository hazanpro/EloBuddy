using EloBuddy;
using static SmartCast.Damages;
using static SmartCast.Items;
using static SmartCast.Program;
using static SmartCast.Settings;

namespace SmartCast.Logic
{
    internal class ActiveItems
    {
        #region Potions

        internal static void UsePotions(Effect effect)
        {
            if (HealthPotion.IsOwned(Udyr) && effect == Effect.Heal)
                HealByHealthPotion();
            else if (RefillablePotion.IsOwned(Udyr) && effect == Effect.Heal)
                HealByRefillablePotion();
            else if (HuntersPotion.IsOwned(Udyr) && effect == Effect.Heal)
                HealByHuntersPotion();
            else if (CorruptingPotion.IsOwned(Udyr) && effect == Effect.Heal)
                HealByCorruptingPotion();
        }

        private static void HealByHealthPotion()
        {
            if (!HealthPotion.IsReady())
                return;
            else if (Udyr.Health < POTIONS_HEALTH && !Udyr.HasBuffOfType(BuffType.Heal))
                HealthPotion.Cast();
        }

        private static void HealByRefillablePotion()
        {
            if (!RefillablePotion.IsReady())
                return;
            else if (Udyr.Health < POTIONS_HEALTH && !Udyr.HasBuffOfType(BuffType.Heal))
                RefillablePotion.Cast();
        }

        private static void HealByHuntersPotion()
        {
            if (!HuntersPotion.IsReady())
                return;
            else if (Udyr.Health < POTIONS_HEALTH && !Udyr.HasBuffOfType(BuffType.Heal))
                HuntersPotion.Cast();
        }

        private static void HealByCorruptingPotion()
        {
            if (!CorruptingPotion.IsReady())
                return;
            else if (Udyr.Health < POTIONS_HEALTH && !Udyr.HasBuffOfType(BuffType.Heal))
                CorruptingPotion.Cast();
        }

        #endregion
    }
}