using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
	public class EggCarton : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }

		public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<EggCartonPlayer>().EggCarton = true;
		}
	}

    public class EggCartonPlayer : ModPlayer
    {
		public bool EggCarton = false;

		public override void ResetEffects()
        {
			EggCarton = false;
		}
	}
}
