using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class MineTimer : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MineDynamite>();
        }
        
		public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.rare = ItemRarityID.Quest;  
            Item.value = Item.buyPrice(copper: 50);
            Item.maxStack = 9999;
        }
	}
}
