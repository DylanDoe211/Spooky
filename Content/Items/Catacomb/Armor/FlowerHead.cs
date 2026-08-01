using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem
	{
		private static Asset<Texture2D> HeadTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "RealHead");
		}

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 38;
			Item.height = 36;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			Rectangle frame = HeadTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			float OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -4f : 8f;

			if (Main.mapFullscreen && drawInfo.drawPlayer.gravDir != 1)
			{
				//OffsetY = 0f;
			}

			drawData = new DrawData(HeadTexture.Value, drawPos.Floor() + origin + new Vector2(0, OffsetY), frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<FlowerBody>() && legs.type == ModContent.ItemType<FlowerLegs>();
		}

        public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.FlowerArmor");
            player.GetModPlayer<SpookyPlayer>().FlowerArmorSet = true;
            player.lifeRegen += 5;
		}

		public override void UpdateEquip(Player player)
        {
			player.manaCost -= 0.15f;
			player.maxMinions += 2;
			player.maxTurrets += 1;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}