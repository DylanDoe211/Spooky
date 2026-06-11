using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Slingshots;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Tiles.Painting;
using Spooky.Content.Tiles.SpiderCave.Furniture;

namespace Spooky.Content.Items.Quest
{
	public class OldHunterBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 38;
			Item.consumable = true;
			Item.rare = ItemRarityID.Quest;
			Item.maxStack = 9999;
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
			//weapon drops
			int[] MainItems = new int[]
			{
				ModContent.ItemType<ProSlingshot>(), 
				ModContent.ItemType<MagicBeanBag>(), 
				ModContent.ItemType<MetalFistBox>(), 
				ModContent.ItemType<PossessedCrown>(), 
				ModContent.ItemType<TrackingCrossbow>(), 
				ModContent.ItemType<WrestlingBelt>()
			};
			itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItems));

			//rewards for when all quests are done
			itemLoot.Add(ItemDropRule.ByCondition(new DropConditions.AllOldHunterQuestsDoneCondition(), ModContent.ItemType<OldHunterPaintingItem>(), 2));
			itemLoot.Add(ItemDropRule.ByCondition(new DropConditions.AllOldHunterQuestsDoneCondition(), ModContent.ItemType<OldHunterEnemyRemover>(), 1));

			//gnome homes
			int[] GnomeHomes = new int[] 
			{ 
				ModContent.ItemType<GnomeHouse1Item>(), 
				ModContent.ItemType<GnomeHouse2Item>(), 
				ModContent.ItemType<GnomeHouse3Item>(), 
				ModContent.ItemType<GnomeHouse4Item>() 
			};
			itemLoot.Add(ItemDropRule.OneFromOptions(1, GnomeHomes));

			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SporeEventStarter>(), 2));

			//gnome homes
			IItemDropRule[] Souls = new IItemDropRule[]
			{
				ItemDropRule.Common(ItemID.SoulofLight, 1, 10, 20),
				ItemDropRule.Common(ItemID.SoulofNight, 1, 10, 20),
			};
			itemLoot.Add(new OneFromRulesRule(1, Souls));

			//bars
			IItemDropRule[] Bars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.MythrilBar, 1, 12, 25),
				ItemDropRule.Common(ItemID.OrichalcumBar, 1, 12, 25),
				ItemDropRule.Common(ItemID.AdamantiteBar, 1, 12, 25),
				ItemDropRule.Common(ItemID.TitaniumBar, 1, 12, 25),
			};
			itemLoot.Add(new OneFromRulesRule(1, Bars));

			//potions
			IItemDropRule[] Potions = new IItemDropRule[]
			{
				ItemDropRule.Common(ItemID.BattlePotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.CalmingPotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.EndurancePotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.LuckPotionGreater, 1, 2, 6),
				ItemDropRule.Common(ItemID.IronskinPotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.LifeforcePotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.MagicPowerPotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.RegenerationPotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.SummoningPotion, 1, 2, 6),
				ItemDropRule.Common(ItemID.WrathPotion, 1, 2, 6)
			};
			itemLoot.Add(new OneFromRulesRule(1, Potions));

			//gold coins
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 1, 5, 10));
		}
	}
}