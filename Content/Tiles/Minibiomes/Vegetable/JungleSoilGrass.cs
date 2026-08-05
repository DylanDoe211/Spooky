using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Tree;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable
{
	public class JungleSoilGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<JungleSoil>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(82, 165, 76));
            RegisterItemDrop(ModContent.ItemType<JungleSoilItem>());
            DustType = DustID.Grass;
            MineResist = 0.1f;
		}

		public override bool CanExplode(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
			return true;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<JungleSoil>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<JungleSoil>();
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);
			Tile Above2 = Framing.GetTileSafely(i - 1, j - 1);
			Tile Above3 = Framing.GetTileSafely(i + 1, j - 1);

			if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.NextBool(5)) 
                {
					WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<JungleVines>(), true);
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
				}

				//radish
				if (Main.rand.NextBool(55))
                {
					TileGlobal.PlaceObject(i, j + 1, (ushort)ModContent.TileType<RadishHanging>(), true, WorldGen.genRand.Next(0, 2));
                }

				//eggplant
				if (Main.rand.NextBool(55))
                {
					TileGlobal.PlaceObject(i, j + 1, (ushort)ModContent.TileType<Eggplant>(), true, WorldGen.genRand.Next(0, 2));
                }
            }

			if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock)
			{
				//grow weeds
				if (Main.rand.NextBool(3))
                {
					TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<JungleMossWeeds>(), true, Main.rand.Next(0, 11));
				}

				//grow broccoli trees
				if (Main.rand.NextBool(50) && VegetableGarden.CanPlaceBroccoli(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
				{
					Broccoli.Grow(i, j - 1, 5, 9);
				}

				//cabbage boulders
				if (Main.rand.NextBool(30) && VegetableGarden.CanPlaceCabbageBoulder(i, j))
				{
					TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<JungleCabbageBoulder>(), true);
				}

				//misc plants
				if (Main.rand.NextBool(15))
				{
					ushort[] LeafyPlants = new ushort[] { (ushort)ModContent.TileType<JunglePlant1>(), (ushort)ModContent.TileType<JunglePlant2>(), (ushort)ModContent.TileType<JunglePlant3>(),
					(ushort)ModContent.TileType<JunglePlant4>(), (ushort)ModContent.TileType<JunglePlant5>(), (ushort)ModContent.TileType<JunglePlant6>() };
					TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(LeafyPlants), true);
				}

				//carrots
				if (Main.rand.NextBool(20))
				{
					ushort[] Carrots = new ushort[] { (ushort)ModContent.TileType<Carrot1>(), (ushort)ModContent.TileType<Carrot2>(), (ushort)ModContent.TileType<Carrot3>() };
					TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Carrots), true, Main.rand.Next(0, 2));
				}

				//corns
				if (Main.rand.NextBool(20))
				{
					ushort[] Corns = new ushort[] { (ushort)ModContent.TileType<Corn1>(), (ushort)ModContent.TileType<Corn2>() };
					TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Corns), true);
				}

				//garlic
				if (Main.rand.NextBool(20))
				{
					TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<Garlic>(), true);
				}

				//potatos
				if (Main.rand.NextBool(20))
				{
					ushort[] Potatos = new ushort[] { (ushort)ModContent.TileType<Potato1>(), (ushort)ModContent.TileType<Potato2>(), (ushort)ModContent.TileType<Potato3>(), (ushort)ModContent.TileType<Potato4>() };
					TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Potatos), true);
				}

				//radish
				if (Main.rand.NextBool(35))
                {
                    ushort[] Radishes = new ushort[] { (ushort)ModContent.TileType<Radish1>(), (ushort)ModContent.TileType<Radish2>() };
					TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Radishes), true);
                }
			}

			//spread grass
			List<Point> adjacents = TileGlobal.OpenAdjacents(i, j, ModContent.TileType<JungleSoil>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (TileGlobal.HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<JungleSoilGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
        }
	}
}
