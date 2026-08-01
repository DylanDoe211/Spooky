using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Items.Costume;
using Spooky.Content.Tiles.Shipyard;
using Spooky.Content.Tiles.Shipyard.Tree;

namespace Spooky.Content.Items.Fishing.Crate
{
	public class ShipyardCrate2 : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.IsFishingCrate[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShipyardCrate>();
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShipyardCrate2Tile>());
            Item.width = 34;
			Item.height = 34;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) 
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
		}

		public override bool CanRightClick() 
		{
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			//drop vanilla bars
			IItemDropRule[] oreBars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.CobaltBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.PalladiumBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.MythrilBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.OrichalcumBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.AdamantiteBar, 1, 2, 5),
				ItemDropRule.Common(ItemID.TitaniumBar, 1, 2, 5),
			};
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

			//drop some potions
			IItemDropRule[] explorationPotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 3),
			};
			itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

			//healing and mana potions
			IItemDropRule[] resourcePotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 6),
				ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 6),
			};

			itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

			//fishing bait
			IItemDropRule[] highendBait = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 6),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			};
			itemLoot.Add(new OneFromRulesRule(2, highendBait));

			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlackSandItem>(), 2, 45, 85));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlackSandstoneItem>(), 2, 45, 85));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MangroveSaplingItem>(), 5, 1, 2));

			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoyaiMask>(), 15));

            //coins
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 1, 7));
		}
	}
}