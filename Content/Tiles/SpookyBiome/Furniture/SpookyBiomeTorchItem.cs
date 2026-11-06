using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyBiomeTorchItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.Torches[Type] = true;
			ItemID.Sets.WaterTorches[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;

			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyBiomeTorch>());
            Item.width = 16;
			Item.height = 16;
			Item.holdStyle = 1;
			Item.flame = true;
			Item.noWet = true;
			Item.value = Item.buyPrice(copper: 40);
		}

		public override void HoldItem(Player player)
		{
			if (!player.wet)
			{
				Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
				
				float divide = 300f;
				Lighting.AddLight(position, new Vector3(245f / divide, 170f / divide, 113f / divide));
			}
		}

		public override void PostUpdate()
		{
			if (!Item.wet)
			{
				float divide = 300f;
				Lighting.AddLight(Item.Center, new Vector3(245f / divide, 170f / divide, 113f / divide));
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe(3)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 1)
			.AddIngredient(ItemID.Gel, 1)
            .Register();
        }
	}
}