using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SporeShroomHead : ModItem
	{
		private static Asset<Texture2D> TopTexture;

		public override void Load()
		{
			TopTexture = ModContent.Request<Texture2D>(Texture + "Top");
		}

		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SporeShroomBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);

			//offset values
			int OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -20 : -24;
			Vector2 HeadOffset = new Vector2(3, OffsetY) * drawInfo.drawPlayer.Directions;

			//draw mushroom top
			Rectangle frame = TopTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition + HeadOffset;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			drawData = new DrawData(TopTexture.Value, drawPos.Floor() + origin, frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SporeShroomBody>() && legs.type == ModContent.ItemType<SporeShroomLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SporeShroomArmor");
			player.GetModPlayer<SpookyPlayer>().SporeShroomSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 10;
        }
	}
}