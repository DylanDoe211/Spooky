using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Legs)]
	public class ChristmasOutfitLegs : ModItem
	{
		public override void SetDefaults() 
        {
			Item.width = 22;
			Item.height = 18;
			Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
		}
	}
}