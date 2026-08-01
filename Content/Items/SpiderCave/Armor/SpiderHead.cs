using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiderHead : ModItem
	{
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 28;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);
			drawInfo.DrawDataCache.Add(drawData with { color = Color.White, texture = GlowTexture.Value });
			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpiderBody>() && legs.type == ModContent.ItemType<SpiderLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SpiderArmor");
			player.GetModPlayer<SpookyPlayer>().SpiderSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetDamage(DamageClass.Generic) += 0.1f;
			player.nightVision = true;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:SilverBars", 12)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 20)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 100)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}