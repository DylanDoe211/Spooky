using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Shipyard
{
	public class CursedConch : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useAnimation = 90;
			Item.useTime = 90;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 5);
		}

		public override void UseAnimation(Player player)
		{
			if (player.statLife >= player.statLifeMax2 / 3)
			{
				if (player.ItemTimeIsZero)
				{
					player.ApplyItemTime(Item);
				}
				player.GetModPlayer<SpookyPlayer>().CursedConchTeleporting = true;
			}
		}
	}
}