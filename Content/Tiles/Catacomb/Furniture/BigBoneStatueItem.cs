using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class BigBoneStatueItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BigBoneStatue>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 25)
			.AddIngredient(ModContent.ItemType<CatacombTorch2Item>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}