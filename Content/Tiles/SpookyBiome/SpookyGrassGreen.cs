using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Gourds;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrassGreen : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(78, 120, 48));
            RegisterItemDrop(ModContent.ItemType<SpookyDirtItem>());
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
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
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpookyDirt>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<SpookyDirt>();
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.NextBool(15)) 
                {
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<SpookyVinesGreen>(), true);
                    NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<SpookyWeedsGreen>(), true, Main.rand.Next(0, 10));
				}

                //grow colored gourds
                if (Main.rand.NextBool(40) && SpookyForest.CanGrowGourd(i, j))
                {
                    ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(), 
                    (ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(), 
                    (ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Gourds), true, Main.rand.Next(0, 2));
                }

                //grow rotten gourd
                if (Main.rand.NextBool(50) && SpookyForest.CanGrowRottenGourd(i, j))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<GourdRotten>(), true, Main.rand.Next(0, 2));
                }
            }

            //spread grass
            List<Point> adjacents = TileGlobal.OpenAdjacents(i, j, ModContent.TileType<SpookyDirt>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (TileGlobal.HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
        }
	}
}
