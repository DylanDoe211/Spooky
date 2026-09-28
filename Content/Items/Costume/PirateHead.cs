using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class PirateHead : ModItem
	{
		public override void SetDefaults() 
        {
			Item.width = 30;
			Item.height = 30;
			Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
		}
	}
}