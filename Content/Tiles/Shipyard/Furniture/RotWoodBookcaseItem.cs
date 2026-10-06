using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Shipyard.Furniture
{
	public class RotWoodBookcaseItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RotWoodBookcase>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 20)
			.AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}