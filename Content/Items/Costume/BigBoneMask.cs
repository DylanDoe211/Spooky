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
			HeadTexture = ModContent.Request<Texture2D>(Texture + "_Head");
			FlippedTexture = ModContent.Request<Texture2D>(Texture + "_HeadFlipped");
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
			Rectangle TextureRectangle = new(0, 0, 52, 49);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(0, (player.gravDir == -1 ? 11 : 0) + yOffset);

			Texture2D texToUse = drawInfo.drawPlayer.direction == -1 ? FlippedTexture.Value : HeadTexture.Value;

			drawInfo.DrawDataCache.Add(drawData with { texture = texToUse, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}
	}
}