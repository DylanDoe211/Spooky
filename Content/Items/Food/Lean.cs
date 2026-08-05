using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Food
{
	public class Lean : ModItem
	{
		public override void SetDefaults()
        {
			Item.healLife = 200;
			Item.width = 42;
			Item.height = 28;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.expert = true;
			Item.noMelee = true;
			Item.consumable = true;
            Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 50);
			Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
        }

		public override void HoldStyle(Player player, Rectangle heldItemFrame) 
		{
			player.itemLocation.X = player.MountedCenter.X + 4f * player.direction;
			player.itemLocation.Y = player.MountedCenter.Y + 14f;
			player.itemRotation = 0f;
		}

		public override bool? UseItem(Player player)
		{
            return true;
        }
	}
}
