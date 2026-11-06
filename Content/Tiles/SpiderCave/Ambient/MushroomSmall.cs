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

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
    public class MushroomSmall : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateWidth = 20;
            TileObjectData.newTile.CoordinateHeights = [40];
            TileObjectData.newTile.DrawYOffset = -20;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(170, 162, 174));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            int variant = Main.tile[i, j].TileFrameX / 66;

			float divide = 500f;

            switch (variant)
            {
                //blue
                case 0:
                {
                    r = 17f / divide;
                    g = 98f / divide;
                    b = 255f / divide;
                    break;
                }
                //brown
                case 1:
                {
                    r = 205f / divide;
                    g = 131f / divide;
                    b = 106f / divide;
                    break;
                }
                //green
                case 2:
                {
                    r = 128f / divide;
                    g = 185f / divide;
                    b = 100f / divide;
                    break;
                }
                //purple
                case 3:
                {
                    r = 161f / divide;
                    g = 106f / divide;
                    b = 226f / divide;
                    break;
                }
                //red
                case 4:
                {
                    r = 243f / divide;
                    g = 89f / divide;
                    b = 89f / divide;
                    break;
                }
                //teal
                case 5:
                {
                    r = 189f / divide;
                    g = 213f / divide;
                    b = 201f / divide;
                    break;
                }
                //yellow
                case 6:
                {
                    r = 252f / divide;
                    g = 216f / divide;
                    b = 109f / divide;
                    break;
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - 2, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 22, 40), Color.White * 0.5f);
        }
    }
}