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

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GiantShroom1 : ModTile
    {
		private Asset<Texture2D> CapTexture;

		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 400f;

            r = 155f / divide;
            g = 83f / divide;
            b = 250f / divide;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int CapTexRealWidth = CapTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 - 7, 14), new Rectangle(0, 0, 52, 26), col, 0f, 
                new Vector2(CapTexRealWidth, CapTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
			    Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            }

			return true;
        }
    }

    public class GiantShroom2 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int CapTexRealWidth = CapTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 - 1, 18), new Rectangle(0, 0, 40, 20), col, 0f, 
                new Vector2(CapTexRealWidth, CapTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
			    Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            }
            
			return true;
        }
    }

    public class GiantShroom3 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int CapTexRealWidth = CapTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2, 14), new Rectangle(0, 0, 62, 30), col, 0f, 
                new Vector2(CapTexRealWidth, CapTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
			    Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            }
            
			return true;
        }
    }

    public class GiantShroom4 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int CapTexRealWidth = CapTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 + 3, 8), new Rectangle(0, 0, 84, 38), col, 0f, 
                new Vector2(CapTexRealWidth, CapTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
			    Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
            }
            
			return true;
        }
    }
}