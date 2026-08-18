using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class BigBoneMask : ModItem
	{
		private static Asset<Texture2D> HeadTexture;
		private static Asset<Texture2D> FlippedTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "Head");
			FlippedTexture = ModContent.Request<Texture2D>(Texture + "HeadFlipped");
		}

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			Rectangle frame = HeadTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			drawData = new DrawData(HeadTexture.Value, drawPos.Floor() + origin, frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			if (drawInfo.drawPlayer.direction == -1)
			{
				drawInfo.DrawDataCache.Add(drawData with { texture = FlippedTexture.Value });
			}
			else
			{
				drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value });
			}

			return false;
		}
	}
}