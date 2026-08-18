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
	public class ZomboidNecromancerHood : ModItem
	{
		private static Asset<Texture2D> HatTexture;

		public override void Load()
		{
			HatTexture = ModContent.Request<Texture2D>(Texture + "Hat");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 32;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);

			//offset values
			int OffsetY = drawInfo.drawPlayer.gravDir == 1 ? -4 : -8;
			Vector2 HeadOffset = new Vector2(0, OffsetY) * drawInfo.drawPlayer.Directions;

			//draw hat
			Rectangle frame = HatTexture.Frame(1, 20, 0, drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height);
			Vector2 drawPos = drawInfo.Position - Main.screenPosition + new Vector2(drawInfo.drawPlayer.width / 2 - frame.Width / 2,
			drawInfo.drawPlayer.height - frame.Height + 4f) + drawInfo.drawPlayer.headPosition + HeadOffset;
			drawPos = drawPos.Floor();
			Vector2 origin = drawInfo.headVect;

			drawData = new DrawData(HatTexture.Value, drawPos.Floor() + origin, frame,
			drawData.color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect);
			drawData.shader = drawInfo.cHead;

			drawInfo.DrawDataCache.Add(drawData);

			return false;
		}
	}
}