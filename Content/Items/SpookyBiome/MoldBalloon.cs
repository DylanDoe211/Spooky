using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Items.SpookyBiome
{
	[AutoloadEquip(EquipType.Balloon)]
	public class MoldBalloon : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 28;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetJumpState<MoldJarJump>().Enable();
			player.jumpBoost = true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MoldJar>(), 1)
            .AddIngredient(ItemID.ShinyRedBalloon)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
	}
}