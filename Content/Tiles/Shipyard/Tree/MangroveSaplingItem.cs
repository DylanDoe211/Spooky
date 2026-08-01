using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Shipyard.Tree
{
    public class MangroveSaplingItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MangroveSapling>());
            Item.width = 16;
			Item.height = 16;
        }

        public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 3);
			return null;
		}
    }
}