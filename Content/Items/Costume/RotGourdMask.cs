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
		private static Asset<Texture2D> HeadTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "_Head");
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
			Rectangle TextureRectangle = new(0, 0, 26, 53);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(1 * player.direction, (player.gravDir == -1 ? 11 : 0) + yOffset);
			drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}
	}
}