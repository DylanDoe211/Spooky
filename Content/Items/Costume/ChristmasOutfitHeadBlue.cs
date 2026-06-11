using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class ChristmasOutfitHeadBlue : ModItem
	{
		public override void SetDefaults() 
        {
			Item.width = 24;
			Item.height = 20;
			Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
		}
	}
}