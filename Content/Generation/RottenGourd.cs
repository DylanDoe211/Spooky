using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome.GourdBlocks;

namespace Spooky.Content.Generation;

internal class RottenGourd
{
	/// <summary>
	/// Succeeds only if placing over a wall of type <paramref name="wall"/>.
	/// </summary>
	/// <param name="wall"></param>
	public class CheckWall(int wall) : GenCondition
	{
		private readonly int Wall = wall;

		protected override bool CheckValidity(int x, int y)
		{
			if (!WorldGen.InWorld(x, y, 10))
				return false;

			if (_tiles[x, y].WallType == Wall)
				return true;

			return false;
		}
	}

	/// <summary>
	/// Places a "rotten gourd" microbiome at the given location.
	/// </summary>
	/// <param name="point"></param>
	public static void Place(Point16 point)
	{
		int baseWidth = WorldGen.genRand.Next(9, 14);
		int baseHeight = baseWidth + WorldGen.genRand.Next(-2, 4);
		int id = WorldGen.genRand.Next(8);

		int tileType = id switch
		{
			0 => ModContent.TileType<GourdBlockGreen>(),
			1 => ModContent.TileType<GourdBlockLime>(),
			2 => ModContent.TileType<GourdBlockWhite>(),
			3 => ModContent.TileType<GourdBlockLimeOrange>(),
			4 => ModContent.TileType<GourdBlockOrange>(),
			5 => ModContent.TileType<GourdBlockRed>(),
			6 => ModContent.TileType<GourdBlockYellow>(),
			_ => ModContent.TileType<GourdBlockYellowGreen>(),
		};

		int wallType = id switch
		{
			0 => ModContent.WallType<GourdBlockGreenWall>(),
			1 => ModContent.WallType<GourdBlockLimeWall>(),
			2 => ModContent.WallType<GourdBlockWhiteWall>(),
			3 => ModContent.WallType<GourdBlockLimeOrangeWall>(),
			4 => ModContent.WallType<GourdBlockOrangeWall>(),
			5 => ModContent.WallType<GourdBlockRedWall>(),
			6 => ModContent.WallType<GourdBlockYellowWall>(),
			_ => ModContent.WallType<GourdBlockYellowGreenWall>(),
		};

		int vineType = id switch
		{
			0 => ModContent.TileType<GourdVinesGreen>(),
			1 => ModContent.TileType<GourdVinesLime>(),
			2 => ModContent.TileType<GourdVinesWhite>(),
			3 => ModContent.TileType<GourdVinesOrange>(), //lime and orange
			4 => ModContent.TileType<GourdVinesOrange>(),
			5 => ModContent.TileType<GourdVinesRed>(),
			6 => ModContent.TileType<GourdVinesYellow>(),
			_ => ModContent.TileType<GourdVinesYellow>(), //yellow and green
		};

		ShapeData shapes = new();

		Point gourdPos = point.ToPoint();
		int reps = Math.Min(WorldGen.genRand.Next(1, 5), WorldGen.genRand.Next(1, 5));
		WorldUtils.Gen(gourdPos, new Shapes.Circle(baseWidth, baseHeight), Actions.Chain(new Modifiers.Blotches(2, 0.4f), new Actions.ClearTile(), new Actions.PlaceWall((ushort)wallType)).Output(shapes));
		Point off = new Point(0, 0);

		for (int i = 0; i < reps; i++)
		{
			off.X += WorldGen.genRand.Next(-2, 3);
			off.Y -= (int)WorldGen.genRand.NextFloat(baseHeight * 0.8f, baseHeight * 1.2f);
			baseWidth = (int)(baseWidth * WorldGen.genRand.NextFloat(0.55f, 0.8f));
			baseHeight = (int)(baseHeight * WorldGen.genRand.NextFloat(0.55f, 0.8f));

			if (WorldGen.InWorld(gourdPos.X, gourdPos.Y, 50))
			{			
				WorldUtils.Gen(gourdPos, new Shapes.Circle(baseWidth, baseHeight), Actions.Chain(new Modifiers.Offset(off.X, off.Y), new Modifiers.Blotches(2, 0.4f), 
				new Actions.ClearTile(), new Actions.PlaceWall((ushort)wallType)).Output(shapes));
			}
		}

		HashSet<Point16> data = shapes.GetData();
		Point16[] positions = new Point16[data.Count];
		data.CopyTo(positions);

		int wallReps = WorldGen.genRand.Next(4, 6);
		for (int i = 0; i < wallReps; i++)
		{
			ShapeData tileData = new();
			GenAction chain = i == 0 ? Actions.Chain(new Actions.PlaceTile((ushort)tileType)).Output(tileData) : 
			Actions.Chain(new Modifiers.Conditions(new CheckWall(wallType)), new Actions.ClearTile(), new Actions.PlaceTile((ushort)tileType)).Output(tileData);

			WorldUtils.Gen(point.ToPoint(), new ModShapes.OuterOutline(shapes, true), chain);
			shapes = tileData;
		}

		if (WorldGen.InWorld(gourdPos.X, gourdPos.Y, 50))
		{
			WorldUtils.Gen(gourdPos, new Shapes.Circle((int)(baseWidth * WorldGen.genRand.NextFloat(0.4f, 0.55f)), (int)(baseHeight * WorldGen.genRand.NextFloat(0.4f, 0.55f)) + 1),
			Actions.Chain(new Modifiers.Offset(off.X, off.Y - baseHeight - 3), new Actions.ClearTile(), new Modifiers.Blotches(2, 0.4f), new Actions.PlaceTile(TileID.LivingWood)));
		}

		for (int i = gourdPos.X - 20; i <= gourdPos.X + 20; i++)
		{
			for (int j = gourdPos.Y - 10; j <= gourdPos.Y + 30; j++)
			{
				if (WorldGen.InWorld(i, j, 50))
				{
					Tile tile = Main.tile[i, j];
					Tile tileBelow = Main.tile[i, j + 1];

					if (tile.WallType == wallType && !tile.HasTile && tileBelow.TileType == tileType)
					{
						int gutsTileVariant = id switch
						{
							0 => WorldGen.genRand.Next(0, 3),
							1 => WorldGen.genRand.Next(3, 6),
							2 => WorldGen.genRand.Next(12, 15),
							3 => WorldGen.genRand.Next(6, 9), //lime and orange
							4 => WorldGen.genRand.Next(6, 9),
							5 => WorldGen.genRand.Next(9, 12),
							6 => WorldGen.genRand.Next(15, 18),
							_ => WorldGen.genRand.Next(15, 18), //yellow and green
						};
						TileGlobal.PlaceObject(i, j, ModContent.TileType<GourdGuts>(), true, gutsTileVariant);
					}
				}
			}

			for (int j = gourdPos.Y - 30; j <= gourdPos.Y + 5; j++)
			{
				if (WorldGen.InWorld(i, j, 50))
				{
					Tile tile = Main.tile[i, j];
					Tile tileAbove = Main.tile[i, j - 1];

					if (tile.WallType == wallType && !tile.HasTile && tileAbove.TileType == tileType)
					{
						for (int vineY = 0; vineY <= 10; vineY++)
						{
							if (!Main.tile[i, j + vineY].HasTile)
							{
								WorldGen.PlaceTile(i, j + vineY, vineType);
							}
						}
					}
				}
			}
		}

		// Notes:	"positions" contains every point in the overall shape, including placed tiles, which should be all you need
		//			to place the remaining content (pots, platforms, decor).
		//			Otherwise, just call RottenGourd.Place at a location to spawn it. Easy enough!
		//			If you want to change the size, baseWidth controls the relative size of everything - minimum width is 5.
	}
}
