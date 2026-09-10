using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs;

namespace Spooky.Content.Items.Shipyard
{
	public class PiratePotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 20;
			ItemID.Sets.DrinkParticleColors[Type] = [new Color(69, 164, 142)];
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<PiratePotionBuff>();
			Item.buffTime = 7200;
		}
    }
}