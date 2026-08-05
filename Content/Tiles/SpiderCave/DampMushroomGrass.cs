using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Tree;

namespace Spooky.Content.Tiles.SpiderCave
{
	public class DampMushroomGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<DampSoil>();
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(204, 223, 216));
            RegisterItemDrop(ModContent.ItemType<DampSoilItem>());
            DustType = DustID.Smoke;
			MineResist = 0.1f;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 700f;

			r = 204f / divide;
			g = 223f / divide;
			b = 216f / divide;
        }

        public override bool HasWalkDust() 
        {
			return true;
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
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<DampSoil>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<DampSoil>();
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
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<DampMushroomVines>(), true);
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
				}
            }

			if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.NextBool(4))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<DampMushroomWeeds>(), true, Main.rand.Next(0, 21));
				}

                //mushrooms 
                if (Main.rand.NextBool(25))
                {
                    ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBlue>(), (ushort)ModContent.TileType<MushroomRedBrown>(),
                    (ushort)ModContent.TileType<MushroomYellow>(), (ushort)ModContent.TileType<MushroomGreen>(), (ushort)ModContent.TileType<MushroomPurple>(),
                    (ushort)ModContent.TileType<MushroomRed>(), (ushort)ModContent.TileType<MushroomTeal>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(Mushrooms), true, WorldGen.genRand.Next(0, 2));
                }

                //friend mushroom
                if (Main.rand.NextBool(35) && Flags.SporeEventHappening)
                {
                    TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<MushroomFriendTile>(), true, WorldGen.genRand.Next(0, 2));
                }
                //mushroom armor
                if (Main.rand.NextBool(20) && Flags.SporeEventHappening)
                {
                    ushort[] ArmorMushrooms = new ushort[] { (ushort)ModContent.TileType<SporeShroomBodyTile>(), (ushort)ModContent.TileType<SporeShroomHeadTile>(), (ushort)ModContent.TileType<SporeShroomLegsTile>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(ArmorMushrooms), true, WorldGen.genRand.Next(0, 2));
                }
			}

            //spread grass
            List<Point> adjacents = TileGlobal.OpenAdjacents(i, j, ModContent.TileType<DampSoil>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (TileGlobal.HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<DampMushroomGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
		}
	}
}
