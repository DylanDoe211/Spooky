using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Catacomb;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class CemeteryLampItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CemeteryLamp>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 15)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}