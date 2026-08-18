using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Tree;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable
{
	public class JungleMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(82, 165, 76));
            DustType = DustID.Grass;
			HitSound = SoundID.Grass;
			MineResist = 0.65f;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (Main.rand.NextBool(3) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
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
				if (Main.rand.NextBool(10))
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
		}
	}
}
