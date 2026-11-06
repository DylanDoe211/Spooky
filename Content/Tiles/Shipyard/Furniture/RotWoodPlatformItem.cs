using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Shipyard.Furniture
{
	public class RotWoodPlatformItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RotWoodPlatform>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 1)
            .Register();
        }
	}
}