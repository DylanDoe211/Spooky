using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Shipyard.Ambient;

namespace Spooky.Content.Tiles.Shipyard
{
	public class BlackSandstoneMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
            AddMapEntry(new Color(105, 144, 190));
            DustType = ModContent.DustType<ShipyardMossGrassDust>();
			HitSound = SoundID.Tink;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 2000f;

			r = 66f / divide;
			g = 138f / divide;
			b = 255f / divide;
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
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<BlackSandstone>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<BlackSandstone>();
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (Main.rand.NextBool(2000) && !WorldGen.SolidTile(i, j - 1))
			{
				Dust.NewDust(new Vector2(i * 16, (j * 16) - 6), 1, 1, ModContent.DustType<ShipyardMossDust>());
			}
		}

		public override void RandomUpdate(int i, int j)
        {
           	Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Above.HasTile && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
				//grow small weeds
                if (Main.rand.NextBool(10))
                {
					TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<BlackSandstoneMossWeeds>(), true, Main.rand.Next(0, 6));
				}

				//grow bleached corals
                int InWaterChance1 = Above.LiquidAmount <= 0 ? 20 : 12;
                if (Main.rand.NextBool(InWaterChance1))
                {
                    TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<BleachedCoral>(), true, Main.rand.Next(0, 8));
				}

				//ghost flowers 
                int InWaterChance2 = Above.LiquidAmount <= 0 ? 25 : 15;
                if (Main.rand.NextBool(InWaterChance2))
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<GhostFlower>(), true);
                }

                //giant bleached coral 
                if (Main.rand.NextBool(InWaterChance2))
                {
                    ushort[] GiantCorals = new ushort[] { (ushort)ModContent.TileType<BleachedCoralGiant1>(), (ushort)ModContent.TileType<BleachedCoralGiant2>(), (ushort)ModContent.TileType<BleachedCoralGiant3>(),
                    (ushort)ModContent.TileType<BleachedCoralGiant4>(), (ushort)ModContent.TileType<BleachedCoralGiant5>(), (ushort)ModContent.TileType<BleachedCoralGiant6>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(GiantCorals), true);
                }
			}

			//spread grass
            List<Point> adjacents = TileGlobal.OpenAdjacents(i, j, ModContent.TileType<BlackSandstone>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (TileGlobal.HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<BlackSandstoneMoss>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
		}
	}
}
