using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
	[LegacyName("MushroomWhite")]
	public class MushroomTeal : ModTile
	{
		private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLighted[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(2, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(189, 213, 201));
			DustType = 288;
            HitSound = SoundID.Dig;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 450f;

			r = 189f / divide;
			g = 213f / divide;
			b = 201f / divide;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
        }

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			if (Flags.SporeEventHappening && Main.rand.NextBool(75) && !Main.tile[i, j - 1].HasTile && !Main.gamePaused && Main.instance.IsActive)
			{
				int newDust = Dust.NewDust(new Vector2((i) * 16, (j + 1) * 16), 1, 1, ModContent.DustType<MushroomSpore>());
				Main.dust[newDust].color = new Color(189, 213, 201);
			}
		}
	}
}