using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyMush>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(188, 87, 82));
            RegisterItemDrop(ModContent.ItemType<SpookyMushItem>());
            DustType = DustID.Blood;
            HitSound = SoundID.Dig;
            MineResist = 0.1f;
		}

        public override bool CanExplode(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
			return true;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpookyMush>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<SpookyMush>();
		}

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288;
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

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
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

                //purple eye stalk
                if (Main.rand.NextBool(35))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkPurple1>(), (ushort)ModContent.TileType<EyeStalkPurple2>(), 
                    (ushort)ModContent.TileType<EyeStalkPurple3>(), (ushort)ModContent.TileType<EyeStalkPurple4>(),
                    (ushort)ModContent.TileType<EyeStalkPurple5>(), (ushort)ModContent.TileType<EyeStalkPurple6>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Stalks), true);
                }

                //ambient manhole teeth
                if (Main.rand.NextBool(35))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<Tooth>(), true);
                }

                //exposed nerve
                if (Main.hardMode && Main.rand.NextBool(1000))
                {
                    TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<ExposedNerveTile>(), true);
                }
            }

            //spread grass
            List<Point> adjacents = TileGlobal.OpenAdjacents(i, j, ModContent.TileType<SpookyMush>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (TileGlobal.HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<SpookyMushGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
        }
    }
}
