using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeballBlock : ModTile
	{
        public override string Texture => "Spooky/Content/Tiles/SpookyHell/LivingFlesh";

        private Asset<Texture2D> EyeTexture;
		private Asset<Texture2D> EyeGlowTexture;

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(145, 24, 12));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
		}

		//cannot be sloped at all
		public override bool Slope(int i, int j)
		{
			return false;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) 
		{
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomSolid);
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
		{
			EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/EyeBallBlockDraw");
			EyeGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/EyeBallBlockDrawGlow");

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

			int frame = tile.TileFrameNumber % 3;

			spriteBatch.Draw(EyeTexture.Value, pos + new Vector2(EyeTexture.Width() / 2 - 6, EyeTexture.Height() / 2 - 6), new Rectangle(0, 28 * frame, 28, 28), col, 0f, 
			new Vector2(EyeTexture.Width() / 2, EyeTexture.Height() / 2), 1f, SpriteEffects.None, 0f);

			spriteBatch.Draw(EyeGlowTexture.Value, pos + new Vector2(EyeTexture.Width() / 2 - 6, EyeTexture.Height() / 2 - 6), new Rectangle(0, 28 * frame, 28, 28), TileGlobal.GetTileColorWithPaint(i, j, Color.White), 0f, 
			new Vector2(EyeTexture.Width() / 2, EyeTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
		}
    }
}
