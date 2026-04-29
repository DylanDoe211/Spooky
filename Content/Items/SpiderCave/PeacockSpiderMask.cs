using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave
{
	public class PeacockSpiderMask : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 52;
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().PeacockSpiderMask = true;
        }
    }
}
