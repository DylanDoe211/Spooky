using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Content.Generation
{
	public class SpookyWorldMethods
	{
		public static void PlaceMound(int X, int Y, int tileType, int halfWidth, int height, bool DestroyOnly)
		{
			ShapeData mound = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Mound(halfWidth, height), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(mound)
			}));

			if (!DestroyOnly)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(mound), Actions.Chain(new GenAction[]
				{
					new Actions.ClearTile(), new Actions.PlaceTile((ushort)tileType)
				}));
			}
			else
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(mound), Actions.Chain(new GenAction[]
				{
					new Actions.ClearTile()
				}));
			}
		}

		public static void PlaceOval(int X, int Y, int tileType, int wallType, int radius, int radiusY, float thickMult, bool ReplaceOnly, bool DestroyOnly, bool SpreadOnSpecificTiles = false, int[] TypeToSpreadOn = null, bool RemoveLiquid = true)
		{
			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;
			for (int x = -radius; x <= radius; x++)
			{
				for (float y = -radius; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(2.5f, 4.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);
						Tile left = Framing.GetTileSafely(PositionX - 1, PositionY);
						Tile right = Framing.GetTileSafely(PositionX + 1, PositionY);
						Tile up = Framing.GetTileSafely(PositionX, PositionY - 1);
						Tile down = Framing.GetTileSafely(PositionX, PositionY + 1);
						Tile topLeft = Framing.GetTileSafely(PositionX - 1, PositionY - 1);
						Tile topRight = Framing.GetTileSafely(PositionX + 1, PositionY - 1);
						Tile bottomLeft = Framing.GetTileSafely(PositionX - 1, PositionY + 1);
						Tile bottomRight = Framing.GetTileSafely(PositionX + 1, PositionY + 1);

						if (!ReplaceOnly)
						{
							if (tileType > -1)
							{
								WorldGen.KillTile(PositionX, PositionY);
								tile.TileType = (ushort)tileType;
								tile.HasTile = true;
							}

							if (wallType > 0)
							{
								tile.WallType = (ushort)wallType;
								if (RemoveLiquid)
								{
									tile.LiquidAmount = 0;
								}
							}

							if (tileType == -1 && wallType == 0 && DestroyOnly)
							{
								WorldGen.KillTile(PositionX, PositionY);
								if (RemoveLiquid)
								{
									tile.LiquidAmount = 0;
								}
							}
						}
						else
						{
							if (WorldGen.SolidOrSlopedTile(PositionX, PositionY))
							{
								if (SpreadOnSpecificTiles)
								{
									if (!left.HasTile || !right.HasTile || !up.HasTile || !down.HasTile || !topLeft.HasTile || !topRight.HasTile || !bottomLeft.HasTile || !bottomRight.HasTile)
									{
										if (TypeToSpreadOn.Contains(tile.TileType))
										{
											tile.TileType = (ushort)tileType;
										}
									}
								}
								else
								{
									if (tileType > -1)
									{
										tile.TileType = (ushort)tileType;
									}
									else
									{
										WorldGen.KillTile(PositionX, PositionY);
									}
								}
							}

							if (wallType > 0 && tile.WallType > 0)
							{
								tile.WallType = (ushort)wallType;
								if (RemoveLiquid)
								{
									tile.LiquidAmount = 0;
								}
							}
						}

						//if (Math.Sqrt(x * x + y * y) >= radius - radialMod)
						//{
						//}
					}
				}
			}
		}

		public static void PlaceCircle(int X, int Y, int tileType, int wallType, int radius, bool clearTiles, bool clearWalls)
		{
			ShapeData circle = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(circle)
			}));

			//clear tiles
			if (clearTiles)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearTile(), new Actions.SetLiquid(0, 0)
				}));
			}

			//place tiles for the circle
			if (tileType > -1)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceTile((ushort)tileType)
				}));
			}

			//wall placing stuff
			ShapeData wallCircle = new ShapeData();
			GenAction wallBlotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius - 1), Actions.Chain(new GenAction[]
			{
				wallBlotchMod.Output(wallCircle)
			}));

			//clear walls
			if (clearWalls)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(wallCircle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearWall()
				}));
			}

			//dont place walls if it is not set to place any
			if (wallType > 0)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(wallCircle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceWall((ushort)wallType)
				}));
			}
		}

		public static void PlaceVines(int X, int Y, int vineType, int[] ValidTiles)
		{
			Tile tileBelow = Framing.GetTileSafely(X, Y + 1);
			if (!tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
			{
				int VineLength = WorldGen.genRand.Next(2, 13);
				bool PlaceVine = false;
				int Test = Y;
				while (Test > Y - VineLength)
				{
					Tile testTile = Framing.GetTileSafely(X, Test);
					if (testTile.BottomSlope)
					{
						break;
					}
					else if (!testTile.HasTile || !ValidTiles.Contains(testTile.TileType))
					{
						Test--;
						continue;
					}
					PlaceVine = true;
					break;
				}

				if (PlaceVine)
				{
					WorldGen.PlaceTile(X, Y + 1, vineType);
					tileBelow.TileType = (ushort)vineType;
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(X, Y + 1, true);
				}
			}
		}

		public static int GetNeighboringTileCount(int X, int Y)
		{
			int count = 0;
			for (int newX = X - 1; newX <= X + 1; newX++)
			{
				for (int newY = Y - 1; newY <= Y + 1; newY++)
				{
					if (newX != X || newY != Y)
					{
						if (WorldGen.SolidOrSlopedTile(newX, newY))
						{
							count++;
						}
					}
				}
			}

			return count;
		}

		public static ushort GetNeighboringTileType(int X, int Y)
		{
			ushort Type = 0;

			for (int newX = X - 1; newX <= X + 1; newX++)
			{
				for (int newY = Y - 1; newY <= Y + 1; newY++)
				{
					if (newX != X || newY != Y)
					{
						if (WorldGen.SolidOrSlopedTile(newX, newY))
						{
							Type = Framing.GetTileSafely(newX, newY).TileType;
						}
					}
				}
			}

			return Type;
		}
	}
}