using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    public class WaxcapTreeSaplingItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WaxcapTreeSapling>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}