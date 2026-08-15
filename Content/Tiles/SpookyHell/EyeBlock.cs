using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(135, 7, 0));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
            MineResist = 0.7f;
		}

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(3) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
        }

        public override bool HasWalkDust()
        {
            return true;
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.Blood;
            makeDust = true;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
            Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope)
            {
                //grow vines
                if (Main.rand.NextBool(35)) 
                {
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<EyeVine>(), true);
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
				}

                //hanging arteries
                if (Main.rand.NextBool(35))
                {
                    ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<ArteryHanging1>(), (ushort)ModContent.TileType<ArteryHanging2>() };
                    TileGlobal.PlaceObject(i, j + 2, WorldGen.genRand.Next(Arteries), true);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0)
            {
                //grow small weeds
                if (Main.rand.NextBool(5))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<SpookyHellWeeds>(), true, Main.rand.Next(0, 6));
				}

                //eye stalks
                if (Main.rand.NextBool(20))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkThinShort>(), (ushort)ModContent.TileType<EyeStalkThin>(), 
                    (ushort)ModContent.TileType<EyeStalkThinTall>(), (ushort)ModContent.TileType<EyeStalkThinVeryTall>(),
                    (ushort)ModContent.TileType<EyeStalkSmall1>(), (ushort)ModContent.TileType<EyeStalkSmall2>(),
                    (ushort)ModContent.TileType<EyeStalkMedium1>(), (ushort)ModContent.TileType<EyeStalkMedium2>(),
                    (ushort)ModContent.TileType<EyeStalkBig1>(), (ushort)ModContent.TileType<EyeStalkBig2>(),
                    (ushort)ModContent.TileType<EyeStalkGiant1>(), (ushort)ModContent.TileType<EyeStalkGiant2>(),
                    (ushort)ModContent.TileType<EyeStalkPurple1>(), (ushort)ModContent.TileType<EyeStalkPurple2>(),
                    (ushort)ModContent.TileType<EyeStalkPurple3>(), (ushort)ModContent.TileType<EyeStalkPurple4>(),
                    (ushort)ModContent.TileType<EyeStalkPurple5>(), (ushort)ModContent.TileType<EyeStalkPurple6>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Stalks), true);
                }
            }
        }
    }
}
