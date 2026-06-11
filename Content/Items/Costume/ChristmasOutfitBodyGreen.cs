using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Body)]
	public class ChristmasOutfitBodyGreen : ModItem
	{
		public override void SetDefaults() 
        {
			Item.width = 30;
			Item.height = 26;
			Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
		}
	}
}