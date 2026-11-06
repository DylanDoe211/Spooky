using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Tiles.Shipyard;

namespace Spooky.Content.Items.Shipyard.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class RotWoodHead : ModItem
	{
		private static Asset<Texture2D> HeadTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "_Head");
		}

		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.White;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			Rectangle TextureRectangle = new(0, 0, 38, 57);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(-1 * player.direction, (player.gravDir == -1 ? 11 : 0) + yOffset);
			drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<RotWoodBody>() && legs.type == ModContent.ItemType<RotWoodLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.RotWoodArmor");
			player.statDefense += 2;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}