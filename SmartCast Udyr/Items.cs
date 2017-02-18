using EloBuddy;
using EloBuddy.SDK;

namespace SmartCast
{
    internal class Items
    {
        internal static Item HealthPotion { get; private set; }
        internal static Item RefillablePotion { get; private set; }
        internal static Item HuntersPotion { get; private set; }
        internal static Item CorruptingPotion { get; private set; }

        internal static void Initialize()
        {
            HealthPotion = new Item(ItemId.Health_Potion);
            RefillablePotion = new Item(ItemId.Refillable_Potion);
            HuntersPotion = new Item(ItemId.Hunters_Potion);
            CorruptingPotion = new Item(ItemId.Corrupting_Potion);
        }
    }
}