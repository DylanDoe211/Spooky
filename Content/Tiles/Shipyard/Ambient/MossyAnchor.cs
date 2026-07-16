using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Shipyard.Ambient
{
	public class MossyAnchor1 : ModTile
	{
		private Asset<Texture2D> GlowTexture1;
		private Asset<Texture2D> GlowTexture2;

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(3, 3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(50, 46, 43));
			DustType = DustID.Ash;
			HitSound = SoundID.Dig;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture1 ??= ModContent.Request<Texture2D>(Texture + "Glow");
			GlowTexture2 ??= ModContent.Request<Texture2D>(Texture + "SandGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture1.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
			spriteBatch.Draw(GlowTexture2.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.15f);
        }
	}

	public class MossyAnchor2 : MossyAnchor1
	{
		private Asset<Texture2D> GlowTexture1;
		private Asset<Texture2D> GlowTexture2;

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture1 ??= ModContent.Request<Texture2D>(Texture + "Glow");
			GlowTexture2 ??= ModContent.Request<Texture2D>(Texture + "SandGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture1.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
			spriteBatch.Draw(GlowTexture2.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.15f);
        }
	}

	public class MossyAnchor3 : MossyAnchor1
	{
		private Asset<Texture2D> GlowTexture1;
		private Asset<Texture2D> GlowTexture2;

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture1 ??= ModContent.Request<Texture2D>(Texture + "Glow");
			GlowTexture2 ??= ModContent.Request<Texture2D>(Texture + "SandGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture1.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
			spriteBatch.Draw(GlowTexture2.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.15f);
        }
	}
}