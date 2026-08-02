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
		private static Asset<Texture2D> HatTexture;
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			HatTexture = ModContent.Request<Texture2D>(Texture + "Hat");
			GlowTexture = ModContent.Request<Texture2D>("Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHeadGlow");
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
			drawInfo.DrawDataCache.Add(drawData);
			drawInfo.DrawDataCache.Add(drawData with { color = Color.White, texture = GlowTexture.Value });

			//offset values
			int OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -4 : -8;
			Vector2 HeadOffset = new Vector2(0, OffsetY) * drawInfo.drawPlayer.Directions;

			//draw hat
			Rectangle frame = HatTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition + HeadOffset;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			drawData = new DrawData(HatTexture.Value, drawPos.Floor() + origin + new Vector2(0, OffsetY), frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

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