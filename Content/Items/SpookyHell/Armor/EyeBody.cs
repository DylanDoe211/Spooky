using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class EyeBody : ModItem
	{
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
		}

		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 36;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);
			drawInfo.DrawDataCache.Add(drawData with { color = Color.White, texture = GlowTexture.Value });
			return false;
		}

		public override void UpdateEquip(Player player) 
		{
			player.maxMinions += 1;
			player.maxTurrets += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:ShadowScales", 15)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 30)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 80)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
