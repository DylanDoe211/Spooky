using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert.Ambient;
using Spooky.Content.Tiles.Minibiomes.Desert.Furniture;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Content.Generation
{
	public class TarPits : ModSystem
	{
		private void PlaceTarPits(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.TarPits").Value;

			List<Point> BiomePoints = new List<Point>();
			List<Point> BiomePointsReal = new List<Point>();
			List<Point> BiomeSizes = new List<Point>();

			int Seed = WorldGen.genRand.Next();

			int BiomeX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
			int BiomeY = (int)((GenVars.desertHiveHigh + GenVars.desertHiveLow) / 1.75f);

			int SizeX = Main.maxTilesX / 55;
			int NumRows = Main.maxTilesY / 300;

			bool foundValidPosition = false;
            int Attempts = 0;

            while (!foundValidPosition && Attempts++ < 100000)
            {
				while (!CanPlaceBiome(BiomeX, BiomeY, SizeX, NumRows))
                {
                    BiomeY -= 20;
                }
                if (CanPlaceBiome(BiomeX, BiomeY, SizeX, NumRows))
                {
                    foundValidPosition = true;
                }
			}

			for (int i = BiomeX - SizeX; i < BiomeX + SizeX; i += 3)
			{
				for (int j = BiomeY; j < BiomeY + (30 * NumRows) - 10; j += 3)
				{
					SpookyWorldMethods.PlaceCircle(i, j - 20, ModContent.TileType<DesertSand>(), ModContent.WallType<DesertSandWall>(), WorldGen.genRand.Next(2, 7), true, false);
				}
			}

			for (int j = BiomeY; j < BiomeY + (30 * NumRows); j += 30)
			{
				for (int i = BiomeX - SizeX; i < BiomeX + SizeX; i++)
				{
					Tile tile = Main.tile[i, j];

					//first, do a distance check on each cave in a single row so they arent placed too close, also only check distance for caves on the same horizontal row
					bool tooClose = false;
					foreach (Point ExistingPosition in BiomePoints)
					{
						if (Vector2.Distance(new Vector2(i, j), ExistingPosition.ToVector2()) <= WorldGen.genRand.Next(10, 125) && ExistingPosition.Y == j)
						{
							tooClose = true;
						}
					}

					if (tile.TileType == ModContent.TileType<DesertSand>() && WorldGen.genRand.NextBool(4) && !tooClose)
					{
						//outputs a value between 0 - 0.5 based on noise
						float caveScale = 100;
						float noiseVal = SimplexNoise.Noise(Seed, i / caveScale, j / caveScale) * 0.5f;

						//takes the noise and creates a value between 0.9 - 1.4 to multiply the cave size by
						float noiseSizeMult = noiseVal + 0.9f;

						float CaveSizeX = WorldGen.genRand.Next(22, 57) * noiseSizeMult;
						float CaveSizeY = WorldGen.genRand.Next(15, 31) * noiseSizeMult;

						float RandomOffsetY = WorldGen.genRand.Next(0, 20) * noiseSizeMult;

						SpookyWorldMethods.PlaceMound(i, j + (int)RandomOffsetY, ModContent.TileType<DesertSand>(),
						ModContent.WallType<DesertSandWall>(), ((int)CaveSizeX / 2) + 15, (int)CaveSizeY + 6, false);

						BiomePoints.Add(new Point(i, j));
						BiomePointsReal.Add(new Point(i, j + (int)RandomOffsetY));
						BiomeSizes.Add(new Point((int)CaveSizeX, (int)CaveSizeY));
					}
				}
			}

			//hollow out each of the positions where caves are placed
			foreach (Point pos in BiomePointsReal)
			{
				Point size = BiomeSizes[BiomePointsReal.IndexOf(pos)];
				SpookyWorldMethods.PlaceMound(pos.X, pos.Y - 4, -1, ModContent.WallType<DesertSandWall>(), (size.X / 2) - 3, size.Y - 3, true);
			}

			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					if (WorldGen.genRand.NextBool(3) && CanPlaceTarLake(i, j) && Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() && 
					Main.tile[i, j - 3].WallType == ModContent.WallType<DesertSandWall>() && WorldGen.SolidTile(i, j) && !WorldGen.SolidTile(i, j - 1))
					{
						PlaceTarLake(i, j, 13, 15, 0.5f);
					}
				}
			}

			//place hallways connecting the cave points first so that it wont look bad with the actual caves
			foreach (Point pos in BiomePoints)
			{
				int NextCaveIndex = BiomePoints.IndexOf(pos) + 1;
				if (NextCaveIndex < BiomePoints.Count - 1 && NextCaveIndex % 2 == 0)
				{
					PlaceMineshaftTunnel(pos - new Point(0, 8), BiomePoints[NextCaveIndex] - new Point(0, 8));
				}
			}

			//tile cave cleanup
			for (int l = 0; l < 6; l++)
			{
				for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
				{
					for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
					{
						int tileType = SpookyWorldMethods.GetNeighboringTileType(i, j);
						if (tileType == ModContent.TileType<DesertSand>())
						{
							int neighborCount = SpookyWorldMethods.GetNeighboringTileCount(i, j);
							if (neighborCount > 4)
							{
								WorldGen.PlaceTile(i, j, tileType);
							}
							else if (neighborCount < 4)
							{
								WorldGen.KillTile(i, j);
							}
						}
					}
				}
			}

			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWood>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSand>())
					{
						if ((!Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile) || (!Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile))
						{
							WorldGen.KillTile(i, j);
						}
					}
				}
			}

			//place fossil plant ore
			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>())
					{
						Tile.SmoothSlope(i, j);

						if (WorldGen.genRand.NextBool(650) && CanPlaceOre(i, j))
						{
							if (Main.tile[i, j].TileType == ModContent.TileType<DesertSand>())
							{
								WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6, 9), WorldGen.genRand.Next(6, 9), ModContent.TileType<PlantFossil>(), false, 0f, 0f, false, true);
							}
						}
					}
				}
			}

			//generate clusters of sandstone with noise
			float sandstoneScaleX = 20;
			float sandstoneScaleY = 13;

			float wallScaleX = 40;
			float wallScaleY = 130;

			float sandstoneThreshold = 0.25f;
			float wallThreshold = 0.2f;

			//first destroy walls to create unique patterns
			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					float noiseVal = SimplexNoise.Noise(Seed, i / wallScaleX, j / wallScaleY);
					if (noiseVal * noiseVal > wallThreshold)
					{
						if (Main.tile[i, j].WallType == ModContent.WallType<DesertSandWall>())
						{
							WorldGen.KillWall(i, j);
						}
					}
				}
			}

			//place chunks of sandstone inside the sand
			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					//replace sandstone with sand using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].WallType == ModContent.WallType<DesertSandWall>())
					{
						float noiseVal = SimplexNoise.Noise(Seed, i / sandstoneScaleX, j / sandstoneScaleY);
						if (noiseVal * noiseVal < sandstoneThreshold)
						{
							Main.tile[i, j].TileType = (ushort)ModContent.TileType<DesertSandstone>();
							Main.tile[i, j].WallType = (ushort)ModContent.WallType<DesertSandstoneWall>();
						}
					}
				}
			}

			for (int i = BiomeX - SizeX - 50; i < BiomeX + SizeX + 50; i++)
			{
				for (int j = BiomeY - 50; j < BiomeY + (30 * NumRows) + 50; j++)
				{
					if (WorldGen.genRand.NextBool(3) && WorldGen.SolidTile(i, j) && Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() && !WorldGen.SolidTile(i + 1, j))
					{
						bool CanPlace = true;
						for (int CheckY = j; CheckY <= j + 6; CheckY++)
						{
							if (WorldGen.SolidTile(i + 1, CheckY) || Main.tile[i + 1, CheckY].LiquidAmount > 0)
							{
								CanPlace = false;
								break;
							}
						}

						if (CanPlace)
						{
							PlaceBridgeAcross(new Point(i + 1, j), WorldGen.genRand.NextBool(3));
						}
					}
				}
			}

			BiomeAmbience(BiomeX, BiomeY, SizeX, NumRows);
		}

		public void BiomeAmbience(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int i = PositionX - SizeX - 50; i < PositionX + SizeX + 50; i++)
			{
				for (int j = PositionY - 50; j < PositionY + (30 * SizeY) + 50; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>())
					{
						//cactuses
						if (WorldGen.genRand.NextBool(4) && CanPlaceCactus(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
						{
							TarPitCactus.Grow(i, j - 1, 5, 9);
						}

						//big piles
						if (WorldGen.genRand.NextBool(6))
						{
							ushort[] LargePiles = new ushort[] { (ushort)ModContent.TileType<DesertPileLarge1>(), (ushort)ModContent.TileType<DesertPileLarge2>(),
							(ushort)ModContent.TileType<DesertPileLarge3>(), (ushort)ModContent.TileType<DesertPileLarge4>(), (ushort)ModContent.TileType<DesertPileLarge5>() };

							TileGlobal.PlaceObject(i, j - 1, WorldGen.genRand.Next(LargePiles));
						}

						//small piles
						if (WorldGen.genRand.NextBool(6))
						{
							ushort[] SmallPiles = new ushort[] { (ushort)ModContent.TileType<DesertPileSmall1>(),
							(ushort)ModContent.TileType<DesertPileSmall2>(), (ushort)ModContent.TileType<DesertPileSmall3>() };

							TileGlobal.PlaceObject(i, j - 1, WorldGen.genRand.Next(SmallPiles));
						}

						//stalagmites and stalactites
						if (WorldGen.genRand.NextBool(3))
						{
							ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<DesertStalactite1>(), (ushort)ModContent.TileType<DesertStalactite2>(), (ushort)ModContent.TileType<DesertStalactite3>() };

							TileGlobal.PlaceObject(i, j + 1, WorldGen.genRand.Next(Stalactites));
						}
						if (WorldGen.genRand.NextBool(3))
						{
							ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<DesertStalagmite1>(), (ushort)ModContent.TileType<DesertStalagmite2>(), (ushort)ModContent.TileType<DesertStalagmite3>() };

							TileGlobal.PlaceObject(i, j - 1, WorldGen.genRand.Next(Stalagmites));
						}

						//place pots last
						if (WorldGen.genRand.NextBool() && !Main.tile[i, j - 1].HasTile)
						{
							TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<TarPitsPots>(), true, WorldGen.genRand.Next(0, 3));
						}
					}
				}
			}
		}

		public void PlaceMineshaftTunnel(Point start, Point end)
		{
			for (int i = start.X; i <= end.X; i += WorldGen.genRand.Next(2, 4))
			{
				int RandomOffset = WorldGen.genRand.Next(3, 6);
				if (Main.tile[i, start.Y - RandomOffset + 3].HasTile && Main.tile[i, start.Y - RandomOffset - 3].HasTile)
				{
					Vector2 Origin = new Vector2(i - 1, start.Y - RandomOffset);
					StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/Mineshaft" + WorldGen.genRand.Next(1, 9) + ".shstruct", Origin.ToPoint16(), Mod);
				}
			}
		}

		public void PlaceBridgeAcross(Point start, bool Minecart)
		{
			Point end = new Point();

			bool CanPlace = true;
			bool FoundPosAcross = false;
			int numTilesInGap = 0;

			//first, do a horizontal check until a wall on the other side is hit
			for (int i = start.X; i <= start.X + 50; i++)
			{
				if (WorldGen.SolidTile(i, start.Y) && !WorldGen.SolidTile(i - 1, start.Y) && numTilesInGap >= 30)
				{
					end = new Point(i, start.Y);
					FoundPosAcross = true;
					break;
				}
				else
				{
					numTilesInGap++;
				}
			}

			//if a position is found on the other side, do some additional checks
			if (FoundPosAcross)
			{
				//make sure the other side is also high enough from any ground below
				for (int CheckY = start.Y; CheckY <= start.Y + 6; CheckY++)
				{
					if (WorldGen.SolidTile(end.X - 1, CheckY) || Main.tile[end.X - 1, CheckY].LiquidAmount > 0)
					{
						CanPlace = false;
					}
				}

				//check if theres no other mineshaft hallways on both sides of where the bridge will be
				for (int i = start.X - 8; i <= start.X + 8; i++)
				{
					for (int j = start.Y - 8; j <= start.Y + 8; j++)
					{
						if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWood>())
						{
							CanPlace = false;
						}
					}
				}
				for (int i = end.X - 8; i <= end.X + 8; i++)
				{
					for (int j = end.Y - 8; j <= end.Y + 8; j++)
					{
						if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWood>())
						{
							CanPlace = false;
						}
					}
				}
			}

			//if it can place and a valid position is found then place everything
			if (FoundPosAcross && CanPlace)
			{
				Vector2 Origin1 = new Vector2(start.X - 3, start.Y - 3);
				StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/Mineshaft" + WorldGen.genRand.Next(1, 9) + ".shstruct", Origin1.ToPoint16(), Mod);

				Vector2 Origin2 = new Vector2(end.X + 3, start.Y - 3);
				StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/Mineshaft" + WorldGen.genRand.Next(1, 9) + ".shstruct", Origin2.ToPoint16(), Mod);

				int ExtraFluff = Minecart ? 2 : 1;
				for (int i = start.X - ExtraFluff; i <= end.X + ExtraFluff + 1; i++)
				{
					if (Minecart)
					{
						WorldGen.KillTile(i, start.Y - 1);
						WorldGen.KillTile(i, start.Y);
						WorldGen.KillTile(i, start.Y + 1);
						WorldGen.PlaceTile(i + 1, start.Y + 2, TileID.MinecartTrack);
					}
					else
					{
						WorldGen.KillTile(i, start.Y - 1);
						WorldGen.KillTile(i, start.Y);
						WorldGen.KillTile(i, start.Y + 1);
						WorldGen.KillTile(i, start.Y + 2);
						WorldGen.PlaceTile(i + 1, start.Y + 3, ModContent.TileType<OldWoodPlatform>());
					}
				}
			}
		}

		public void PlaceTarLake(int X, int Y, int radius, int radiusY, float thickMult)
		{
			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;

			for (int x = -radius - 2; x <= radius + 2; x++)
			{
				for (float y = 0; y <= radius + 4; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(5.5f, 6.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						WorldGen.PlaceTile(PositionX, PositionY, ModContent.TileType<DesertSand>());
						Main.tile[PositionX, PositionY].TileType = (ushort)ModContent.TileType<DesertSand>();
					}
				}
			}

			for (int x = -radius; x <= radius; x++)
			{
				for (float y = 0; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(5.5f, 6.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						if (Math.Sqrt(x * x + y * y) < radius - radialMod)
						{
							WorldGen.KillTile(PositionX, PositionY);
							tile.LiquidType = LiquidID.Water;
							tile.LiquidAmount = 255;
						}
					}
				}
			}
		}

		public bool CanPlaceTarLake(int PositionX, int PositionY)
		{
			for (int i = PositionX - 18; i <= PositionX + 18; i++)
			{
				for (int j = PositionY - 8; j <= PositionY + 20; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Water)
					{
						return false;
					}

					if (Main.tile[i, j].WallType != ModContent.WallType<DesertSandWall>())
					{
						return false;
					}

					if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWood>())
					{
						return false;
					}
				}
			}

			for (int i = PositionX - 3; i <= PositionX + 3; i++)
			{
				if (Main.tile[i, PositionY].TileType == ModContent.TileType<DesertSand>() && Main.tile[i, PositionY - 1].HasTile)
				{
					return false;
				}
			}

			return true;
		}

		//dont allow cactuses to naturally grow too close to each other
		public static bool CanPlaceCactus(int X, int Y)
		{
			for (int i = X - 4; i < X + 4; i++)
			{
				for (int j = Y - 4; j < Y + 4; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<TarPitCactus>())
					{
						return false;
					}
				}
			}

			return true;
		}

		//dont allow plant fossil to place too close to water
		public static bool CanPlaceOre(int X, int Y)
		{
			for (int i = X - 10; i < X + 10; i++)
			{
				for (int j = Y - 10; j < Y + 10; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Water)
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlaceBiome(int PositionX, int PositionY, int SizeX, int NumRows)
		{
			int numDesertTiles = 0;
			for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
			{
				for (int j = PositionY; j < PositionY + (30 * NumRows) - 10; j++)
				{
					int[] ValidWalls = { WallID.Sandstone, WallID.HardenedSand };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && ValidWalls.Contains(Main.tile[i, j].WallType))
					{ 
						numDesertTiles++;
					}

					int[] InvalidTiles = { ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombBrick2>() };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && InvalidTiles.Contains(Main.tile[i, j].TileType))
					{
						return false;
					}
				}
			}

			int AmountOfTilesNeeded = (SizeX * (30 * NumRows)) / 2;

			if (numDesertTiles >= AmountOfTilesNeeded)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex1 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Tar Pits", PlaceTarPits));
		}
	}
}