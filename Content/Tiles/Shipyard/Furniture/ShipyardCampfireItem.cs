using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Shipyard.Furniture
{
	public class ShipyardCampfireItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShipyardCampfire>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BlackSandstoneItem>(), 10)
            .AddIngredient(ItemID.Torch, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}