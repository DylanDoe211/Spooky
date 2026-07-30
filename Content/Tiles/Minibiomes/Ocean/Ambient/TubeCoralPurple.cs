using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class TubeCoralPurple1 : ModTile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private Asset<Texture2D> PlantTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(153, 51, 178));
            HitSound = SoundID.Dig;
            DustType = -1;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralPurple");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int PlantTexRealWidth = PlantTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the coral, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                spriteBatch.Draw(PlantTexture.Value, pos + new Vector2(PlantTexRealWidth / 2 + 8, (PlantTexture.Height() / 3) / 2 + 19), new Rectangle(0, 0, 62, 64), col, 0f, 
                new Vector2(PlantTexRealWidth, PlantTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
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

    public class TubeCoralPurple2 : TubeCoralPurple1
    {
        private Asset<Texture2D> PlantTexture;
        
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralPurple");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int PlantTexRealWidth = PlantTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the coral, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                spriteBatch.Draw(PlantTexture.Value, pos + new Vector2(PlantTexRealWidth / 2 + 8, (PlantTexture.Height() / 3) / 2 + 19), new Rectangle(0, 66, 62, 64), col, 0f, 
                new Vector2(PlantTexRealWidth, PlantTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
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

    public class TubeCoralPurple3 : TubeCoralPurple1
    {
        private Asset<Texture2D> PlantTexture;

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Ambient/TubeCoralPurple");

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int PlantTexRealWidth = PlantTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw the coral, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                spriteBatch.Draw(PlantTexture.Value, pos + new Vector2(PlantTexRealWidth / 2 + 8, (PlantTexture.Height() / 3) / 2 + 19), new Rectangle(0, 66 * 2, 62, 64), col, 0f, 
                new Vector2(PlantTexRealWidth, PlantTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
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