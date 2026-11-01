using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class WizardGangsterHead2 : ModItem
	{
		private static Asset<Texture2D> HeadTexture;
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "_Head");
			GlowTexture = ModContent.Request<Texture2D>(Texture + "_HeadGlow");
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			Rectangle TextureRectangle = new(0, 0, 30, 61);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(1 * player.direction, (player.gravDir == -1 ? 11 : 0) + yOffset);
			drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });
			drawInfo.DrawDataCache.Add(drawData with { texture = GlowTexture.Value, color = Color.White, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<WizardGangsterBody>() && legs.type == ModContent.ItemType<WizardGangsterLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor2");

			if (player.HasItem(ItemID.PlatinumCoin))
			{
				player.manaCost -= 0.2f;
			}
			else
			{
				float bonusPerGold = 0.02f;
				int numGoldCoins = player.CountItem(ItemID.GoldCoin);

				if (numGoldCoins < 10)
				{
					player.manaCost -= bonusPerGold * numGoldCoins;
				}
				else
				{
					player.manaCost -= 0.2f;
				}
			}
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:GoldBars", 8)
			.AddIngredient(ItemID.Silk, 8)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}