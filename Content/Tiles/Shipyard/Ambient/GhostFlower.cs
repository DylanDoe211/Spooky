using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Shipyard.Ambient
{
    public class GhostFlower : ModTile
    {
        private Asset<Texture2D> TileTexture;
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(198, 187, 225));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 450f;

			r = 198f / divide;
			g = 187f / divide;
			b = 225f / divide;
		}

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TileTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int TexRealWidth = TileTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 2f + cos));

                spriteBatch.Draw(TileTexture.Value, pos + new Vector2(TexRealWidth / 2 + 7, 34), new Rectangle(0, 0, 32, 32), col, 0f, 
                new Vector2(TexRealWidth, TileTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
                spriteBatch.Draw(GlowTexture.Value, pos + new Vector2(TexRealWidth / 2 + 7, 34), new Rectangle(0, 0, 32, 32), Color.White, 0f, 
                new Vector2(TexRealWidth, TileTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
			    Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            }

			return false;
        }
    }
}