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
using Spooky.Content.Items.Pets;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GiantShroomYellow1 : ModTile
    {
        private Asset<Texture2D> CapTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 400f;

            r = 255f / divide;
            g = 186f / divide;
            b = 0f / divide;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            if (Main.rand.NextBool(20))
            {
			    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<YellowSpore>());
            }
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
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Sin(-i / 8f + sin));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 + 2, 12), new Rectangle(0, 0, 26, 20), col, 0f, 
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

    public class GiantShroomYellow2 : GiantShroomYellow1
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
            AddMapEntry(new Color(208, 162, 44));
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
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Sin(-i / 8f + sin));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 - 8, 16), new Rectangle(0, 0, 52, 26), col, 0f, 
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

    public class GiantShroomYellow3 : GiantShroomYellow1
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
            AddMapEntry(new Color(208, 162, 44));
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
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Sin(-i / 8f + sin));

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

    public class GiantShroomYellow4 : GiantShroomYellow1
    {
        private Asset<Texture2D> CapTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
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
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
                Vector2 scale = new Vector2(1f, -MathF.Sin(-i / 8f + sin));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(CapTexRealWidth / 2 - 7, 6), new Rectangle(0, 0, 122, 46), col, 0f, 
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