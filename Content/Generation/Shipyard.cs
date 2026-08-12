using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Tiles.Shipyard;
using Spooky.Content.Tiles.Shipyard.Ambient;
using Spooky.Content.Tiles.Shipyard.Furniture;
using Spooky.Content.Tiles.Shipyard.Tree;
using SpiritReforged.Common.WorldGeneration.Ecotones;

namespace Spooky.Content.Generation
{
	[ExtendsFromMod("SpiritReforged")]
	[JITWhenModsEnabled("SpiritReforged")]
	internal class ShipyardEcotone : EcotoneBase
	{
		//can be useful to block ecotone from certain biomes
		//public override HashSet<string> EcotoneEdgeBlocklist => ["Jungle", "Ocean"];

		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModLoader.HasMod("SpiritReforged");
		}

		protected override EcotoneIcon GetIcon() => EcotoneIcon.FromBiome<ShipyardBiome>();

		static int LeftY = 0;
		static int RightY = 0;

		private static bool IsEvilBiomeWall(int wall) => WallID.Sets.Corrupt[wall] || WallID.Sets.Crimson[wall];

		private static void GenerateShipyard(GenerationProgress progress, (int, int) bounds)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Shipyard").Value;

			int leftBound = bounds.Item1 - 6;
			int rightBound = bounds.Item2 + 6;

			leftBound = Math.Max(leftBound, Main.offLimitBorderTiles + 12);
			rightBound = Math.Min(rightBound, Main.maxTilesX - Main.offLimitBorderTiles + 12);

			bool OceanOnLeft = ((leftBound + rightBound) / 2) < (Main.maxTilesY / 2);

			double heightLimit = Main.worldSurface * 0.35f;

			bool foundSurfaceLeft = false;
			int attemptsLeft = 0;

			//get the two surface points at the left and right of the cemetery biome
			while (!foundSurfaceLeft && attemptsLeft++ < 100000)
			{
				if (OceanOnLeft)
				{
					if (WorldGen.SolidTile(leftBound, LeftY) && Cemetery.NoFloatingIsland(leftBound, LeftY) && Main.tile[leftBound, LeftY].TileType != TileID.Sand)
					{
						LeftY = (int)heightLimit;
						leftBound -= 5;
					}
					if ((!WorldGen.SolidTile(leftBound, LeftY) || !Cemetery.NoFloatingIsland(leftBound, LeftY)) && LeftY <= Main.worldSurface)
					{
						LeftY++;
					}
					else
					{
						foundSurfaceLeft = true;
					}
				}
				else
				{
					if (WorldGen.SolidTile(leftBound, LeftY) && Cemetery.NoFloatingIsland(leftBound, LeftY) && !Cemetery.IsCemeteryTile(leftBound, LeftY))
					{
						LeftY = (int)heightLimit;
						leftBound -= 5;
					}
					if ((!Cemetery.IsCemeteryTile(leftBound, LeftY) || !Cemetery.NoFloatingIsland(leftBound, LeftY)) && LeftY <= Main.worldSurface)
					{
						LeftY++;
					}
					else
					{
						foundSurfaceLeft = true;
					}
				}
			}

			bool foundSurfaceRight = false;
			int attemptsRight = 0;

			while (!foundSurfaceRight && attemptsRight++ < 100000)
			{
				if (!OceanOnLeft)
				{
					if (WorldGen.SolidTile(rightBound, RightY) && Cemetery.NoFloatingIsland(rightBound, RightY) && Main.tile[rightBound, RightY].TileType != TileID.Sand)
					{
						RightY = (int)heightLimit;
						rightBound += 5;
					}
					if ((!WorldGen.SolidTile(rightBound, RightY) || !Cemetery.NoFloatingIsland(rightBound, RightY)) && RightY <= Main.worldSurface)
					{
						RightY++;
					}
					else
					{
						foundSurfaceRight = true;
					}
				}
				else
				{
					if (WorldGen.SolidTile(rightBound, RightY) && Cemetery.NoFloatingIsland(rightBound, RightY) && !Cemetery.IsCemeteryTile(rightBound, RightY))
					{
						RightY = (int)heightLimit;
						rightBound += 5;
					}
					if ((!Cemetery.IsCemeteryTile(rightBound, RightY) || !Cemetery.NoFloatingIsland(rightBound, RightY)) && RightY <= Main.worldSurface)
					{
						RightY++;
					}
					else
					{
						foundSurfaceLeft = true;
					}
				}
			}

			//create the terrain with bezier curves
			int segments = 0;
			for (int i = leftBound; i < rightBound; i++)
			{
				segments += 2;
			}

			Vector2 Start = new Vector2(leftBound, LeftY);
			Vector2 End = new Vector2(rightBound, RightY);

			Vector2 MiddlePoint = (Start + End) / 2;

			Vector2 p0 = End;
			Vector2 p1 = new Vector2(MiddlePoint.X - 30, End.Y + WorldGen.genRand.Next(-25, 26));
			Vector2 p2 = new Vector2(MiddlePoint.X + 30, Start.Y + WorldGen.genRand.Next(-25, 26));
			Vector2 p3 = Start;

			Vector2 Start2 = !OceanOnLeft ? new Vector2(leftBound, (int)Main.worldSurface) : new Vector2(leftBound, LeftY + 8);
			Vector2 End2 = !OceanOnLeft ? new Vector2(rightBound, RightY + 8) : new Vector2(rightBound, (int)Main.worldSurface);

			Vector2 p4 = End2;
			Vector2 p5 = Start2;

			//place terrain
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				float u = i / (float)segments;
				Vector2 BottomPos = BezierCurveUtil.CalculateBezierPoint(u, p4, p4, p5, p5);
				u = (i + 1) / (float)segments;

				//place tiles below the line to create surface, and use noise to place clusters of black sandstone in the sand
				for (int Y = (int)Position.Y; Y <= (int)BottomPos.Y; Y++)
				{
					if (Main.tile[(int)Position.X, Y].TileType != TileID.Sand && !IsEvilBiomeWall(Main.tile[(int)Position.X, Y].WallType))
					{
						Main.tile[(int)Position.X, Y].ClearEverything();
						WorldGen.PlaceTile((int)Position.X, Y, ModContent.TileType<BlackSand>());
						WorldGen.PlaceWall((int)Position.X, Y, ModContent.WallType<BlackSandWall>());
					}
				}

				//create dirt blocks below so that any terrain below the biome is filled in
				for (int X = (int)Position.X - 15; X <= (int)Position.X + 15; X++)
				{
					int BottomEndPos = ((int)BottomPos.Y + 30) >= (int)Main.worldSurface ? (int)Main.worldSurface : (int)BottomPos.Y + 30;
					for (int Y = (int)BottomPos.Y; Y <= BottomEndPos; Y++)
					{
						if (!IsEvilBiomeWall(Main.tile[X, Y].WallType))
						{
							//destroy any non-solid tiles
							if (!WorldGen.SolidTile(X, Y) && Main.tile[X, Y].TileType != ModContent.TileType<BlackSand>())
							{
								WorldGen.KillTile(X, Y);
								WorldGen.PlaceTile(X, Y, TileID.Dirt);
							}
							//replace empty space with dirt
							if (Main.tile[X, Y].WallType <= 0)
							{
								WorldGen.PlaceTile(X, Y, TileID.Dirt);
							}
						}
					}
				}

				//create dithering on the edges of the biome
				for (int X = (int)Position.X - 15; X <= (int)Position.X + 15; X++)
				{
					for (int Y = (int)Position.Y; Y <= (int)BottomPos.Y + 10; Y++)
					{
						if (WorldGen.genRand.NextBool(10) && !IsEvilBiomeWall(Main.tile[X, Y].WallType))
						{
							if (WorldGen.SolidTile(X, Y) && Main.tile[X, Y].TileType != TileID.Sand && Main.tile[X, Y].TileType != ModContent.TileType<BlackSand>())
							{
								Main.tile[X, Y].ClearEverything();
								WorldGen.PlaceTile(X, Y, ModContent.TileType<BlackSand>());
								WorldGen.PlaceWall(X, Y, ModContent.WallType<BlackSandWall>());
							}
						}
					}
				}

				//clear all tiles above the surface line
				for (int Y = (int)heightLimit; Y < (int)Position.Y; Y++)
				{
					Main.tile[(int)Position.X, Y].ClearEverything();
				}
			}

			//generate lakes across the surface
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				for (int Y = (int)Position.Y - 10; Y <= (int)Position.Y + 2; Y++)
				{
					if (WorldGen.genRand.NextBool(55) && CanPlaceNearCemetery((int)Position.X, Y, 50) &&
					WorldGen.InWorld((int)Position.X, Y, 10) && Main.tile[(int)Position.X, Y].HasTile && !Main.tile[(int)Position.X, Y - 1].HasTile)
					{
						PlaceLake((int)Position.X, Y + 2);
					}
				}
			}

			//initial wall cleanup and evil biome tile conversion
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				float u = i / (float)segments;
				Vector2 BottomPos = BezierCurveUtil.CalculateBezierPoint(u, p4, p4, p5, p5);
				u = (i + 1) / (float)segments;

				//convert any evil blocks/walls into black sandstone
				for (int Y = (int)Position.Y - 10; Y <= (int)BottomPos.Y + 10; Y++)
				{
					if (Main.tile[(int)Position.X, Y].TileType == TileID.Ebonstone || Main.tile[(int)Position.X, Y].TileType == TileID.Crimstone)
					{
						Main.tile[(int)Position.X, Y].TileType = (ushort)ModContent.TileType<BlackSandstone>();
					}

					if (IsEvilBiomeWall(Main.tile[(int)Position.X, Y].WallType) && Main.tile[(int)Position.X, Y].TileType != TileID.Ebonstone && Main.tile[(int)Position.X, Y].TileType != TileID.Crimstone)
					{
						Main.tile[(int)Position.X, Y].WallType = (ushort)ModContent.WallType<BlackSandstoneWall>();
					}
				}

				for (int Y = (int)Position.Y - 10; Y <= (int)Position.Y + 12; Y++)
				{
					//kill walls not surrounded by enough tiles
					if (ShouldDestroyWall((int)Position.X, Y, 2))
					{
						WorldGen.KillWall((int)Position.X, Y);
					}
				}
			}

			int seed = WorldGen.genRand.Next();

			//generate black sandstone with noise
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.InWorld(X, Y, 5))
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 1500f, Y / 320f, 5, seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 1500f, Y / 320f, 5, unchecked(seed - 1)) + 0.5f;
						float noiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float StoneThreshold = horizontalOffsetNoise * 4f + 0.3f;

						//replace tiles/walls with black sandstone
						if (noiseMap * noiseMap < StoneThreshold)
						{
							if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>())
							{
								Main.tile[X, Y].TileType = (ushort)ModContent.TileType<BlackSandstone>();
							}
							if (Main.tile[X, Y].WallType == ModContent.WallType<BlackSandWall>())
							{
								Main.tile[X, Y].WallType = (ushort)ModContent.WallType<BlackSandstoneWall>();
							}
						}
					}
				}
			}

			//generate caves inside of black sandstone
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.InWorld(X, Y, 5) && Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 1500f, Y / 320f, 5, seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 1500f, Y / 320f, 5, unchecked(seed - 1)) + 0.5f;
						float noiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float CaveThreshold = horizontalOffsetNoise * 2f + 0.1f;

						if (noiseMap * noiseMap <= CaveThreshold)
						{
							WorldGen.KillTile(X, Y);
						}
					}
				}
			}

			//tile cleanup
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				float u = i / (float)segments;
				Vector2 BottomPos = BezierCurveUtil.CalculateBezierPoint(u, p4, p4, p5, p5);
				u = (i + 1) / (float)segments;

				for (int Y = (int)Position.Y - 20; Y <= (int)BottomPos.Y + 10; Y++)
				{
 					//clean tiles that are sticking out (basically tiles only attached to one tile on one side)
					bool OnlyRight = !Main.tile[(int)Position.X, Y - 1].HasTile && !Main.tile[(int)Position.X, Y + 1].HasTile && !Main.tile[(int)Position.X - 1, Y].HasTile;
					bool OnlyLeft = !Main.tile[(int)Position.X, Y - 1].HasTile && !Main.tile[(int)Position.X, Y + 1].HasTile && !Main.tile[(int)Position.X + 1, Y].HasTile;
					bool OnlyDown = !Main.tile[(int)Position.X, Y - 1].HasTile && !Main.tile[(int)Position.X - 1, Y].HasTile && !Main.tile[(int)Position.X + 1, Y].HasTile;
					bool OnlyUp = !Main.tile[(int)Position.X, Y + 1].HasTile && !Main.tile[(int)Position.X - 1, Y].HasTile && !Main.tile[(int)Position.X + 1, Y].HasTile;

					if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
					{
						WorldGen.KillTile((int)Position.X, Y);
					}

					//kill random single floating tiles
					if (!Main.tile[(int)Position.X, Y - 1].HasTile && !Main.tile[(int)Position.X, Y + 1].HasTile && 
					!Main.tile[(int)Position.X - 1, Y].HasTile && !Main.tile[(int)Position.X + 1, Y].HasTile)
					{
						WorldGen.KillTile((int)Position.X, Y);
					}

					//kill one block thick surfaces
					if (Main.tile[(int)Position.X, Y].HasTile && !Main.tile[(int)Position.X, Y - 1].HasTile && !Main.tile[(int)Position.X, Y + 1].HasTile)
					{
						WorldGen.KillTile((int)Position.X, Y);
					}

					//get rid of single tiles on the ground since it looks weird
					if (Main.tile[(int)Position.X, Y].HasTile && !Main.tile[(int)Position.X - 1, Y].HasTile && !Main.tile[(int)Position.X + 1, Y].HasTile)
					{
						WorldGen.KillTile((int)Position.X, Y);
					}
				}
			}

			//generate water inside of the caves
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.genRand.NextBool() && WorldGen.InWorld(X, Y, 5) && 
					Main.tile[X, Y].WallType == ModContent.WallType<BlackSandstoneWall>() && CanPlaceNearCemetery(X, Y, 4, false))
					{
						WorldGen.PlaceLiquid(X, Y, 0, byte.MaxValue);
					}
				}
			}

			//spread grass on black sandstone and slope blocks
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					Tile.SmoothSlope(X, Y);

					if (Main.tile[X, Y].WallType > 0)
					{
						WorldGen.SpreadGrass(X, Y, ModContent.TileType<BlackSandstone>(), ModContent.TileType<BlackSandstoneMoss>(), false);
					}
				}
			}

			//additional wall cleanup
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				float u = i / (float)segments;
				Vector2 BottomPos = BezierCurveUtil.CalculateBezierPoint(u, p4, p4, p5, p5);
				u = (i + 1) / (float)segments;

				for (int Y = (int)Position.Y - 10; Y <= (int)BottomPos.Y + 10; Y++)
				{
					//kill random single floating walls
					if (Main.tile[(int)Position.X, Y - 1].WallType <= 0 && Main.tile[(int)Position.X, Y + 1].WallType <= 0 && 
					Main.tile[(int)Position.X - 1, Y].WallType <= 0 && Main.tile[(int)Position.X + 1, Y].WallType <= 0)
					{
						WorldGen.KillWall((int)Position.X, Y);
					}
				}
			}

			//liquid settling
			SettleLiquids();

			//ambient tiles
			//first, grow trees
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.genRand.NextBool(5) && WorldGen.InWorld(X, Y, 10) && CanPlaceMangrove(X, Y) && WorldGen.SolidTile(X, Y) &&
					!WorldGen.SolidTile(X, Y - 1) && !WorldGen.SolidTile(X - 1, Y - 1) && !WorldGen.SolidTile(X + 1, Y - 1) &&
					!Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock && Main.tile[X, Y - 1].LiquidAmount <= 0 &&
					(Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>()))
					{
						MangroveTree.Grow(X, Y - 1, 5, 13);
					}

					if (WorldGen.genRand.NextBool() && WorldGen.InWorld(X, Y, 10) && CanPlaceCoralTree(X, Y) && WorldGen.SolidTile(X, Y) && //make sure the tree can place on a solid tile and not nearby other trees
					!WorldGen.SolidTile(X, Y - 1) && !WorldGen.SolidTile(X - 1, Y - 1) && !WorldGen.SolidTile(X + 1, Y - 1) && //make sure theres no tiles around where the tree will grow
					Main.tile[X, Y - 1].LiquidAmount > 0 && Main.tile[X, Y - 1].LiquidType == LiquidID.Water && //must be water above the tile it grows on
					!Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock && //tree cannot be placed on slopes
					(Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() ||
					Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>()))
					{
						int[] Types = new int[] { ModContent.TileType<CoralTreeBlue>(), ModContent.TileType<CoralTreeGreen>(), 
						ModContent.TileType<CoralTreePink>(), ModContent.TileType<CoralTreePurple>(), ModContent.TileType<CoralTreeTeal>() };

						CoralTreeBlue.Grow(X, Y - 1, 5, 8, WorldGen.genRand.Next(Types));
					}
				}
			}
			//then place the rest of the ambient tiles
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					Tile tileAbove = Main.tile[X, Y - 1];

					if (Main.tile[X, Y].HasTile && !tileAbove.HasTile && WorldGen.InWorld(X, Y, 10))
					{
						//grow bleached corals on all blocks
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() ||
						Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							//giant mossy anchors
							if (WorldGen.genRand.NextBool())
							{
								ushort[] Anchors = new ushort[] { (ushort)ModContent.TileType<MossyAnchor1>(), (ushort)ModContent.TileType<MossyAnchor2>(), (ushort)ModContent.TileType<MossyAnchor3>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Anchors), true);
							}

							//giant bleached coral 
							int InWaterChance1 = tileAbove.LiquidAmount <= 0 ? 20 : 8;
							if (WorldGen.genRand.NextBool(InWaterChance1))
							{
								ushort[] GiantCorals = new ushort[] { (ushort)ModContent.TileType<BleachedCoralGiant1>(), (ushort)ModContent.TileType<BleachedCoralGiant2>(), (ushort)ModContent.TileType<BleachedCoralGiant3>(),
								(ushort)ModContent.TileType<BleachedCoralGiant4>(), (ushort)ModContent.TileType<BleachedCoralGiant5>(), (ushort)ModContent.TileType<BleachedCoralGiant6>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(GiantCorals), true);
							}

							//small bleached corals/stafishes
							int InWaterChance2 = tileAbove.LiquidAmount <= 0 ? 8 : 2;
							if (WorldGen.genRand.NextBool(InWaterChance2))
							{
								if (WorldGen.genRand.NextBool())
								{
									TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<BleachedCoral>(), true, WorldGen.genRand.Next(0, 8));
								}
								else
								{
									TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<PaleStarfish>(), true, WorldGen.genRand.Next(0, 4));
								}
							}
						}

						//black sand only ambient tiles
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>())
						{
							//sand piles, only place above water
							if (WorldGen.genRand.NextBool(3) && tileAbove.LiquidAmount <= 0)
							{
								ushort[] SandPiles = new ushort[] { (ushort)ModContent.TileType<BlackSandPile1>(), (ushort)ModContent.TileType<BlackSandPile2>(), (ushort)ModContent.TileType<BlackSandPile3>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(SandPiles), true);
							}
						}

						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							//ghost flowers 
							int InWaterChance = tileAbove.LiquidAmount <= 0 ? 8 : 5;
							if (WorldGen.genRand.NextBool(InWaterChance))
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<GhostFlower>(), true);
							}
						}

						//black sandstone pebbles
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							//rock piles
							int Chance1 = Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>() ? 15 : 4;
							if (WorldGen.genRand.NextBool(Chance1))
							{
								ushort[] BigRockPiles = new ushort[] { (ushort)ModContent.TileType<BlacksandstoneRock1>(), (ushort)ModContent.TileType<BlacksandstoneRock2>(), 
								(ushort)ModContent.TileType<BlacksandstoneRock3>(), (ushort)ModContent.TileType<BlacksandstoneRock4>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(BigRockPiles), true);
							}

							//small pebbles
							int Chance2 = Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>() ? 8 : 3;
							if (WorldGen.genRand.NextBool(Chance2))
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<BlacksandstoneRockSmall>(), true, WorldGen.genRand.Next(0, 5));
							}
						}

						//grow pale sea oats
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>())
						{
							if (WorldGen.genRand.NextBool() && tileAbove.LiquidAmount <= 0)
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<PaleSeaOats>(), true, WorldGen.genRand.Next(0, 14));
							}
						}

						//grow mossy weeds
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							if (WorldGen.genRand.NextBool())
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<BlackSandstoneMossWeeds>(), true, WorldGen.genRand.Next(0, 6));
							}
						}

						//generate pots after everything else
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || 
						Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							if (WorldGen.genRand.NextBool() && !tileAbove.HasTile)
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<ShipyardPots>(), true, WorldGen.genRand.Next(0, 5));
							}
						}
					}

					//place shelf corals on walls
					if (Main.tile[X, Y].WallType == ModContent.WallType<BlackSandstoneWall>() && WorldGen.InWorld(X, Y, 10))
					{
						if (WorldGen.genRand.NextBool(150))
						{
							TileGlobal.PlaceObject(X, Y, ModContent.TileType<ShelfCoralSmall>(), true, WorldGen.genRand.Next(0, 3));
						}
						if (WorldGen.genRand.NextBool(135))
						{
							TileGlobal.PlaceObject(X, Y, ModContent.TileType<ShelfCoralLarge>(), true, WorldGen.genRand.Next(0, 3));
						}
					}
				}
			}
		}

		public static void SettleLiquids()
		{
			Liquid.QuickWater(3);
			WorldGen.WaterCheck();
			int num = 0;
			Liquid.quickSettle = true;
			int num2 = 10;
			while (num < num2)
			{
				int num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
				num++;
				double num4 = 0.0;
				int num5 = num3 * 5;
				while (Liquid.numLiquid > 0)
				{
					num5--;
					if (num5 < 0)
					{
						break;
					}

					double num6 = (double)(num3 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (double)num3;
					if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num3)
					{
						num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					}

					if (num6 > num4)
					{
						num4 = num6;
					}
					else
					{
						num6 = num4;
					}

					int num7 = 10;
					if (num > num7)
					{
						num7 = num;
					}

					Liquid.UpdateLiquid();
				}

				WorldGen.WaterCheck();
			}

			Liquid.quickSettle = false;
		}

		private static bool CanGenerate(out (int, int) bounds)
		{
			bounds = (0, 0);

			//shipyard ecotone should only generate if surrounded by the cemetery and the ocean
			if (EcotoneSurfaceMapping.FindWhere(x => x.SurroundedBy("Ocean", "Cemetery") &&
			EcotoneSurfaceMapping.OnSurface(x), false) is EcotoneSurfaceMapping.EcotoneEntry entry &&
			!entry.Definition.Ecotone)
			{
				bounds = (entry.Start.X, entry.End.X);
				return true;
			}

			return false;
		}

		//shamelessly copy pasted from the generation used for vanilla oasis lakes
		public static void PlaceLake(int X, int Y)
		{
			int num2 = WorldGen.genRand.Next(28, 55);
			int oasisHeight = 20;
			int num3 = num2 + 50;
			int num6 = Y;
			int num7 = num2 / 2;
			int num8 = X - num2 * 1;
			int num9 = X + num2 * 1;
			int num10 = Y - oasisHeight * 4;
			int num11 = Y + oasisHeight * 3;
			if (num8 < 0)
			{
				num8 = 0;
			}
			if (num9 > Main.maxTilesX)
			{
				num9 = Main.maxTilesX;
			}
			if (num10 < 0)
			{
				num10 = 0;
			}
			if (num11 > Main.maxTilesY)
			{
				num11 = Main.maxTilesY;
			}
			for (int m = num8; m < num9; m++)
			{
				for (int n = num10; n < num11; n++)
				{
					double num12 = (double)Math.Abs(m - X) * 0.7;
					double num13 = (double)Math.Abs(n - Y) * 1.35;
					double num14 = Math.Sqrt(num12 * num12 + num13 * num13);
					double num15 = (double)num7 * (0.53 + WorldGen.genRand.NextDouble() * 0.04);
					double num16 = (double)Math.Abs(m - X) / (double)(num9 - X);
					num16 = 1.0 - num16;
					num16 *= 2.3;
					num16 *= num16;
					num16 *= num16;
					if (num14 < num15)
					{
						if (n == Y + 1)
						{
							Main.tile[m, n].LiquidAmount = 127;
						}
						else if (n > Y + 1)
						{
							Main.tile[m, n].LiquidAmount = byte.MaxValue;
						}
						WorldGen.KillTile(m, n);
						WorldGen.KillWall(m, n);
						WorldGen.KillWall(m, n + 1);
					}
					else if (n < Y && num12 < num15 + (double)(Math.Abs(n - Y) * 3) * num16)
					{
						if (Main.tile[m, n].TileType == ModContent.TileType<BlackSand>())
						{
							WorldGen.KillTile(m, n);
						}
					}
					else if (n >= Y && num12 < num15 + (double)Math.Abs(n - Y) * num16)
					{
						if (Main.tile[m, n].HasTile && Main.tileSolid[Main.tile[m, n].TileType] && !Main.tileSolidTop[Main.tile[m, n].TileType])
						{
							continue;
						}
						if (Main.tile[m, n].LiquidAmount <= 0 && !WorldGen.SolidTile(m, n) && Main.tile[m, n].WallType < 0)
						{
							WorldGen.PlaceTile(m, n, ModContent.TileType<BlackSand>());
						}
					}
				}
			}
		}

		//check if a lake can be placed
		public static bool CanPlaceNearCemetery(int X, int Y, int Dist, bool LiquidCheck = true)
		{
			for (int i = X - Dist; i < X + Dist; i++)
			{
				for (int j = Y - Dist; j < Y + Dist; j++)
				{
					if (WorldGen.InWorld(i, j))
					{
						if ((LiquidCheck && Main.tile[i, j].LiquidAmount > 0) || Cemetery.IsCemeteryTile(i, j))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public static bool ShouldDestroyWall(int X, int Y, int Dist)
		{
			for (int i = X - Dist; i <= X + Dist; i++)
			{
				for (int j = Y - Dist; j <= Y + Dist; j++)
				{
					if (!WorldGen.SolidOrSlopedTile(i, j))
					{
						return true;
					}
				}
			}

			return false;
		}

		//dont allow trees to naturally grow too close to each other
		public static bool CanPlaceMangrove(int X, int Y)
        {
            for (int i = X - 4; i < X + 4; i++)
            {
                for (int j = Y - 4; j < Y + 4; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<MangroveTree>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
		
		//dont allow coral trees to naturally grow too close to each other
		public static bool CanPlaceCoralTree(int X, int Y)
        {
            for (int i = X - 4; i < X + 4; i++)
            {
                for (int j = Y - 4; j < Y + 4; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<CoralTreeBlue>() || Main.tile[i, j].TileType == ModContent.TileType<CoralTreeGreen>() ||
					Main.tile[i, j].TileType == ModContent.TileType<CoralTreePink>() || Main.tile[i, j].TileType == ModContent.TileType<CoralTreePurple>() || Main.tile[i, j].TileType == ModContent.TileType<CoralTreeTeal>()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		public override void AddTasks(List<GenPass> tasks, List<EcotoneSurfaceMapping.EcotoneEntry> entries)
		{
			if (tasks.FindIndex(x => x.Name == "Waterfalls") is int index && index != -1)
			{
				tasks.Insert(index, new EcotonePass("Shipyard", Generation, this));
			}
		}

		private static void Generation(GenerationProgress progress, GameConfiguration configuration)
		{
			if (!CanGenerate(out var bounds))
			{
				return;
			}

			GenerateShipyard(progress, bounds);
		}
	}	
}