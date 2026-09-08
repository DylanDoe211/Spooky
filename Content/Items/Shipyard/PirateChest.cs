using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

namespace Spooky.Content.Items.Shipyard
{
	public class PirateChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 54;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 9999;
		}
		
		public override bool CanRightClick()
		{
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			/*
			//blocks
			IItemDropRule[] Blocks = new IItemDropRule[]
			{
				ItemDropRule.Common(ModContent.ItemType<EyeBlockItem>(), 1, 25, 55),
				ItemDropRule.Common(ModContent.ItemType<LivingFleshItem>(), 1, 25, 55),
				ItemDropRule.Common(ModContent.ItemType<SpookyMushItem>(), 1, 25, 55),
				ItemDropRule.Common(ModContent.ItemType<ValleyStoneItem>(), 1, 25, 55),
				ItemDropRule.Common(ModContent.ItemType<EyeSeed>(), 1, 1, 2)
			};
			itemLoot.Add(new FewFromRulesRule(2, 1, Blocks));
			*/
		}
	}
}