using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Generation
{
    //tile conversions for clentaminator solutions
    public class TileConversionMethods
    {
		public static int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>(),
		ModContent.TileType<DampGrass>(), ModContent.TileType<DampMushroomGrass>() };
		public static int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>(), ModContent.TileType<DampSoil>() };
		public static int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>(), ModContent.TileType<DampStone>() };
		public static int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>(), ModContent.WallType<DampGrassWall>() };
		public static int[] DirtWallReplace = { ModContent.WallType<CemeteryDirtWall>(), ModContent.WallType<DampSoilWall>(), ModContent.WallType<SpookyDirtWall>() };
		public static int[] StoneWallReplace = { ModContent.WallType<SpookyStoneWall>() };

		//convert spooky mod blocks into purity when used with green or brown solution
		public static void ConvertSpookyIntoPurity(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with regular grass
                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Grass;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with stone
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Stone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

						//replace spooky grass walls with grass walls
						if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.GrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt walls with dirt walls
						if (DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.DirtUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone walls with grass walls
						if (StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.Stone;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into hallowed blocks when used with light blue solution
        public static void ConvertSpookyIntoHallow(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with hallowed grass
                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.HallowedGrass;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with pearlstone
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Pearlstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with hallowed grass walls
                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.HallowedGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt walls with dirt walls
						if (DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.DirtUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone walls with hallowed stone walls
						if (StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.PearlstoneBrickUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into corrupt blocks when used with purple solution
        public static void ConvertSpookyIntoCorruption(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with corrupt grass
                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.CorruptGrass;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with ebonstone
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Ebonstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with corrupt grass walls
                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.CorruptGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt walls with dirt walls
						if (DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.DirtUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone walls with corrupt stone walls
						if (StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.EbonstoneUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into crimson blocks when used with red solution
        public static void ConvertSpookyIntoCrimson(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with crimson grass
                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.CrimsonGrass;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with crimstone
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Crimstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with crimson grass walls
                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.CrimsonGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt walls with dirt walls
						if (DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.DirtUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone walls with crimson stone walls
						if (StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.CrimstoneUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into snow blocks when used with white solution
        public static void ConvertSpookyIntoSnow(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses and dirt with snow
                        if (GrassReplace.Contains(Main.tile[k, l].TileType) || DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.SnowBlock;
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with ice
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.IceBlock;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass/dirt walls with snow walls
                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType) || DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.SnowWallUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone walls with ice walls
						if (StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.IceUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into desert blocks when used with yellow solution
        public static void ConvertSpookyIntoDesert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses and dirt with sand
                        if (GrassReplace.Contains(Main.tile[k, l].TileType) || DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
							if (!Main.tile[k, l + 1].HasTile)
							{
								Main.tile[k, l].TileType = TileID.HardenedSand;
							}
							else
							{
								Main.tile[k, l].TileType = TileID.Sand;
							}

							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace spooky stone with sandstone
                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Sandstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with sandstone walls
                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType) || StoneWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.Sandstone;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt walls with snow walls
						if (DirtWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.HardenedSand;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert blocks into spooky forest ones with orange solution
        public static void ConvertPurityIntoSpooky(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
                        //replace corrupt biome grasses with green grass
                        int[] GreenGrassReplace = { TileID.CorruptGrass, TileID.CrimsonGrass };

                        if (GreenGrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
						else
						{
							if (TileID.Sets.Conversion.Grass[Main.tile[k, l].TileType] || TileID.Sets.Conversion.GolfGrass[Main.tile[k, l].TileType])
							{
								Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrass>();
								WorldGen.SquareTileFrame(k, l);
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
						}

                        //replace dirt blocks with spooky dirt
						if (Main.tile[k, l].TileType == TileID.Dirt) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyDirt>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace stone blocks with spooky stone
						if (TileID.Sets.Conversion.Stone[Main.tile[k, l].TileType])
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyStone>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace grass walls with spooky grass walls
						if (WallID.Sets.Conversion.Grass[Main.tile[k, l].WallType]) 
                        {
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<SpookyGrassWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

						//replace dirt walls with spooky dirt walls
						if (WallID.Sets.Conversion.Dirt[Main.tile[k, l].WallType])
						{
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<SpookyDirtWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}

        //convert blocks into swampy cemetery ones with dark green solution
        public static void ConvertPurityIntoCemetery(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
						if (TileID.Sets.Conversion.Grass[Main.tile[k, l].TileType] || TileID.Sets.Conversion.GolfGrass[Main.tile[k, l].TileType])
						{
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryGrass>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

						//replace dirt blocks with cemetery dirt
						if (Main.tile[k, l].TileType == TileID.Dirt) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryDirt>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace stone blocks with cemetery stone
						if (TileID.Sets.Conversion.Stone[Main.tile[k, l].TileType]) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryStone>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

						//replace grass walls with cemetery grass walls
						if (WallID.Sets.Conversion.Grass[Main.tile[k, l].WallType])
						{
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

						//replace dirt walls with cemetery dirt walls
						if (WallID.Sets.Conversion.Dirt[Main.tile[k, l].WallType])
						{
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<CemeteryDirtWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}

        //convert underworld blocks into valley of eyes blocks with red & purple solution
        public static void ConvertHellIntoEyeValley(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
						if (Main.tile[k, l].TileType == TileID.Ash || Main.tile[k, l].TileType == TileID.AshGrass)
                        {
							if (!Main.tile[k - 1, l].HasTile || !Main.tile[k + 1, l].HasTile || !Main.tile[k, l - 1].HasTile || !Main.tile[k, l + 1].HasTile)
							{
								Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyMushGrass>();
								WorldGen.SquareTileFrame(k, l);
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
							else
							{
								Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyMush>();
								WorldGen.SquareTileFrame(k, l);
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
						}
					}
				}
			}
		}
    }
}