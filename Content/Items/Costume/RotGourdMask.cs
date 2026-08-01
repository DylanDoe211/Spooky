using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class RotGourdMask : ModItem
	{
		private static Asset<Texture2D> TopTexture;

		public override void Load()
		{
			TopTexture = ModContent.Request<Texture2D>(Texture + "Top");
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 34;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);

			//draw hat
			Rectangle frame = TopTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			float OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -4f : 8f;

			if (Main.mapFullscreen && drawInfo.drawPlayer.gravDir != 1)
			{
				//OffsetY = 0f;
			}

			drawData = new DrawData(TopTexture.Value, drawPos.Floor() + origin + new Vector2(0, OffsetY), frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

			return false;
		}
	}
}