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
using Spooky.Content.Tiles.SpookyBiome;

using SpiritReforged.Common.WorldGeneration.Ecotones;

namespace Spooky.Content.Generation
{
	[ExtendsFromMod("SpiritReforged")]
	[JITWhenModsEnabled("SpiritReforged")]
	internal class ShipyardEcotone : EcotoneBase
	{
		//can be useful to block ecotone from certain biomes
		//public override HashSet<string> EcotoneEdgeBlocklist => ["Jungle", "Ocean"];

		//3.4.2.2118163272
		//3.4.1.1723835072

		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModLoader.HasMod("SpiritReforged");
		}

		protected override EcotoneIcon GetIcon() => EcotoneIcon.FromBiome<ShipyardBiome>();

		public static int LeftY = 0;
		public static int RightY = 0;

		private static bool IsEvilBiomeWall(int wall) => WallID.Sets.Corrupt[wall] || WallID.Sets.Crimson[wall];

		private static void GenerateShipyard(GenerationProgress progress, (int, int) bounds)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Shipyard").Value;

			Mod SpookyMod = Spooky.mod;

			int Seed = WorldGen.genRand.Next();

			int leftBound = bounds.Item1 - 6;
			int rightBound = bounds.Item2 + 6;

			bool OceanOnLeft = ((leftBound + rightBound) / 2) < (Main.maxTilesY / 2);

			if (OceanOnLeft)
			{
				leftBound = WorldGen.beachDistance - 40;
			}
			else
			{
				rightBound = Main.maxTilesX - (WorldGen.beachDistance - 40);
			}

			double heightLimit = Main.worldSurface * 0.35f;

			//when the ocean is on the left side of the world, use the left bound and keep going left until a sand tile on the ground is found
			if (OceanOnLeft)
			{
				bool foundSurfaceLeft = false;
				int attemptsLeft = 0;

				//get the two surface points, with one being at the ocean and the other just an edge of the cemetery
				while (!foundSurfaceLeft && attemptsLeft++ < 100000)
				{
					if ((WorldGen.SolidOrSlopedTile(leftBound, LeftY) && Cemetery.NoFloatingIsland(leftBound, LeftY)) || LeftY > Main.worldSurface)
					{
						foundSurfaceLeft = true;
					}
					else
					{
						LeftY++;
					}
				}
			}
			//otherwise just grab the edge of the cemetery
			else
			{
				//this needs to be the opposite, because if ocean is right, then the left edge of the shipyard is the right of the cemetery
				LeftY = Cemetery.RightY;
			}
			
			//when the ocean is on the right side of the world, use the right bound and keep going right until a sand tile on the ground is found
			if (!OceanOnLeft)
			{
				bool foundSurfaceRight = false;
				int attemptsRight = 0;

				while (!foundSurfaceRight && attemptsRight++ < 100000)
				{
					if ((WorldGen.SolidOrSlopedTile(rightBound, RightY) && Cemetery.NoFloatingIsland(rightBound, RightY)) || RightY > Main.worldSurface)
					{
						foundSurfaceRight = true;
					}
					else
					{
						RightY++;
					}
				}
			}
			//otherwise just grab the edge of the cemetery
			else
			{
				//this needs to be the opposite, because if ocean is left, then the right edge of the shipyard is the left of the cemetery
				RightY = Cemetery.LeftY;
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

			Vector2 Start2 = !OceanOnLeft ? new Vector2(leftBound, (int)Main.worldSurface) : new Vector2(leftBound, LeftY);
			Vector2 End2 = !OceanOnLeft ? new Vector2(rightBound, RightY) : new Vector2(rightBound, (int)Main.worldSurface);

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

				//create dirt blocks below so that any terrain below the biome is filled in
				for (int X = (int)Position.X - 15; X <= (int)Position.X + 15; X++)
				{
					int BottomEndPos = ((int)BottomPos.Y + 50) >= (int)Main.worldSurface ? (int)Main.worldSurface : (int)BottomPos.Y + 50;
					for (int Y = (int)BottomPos.Y; Y <= (int)Main.worldSurface; Y++)
					{
						if (!IsEvilBiomeWall(Main.tile[X, Y].WallType) && WorldGen.SolidTile(X, Y - 1))
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

				//place tiles below the line to create surface, and use noise to place clusters of black sandstone in the sand
				for (int Y = (int)Position.Y; Y <= (int)BottomPos.Y; Y++)
				{
					if (/*!IsEvilBiomeWall(Main.tile[(int)Position.X, Y].WallType) && */!InOcean((int)Position.X))
					{
						Main.tile[(int)Position.X, Y].ClearEverything();
						WorldGen.PlaceTile((int)Position.X, Y, ModContent.TileType<BlackSand>());
						WorldGen.PlaceWall((int)Position.X, Y, ModContent.WallType<BlackSandWall>());
					}
				}

				//create dithering on the edges of the biome
				for (int X = (int)Position.X - 15; X <= (int)Position.X + 15; X++)
				{
					for (int Y = (int)Position.Y; Y <= (int)BottomPos.Y + 10; Y++)
					{
						if (WorldGen.genRand.NextBool(10)/* && !IsEvilBiomeWall(Main.tile[X, Y].WallType)*/ && !InOcean(X))
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
					double SkyIslandCheckLimit = Main.worldSurface * 0.5f;

					if (Y < SkyIslandCheckLimit)
					{
						if (Cemetery.NoFloatingIsland((int)Position.X, Y))
						{
							Main.tile[(int)Position.X, Y].ClearEverything();
						}
					}
					else
					{
						Main.tile[(int)Position.X, Y].ClearEverything();
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
					if (!InOcean((int)Position.X))
					{
						if (Main.tile[(int)Position.X, Y].TileType == TileID.Ebonstone || Main.tile[(int)Position.X, Y].TileType == TileID.Crimstone)
						{
							Main.tile[(int)Position.X, Y].TileType = (ushort)ModContent.TileType<BlackSandstone>();
						}

						if (/*IsEvilBiomeWall(Main.tile[(int)Position.X, Y].WallType) && */Main.tile[(int)Position.X, Y].TileType != TileID.Ebonstone && Main.tile[(int)Position.X, Y].TileType != TileID.Crimstone)
						{
							Main.tile[(int)Position.X, Y].WallType = (ushort)ModContent.WallType<BlackSandstoneWall>();
						}
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

			//generate black sandstone with noise
			float sandScaleX = 110;
			float sandScaleY = 60;

			float sandThreshold = 0.1f;
			float caveThreshold = 0.01f;

			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.InWorld(X, Y, 5))
					{
						float noiseVal = SimplexNoise.FractalNoise(Seed, X / sandScaleX, Y / sandScaleY);
						if (noiseVal * noiseVal < sandThreshold)
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
						float noiseVal = SimplexNoise.FractalNoise(Seed, X / sandScaleX, Y / sandScaleY);
						if (noiseVal * noiseVal <= caveThreshold)
						{
							WorldGen.KillTile(X, Y);
						}
					}
				}
			}

			//cave cleanup
			for (int l = 0; l < 6; l++)
			{
				for (int X = leftBound - 10; X <= rightBound + 10; X++)
				{
					for (int Y = 10; Y <= Main.worldSurface; Y++)
					{
						int tileType = SpookyWorldMethods.GetNeighboringTileType(X, Y);
						if (tileType == ModContent.TileType<BlackSand>() || tileType == ModContent.TileType<BlackSandstone>())
						{
							int neighborCount = SpookyWorldMethods.GetNeighboringTileCount(X, Y);
							if (neighborCount > 4)
							{
								WorldGen.PlaceTile(X, Y, tileType);
							}
							else if (neighborCount < 4)
							{
								WorldGen.KillTile(X, Y);
							}
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

			//generate water and moss clusters inside of the cave
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.genRand.NextBool() && WorldGen.InWorld(X, Y, 5) && Main.tile[X, Y].WallType == ModContent.WallType<BlackSandstoneWall>())
					{
						WorldGen.PlaceLiquid(X, Y, 0, byte.MaxValue);
					}

					if (WorldGen.genRand.NextBool(160) && WorldGen.InWorld(X, Y, 5) && !WorldGen.SolidTile(X, Y - 1) && Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>())
					{
						int SizeX = WorldGen.genRand.Next(10, 17);
                        int SizeY = WorldGen.genRand.Next(10, 17);

						int[] ValidTiles = { ModContent.TileType<BlackSandstone>() };

						SpookyWorldMethods.PlaceOval(X, Y - 10, ModContent.TileType<BlackSandstoneMoss>(), ModContent.WallType<BlackSandWall>(),
						SizeX, SizeY, 1f, true, false, true, ValidTiles, false);
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

			Vector2 LighthousePos = Vector2.Zero;

			if (!OceanOnLeft)
			{
				LighthousePos = new Vector2(rightBound - 20, RightY); 
			}
			else
			{
				LighthousePos = new Vector2(leftBound + 20, LeftY);
			}

			//generate lighthouse inbetween the shipyard and ocean
			Vector2 BottomOrigin = new Vector2((int)LighthousePos.X - 9, (int)LighthousePos.Y - 14);
			StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/LighthouseBottom.shstruct", BottomOrigin.ToPoint16(), SpookyMod);

			int RandomHeight = WorldGen.genRand.Next(1, 4);
			Vector2 SegmentOrigin = Vector2.Zero;
			for (int i = 1; i <= RandomHeight + 1; i++)
			{
				SegmentOrigin = new Vector2((int)LighthousePos.X - 9, ((int)LighthousePos.Y - (10 * i)) - 14);
				StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/LighthouseSegment" + WorldGen.genRand.Next(1, 6) + ".shstruct", SegmentOrigin.ToPoint16(), SpookyMod);
			}

			Vector2 TopOrigin = new Vector2((int)SegmentOrigin.X - 3, (int)SegmentOrigin.Y - 27);
			StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/LighthouseTop.shstruct", TopOrigin.ToPoint16(), SpookyMod);

			//generate structures across the surface
			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

				if (WorldGen.genRand.NextBool(35))
				{
					int StructureY = (int)Position.Y;

					bool validSurface = false;
					int attemptsToPlace = 0;

					while (!validSurface && attemptsToPlace++ < 100000)
					{
						if (!WorldGen.SolidTile((int)Position.X, StructureY))
						{
							StructureY++;
						}
						else
						{
							validSurface = true;
						}
					}

					if (CanPlaceShipwreck((int)Position.X, StructureY, 20) && IsFlatSurface((int)Position.X, StructureY, 1) &&
					(Main.tile[(int)Position.X, StructureY].TileType == ModContent.TileType<BlackSand>() || Main.tile[(int)Position.X, StructureY].TileType == ModContent.TileType<BlackSandGrass>() || 
					Main.tile[(int)Position.X, StructureY].TileType == ModContent.TileType<BlackSandstone>()|| Main.tile[(int)Position.X, StructureY].TileType == ModContent.TileType<BlackSandstoneMoss>()))
					{
						switch (WorldGen.genRand.Next(4))
						{
							case 0:
							{
								Vector2 WreckOrigin = new Vector2((int)Position.X - 13, StructureY - 19);
								StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/WreckGiant" + WorldGen.genRand.Next(1, 3) + ".shstruct", WreckOrigin.ToPoint16(), SpookyMod);
								break;
							}
							case 1:
							{
								Vector2 WreckOrigin = new Vector2((int)Position.X - 9, StructureY - 11);
								StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/WreckMedium" + WorldGen.genRand.Next(1, 3) + ".shstruct", WreckOrigin.ToPoint16(), SpookyMod);
								break;
							}
							case 2:
							{
								Vector2 WreckOrigin = new Vector2((int)Position.X - 5, StructureY - 6);
								StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/WreckSmall" + WorldGen.genRand.Next(1, 3) + ".shstruct", WreckOrigin.ToPoint16(), SpookyMod);
								break;
							}
							case 3:
							{
								Vector2 WreckOrigin = new Vector2((int)Position.X - 2, StructureY - 6);
								StructureHelper.API.Generator.GenerateStructure("Content/Structures/Shipyard/WreckTiny" + WorldGen.genRand.Next(1, 3) + ".shstruct", WreckOrigin.ToPoint16(), SpookyMod);
								break;
							}
						}
					}
				}
			}

			//liquid settling
			SettleLiquids();

			//spread grass on black sandstone and slope blocks
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (Main.tile[X, Y].TileType != ModContent.TileType<RotWood>())
					{
						Tile.SmoothSlope(X, Y);
					}

					WorldGen.SpreadGrass(X, Y, ModContent.TileType<BlackSand>(), ModContent.TileType<BlackSandGrass>(), false);
				}
			}

			//place cemetery gravesites
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.genRand.NextBool(55) && IsFlatSurface(X, Y, 2))
                    {
                        for (int TombstoneX = X - 10; TombstoneX <= X + 10; TombstoneX++)
                        {
                            Tile tile = Main.tile[TombstoneX, Y];
                            if (IsShipyardTile(TombstoneX, Y) && WorldGen.SolidTile(TombstoneX, Y) && WorldGen.SolidTile(TombstoneX, Y + 1) && !WorldGen.SolidTile(TombstoneX, Y - 1) && tile.WallType <= 0)
                            {
                                WorldGen.PlaceWall(TombstoneX, Y - 2, ModContent.WallType<SpookyWoodFence>());
                                WorldGen.PlaceWall(TombstoneX, Y - 1, ModContent.WallType<SpookyWoodFence>());
                                WorldGen.PlaceWall(TombstoneX, Y, ModContent.WallType<SpookyWoodFence>());

                                if (WorldGen.SolidTile(TombstoneX, Y) && WorldGen.genRand.NextBool())
                                {
                                    TileGlobal.PlaceObject(TombstoneX, Y - 1, ModContent.TileType<ShipyardGravestone>(), true, WorldGen.genRand.Next(0, 5));
                                }
                            }
                        }
                    }
				}
			}

			//place chests
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.InWorld(X, Y, 10) && WorldGen.genRand.NextBool(20) && CanPlaceChest(X, Y) && 
					(Main.tile[X, Y].WallType == ModContent.WallType<BlackSandWall>() || Main.tile[X, Y].WallType == ModContent.WallType<BlackSandstoneWall>()))
					{
						TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<GiantPirateChest>());
					}
				}
			}

			//ambient tiles
			//first, grow trees and giant corals
			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					if (WorldGen.genRand.NextBool(5) && WorldGen.InWorld(X, Y, 10) && CanPlaceMangrove(X, Y) && WorldGen.SolidTile(X, Y) &&
					!WorldGen.SolidTile(X, Y - 1) && !WorldGen.SolidTile(X - 1, Y - 1) && !WorldGen.SolidTile(X + 1, Y - 1) &&
					!Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock && Main.tile[X, Y - 1].LiquidAmount <= 0 &&
					(Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>()))
					{
						MangroveTree.Grow(X, Y - 1, 5, 13);
					}

					if (WorldGen.genRand.NextBool() && WorldGen.InWorld(X, Y, 10) && CanPlaceCoralTree(X, Y) && WorldGen.SolidTile(X, Y) && //make sure the tree can place on a solid tile and not nearby other trees
					!WorldGen.SolidTile(X, Y - 1) && !WorldGen.SolidTile(X - 1, Y - 1) && !WorldGen.SolidTile(X + 1, Y - 1) && //make sure theres no tiles around where the tree will grow
					Main.tile[X, Y - 1].LiquidAmount > 0 && Main.tile[X, Y - 1].LiquidType == LiquidID.Water && //must be water above the tile it grows on
					!Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock && //tree cannot be placed on slopes
					(Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>() ||
					Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>()))
					{
						CoralTree.Grow(X, Y - 1, 5, 8, WorldGen.genRand.Next(0, 6));
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
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>() ||
						Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>())
						{
							//conch shells
							if (WorldGen.genRand.NextBool(6))
							{
								if (Main.tile[X, Y - 1].WallType <= 0)
								{
									TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<QueenShell>(), true, WorldGen.genRand.Next(0, 2));
								}
							}
							else
							{
								//giant mossy anchors
								if (WorldGen.genRand.NextBool())
								{
									ushort[] Anchors = new ushort[] { (ushort)ModContent.TileType<MossyAnchor1>(), (ushort)ModContent.TileType<MossyAnchor2>(), (ushort)ModContent.TileType<MossyAnchor3>() };
									TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Anchors), true);
								}
							}
						}
					}
				}
			}

			for (int X = leftBound - 10; X <= rightBound + 10; X++)
			{
				for (int Y = 10; Y <= Main.worldSurface; Y++)
				{
					Tile tileAbove = Main.tile[X, Y - 1];
					Tile tileBelow = Main.tile[X, Y + 1];

					if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>() && !tileBelow.HasTile && WorldGen.InWorld(X, Y, 10))
					{
						if (WorldGen.genRand.NextBool())
						{
							WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<BlackSandstoneMossVines>());
						}
					}
					//grow vines
					if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMossVines>())
					{
						int[] ValidTiles = { ModContent.TileType<BlackSandstoneMoss>() };

						SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<BlackSandstoneMossVines>(), ValidTiles);
					}

					if (Main.tile[X, Y].HasTile && !tileAbove.HasTile && WorldGen.InWorld(X, Y, 10))
					{
						//grow vines and weeds on black sandstone moss
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							if (WorldGen.genRand.NextBool())
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<BlackSandstoneMossWeeds>(), true, WorldGen.genRand.Next(0, 6));
							}
						}

						//grow bleached corals on all blocks
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>() ||
						Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							//giant bleached coral 
							int InWaterChance1 = tileAbove.LiquidAmount <= 0 ? 20 : 8;
							if (WorldGen.genRand.NextBool(InWaterChance1))
							{
								ushort[] GiantCorals = new ushort[] { (ushort)ModContent.TileType<BleachedCoralGiant1>(), (ushort)ModContent.TileType<BleachedCoralGiant2>(), (ushort)ModContent.TileType<BleachedCoralGiant3>(),
								(ushort)ModContent.TileType<BleachedCoralGiant4>(), (ushort)ModContent.TileType<BleachedCoralGiant5>(), (ushort)ModContent.TileType<BleachedCoralGiant6>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(GiantCorals), true);
							}

							//small bleached corals/starfishes
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

						//black sandstone pebbles
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>())
						{
							//rock piles
							if (WorldGen.genRand.NextBool(4))
							{
								ushort[] BigRockPiles = new ushort[] { (ushort)ModContent.TileType<BlacksandstoneRock1>(), (ushort)ModContent.TileType<BlacksandstoneRock2>(), 
								(ushort)ModContent.TileType<BlacksandstoneRock3>(), (ushort)ModContent.TileType<BlacksandstoneRock4>() };
								TileGlobal.PlaceObject(X, Y - 1, WorldGen.genRand.Next(BigRockPiles), true);
							}

							//small pebbles
							if (WorldGen.genRand.NextBool(3))
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<BlacksandstoneRockSmall>(), true, WorldGen.genRand.Next(0, 5));
							}
						}

						//grow pale sea oats
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>())
						{
							if (WorldGen.genRand.NextBool() && tileAbove.LiquidAmount <= 0)
							{
								TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<PaleSeaOats>(), true, WorldGen.genRand.Next(0, 14));
							}
						}

						//generate pots after everything else
						if (Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>() || 
						Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneSlab>() || 
						Main.tile[X, Y].TileType == ModContent.TileType<RotWood>())
						{
							if (WorldGen.genRand.NextBool() && !tileAbove.HasTile)
							{
								if (CanPlaceShipwreckPot(X, Y))
								{
									TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<ShipyardPotsWood>(), true, WorldGen.genRand.Next(0, 3));
								}
								else
								{
									TileGlobal.PlaceObject(X, Y - 1, ModContent.TileType<ShipyardPots>(), true, WorldGen.genRand.Next(0, 5));
								}
							}
						}
					}

					//place shelf corals on walls
					if ((Main.tile[X, Y].WallType == ModContent.WallType<BlackSandstoneWall>() || Main.tile[X, Y].WallType == ModContent.WallType<BlackSandWall>()) && WorldGen.InWorld(X, Y, 10))
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

		//check if the position is in the ocean
		public static bool InOcean(int X)
		{
			if (X < (WorldGen.beachDistance - 30) || X > Main.maxTilesX - (WorldGen.beachDistance - 30))
			{
				return true;
			}
			
			return false;
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

		public static bool IsFlatSurface(int PositionX, int PositionY, int Width)
		{
			for (int x = PositionX - Width; x <= PositionX + Width; x++)
			{
				if (Main.tile[x, PositionY].HasTile && !Main.tile[x, PositionY - 1].HasTile && !Main.tile[x, PositionY - 2].HasTile && !Main.tile[x, PositionY - 3].HasTile && !Main.tile[x, PositionY - 4].HasTile)
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;
		}
		
		public static bool CanPlaceShipwreck(int PositionX, int PositionY, int TileCheckDistance)
		{
			for (int x = PositionX - TileCheckDistance; x <= PositionX + TileCheckDistance; x++)
			{
				for (int y = PositionY - TileCheckDistance; y <= PositionY + TileCheckDistance; y++)
				{
					if (Main.tile[x, y].TileType == ModContent.TileType<RotWood>())
					{
						return false;
					}
				}
			}

			return true;
		}

		//dont allow wooden crate pots to place too far from shipwrecks
		public static bool CanPlaceShipwreckPot(int X, int Y)
        {
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<RotWood>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

		//dont allow coral trees to naturally grow too close to each other
		public static bool CanPlaceCoralTree(int X, int Y)
        {
            for (int i = X - 4; i < X + 4; i++)
            {
                for (int j = Y - 4; j < Y + 4; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<CoralTree>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		//determine if theres no chests nearby another chest thats about to place
        public static bool CanPlaceChest(int X, int Y)
        {
            for (int i = X - 20; i < X + 20; i++)
            {
                for (int j = Y - 25; j < Y + 25; j++)
                {
                    if (WorldGen.InWorld(i, j, 10) && Main.tile[i, j].TileType == ModContent.TileType<GiantPirateChest>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		public static bool IsShipyardTile(int X, int Y)
        {
            return Main.tile[X, Y].TileType == ModContent.TileType<BlackSand>() || 
            Main.tile[X, Y].TileType == ModContent.TileType<BlackSandGrass>() ||
            Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstone>() || 
            Main.tile[X, Y].TileType == ModContent.TileType<BlackSandstoneMoss>();
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