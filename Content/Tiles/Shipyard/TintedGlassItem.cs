using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Shipyard
{
    public class TintedGlassItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TintedGlass>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BlackSandItem>(), 2)
            .AddTile(TileID.Furnaces)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TintedGlassWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}