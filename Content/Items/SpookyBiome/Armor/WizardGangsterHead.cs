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
	public class WizardGangsterHead : ModItem, ISpecialArmorDraw
	{
		public string HeadTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHeadHat";

		public Vector2 Offset => new Vector2(0, 4f);

		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
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
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor");

            if (player.HasItem(ItemID.PlatinumCoin))
            {
                player.GetDamage(DamageClass.Magic) += 0.2f;
			}
			else
			{
				float bonusPerGold = 0.02f;
				int numGoldCoins = player.CountItem(ItemID.GoldCoin);

				if (numGoldCoins < 10)
				{
					player.GetDamage(DamageClass.Magic) += bonusPerGold * numGoldCoins;
				}
				else
				{
					player.GetDamage(DamageClass.Magic) += 0.2f;
				}
			}
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Magic) += 5;
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