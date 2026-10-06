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
			HeadTexture = ModContent.Request<Texture2D>(Texture + "RealHead");
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
			//offset values
			int OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -4 : -8;
			Vector2 HeadOffset = new Vector2(0, OffsetY) * drawInfo.drawPlayer.Directions;

			//draw hat
			Rectangle frame = HeadTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition + HeadOffset;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			drawData = new DrawData(HeadTexture.Value, drawPos.Floor() + origin, frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<RotWoodBody>() && legs.type == ModContent.ItemType<RotWoodLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.RotWoodArmor");
			player.statDefense += 1;
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