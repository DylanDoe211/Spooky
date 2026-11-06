using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class MineMetalPlates : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MinePressureSensor>();
        }

		public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 32;
            Item.rare = ItemRarityID.Quest;  
            Item.value = Item.buyPrice(copper: 50);
            Item.maxStack = 9999;
        }
	}
}
