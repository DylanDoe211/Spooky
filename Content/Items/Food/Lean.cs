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
			Item.width = 30;
			Item.height = 36;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.maxStack = 9999;
			Item.noMelee = true;
			Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
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
			int[] RandomBuffs = { BuffID.Regeneration, BuffID.Swiftness, BuffID.Gills, BuffID.Ironskin, BuffID.Featherfall, BuffID.Spelunker,
			BuffID.Shine, BuffID.NightOwl, BuffID.Battle, BuffID.Thorns, BuffID.Gravitation, BuffID.Endurance };

			player.AddBuff(Main.rand.Next(RandomBuffs), 18000);

            return true;
        }
	}
}
