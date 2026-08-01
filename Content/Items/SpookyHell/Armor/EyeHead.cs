using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class EyeHead : ModItem
	{
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
		}

		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 38;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);
			drawInfo.DrawDataCache.Add(drawData with { color = Color.White, texture = GlowTexture.Value });
			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<EyeBody>() && legs.type == ModContent.ItemType<EyeLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.EyeArmor");
			player.GetModPlayer<SpookyPlayer>().EyeArmorSet = true;
			player.whipRangeMultiplier += 0.15f;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.08f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:ShadowScales", 8)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 20)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 50)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
