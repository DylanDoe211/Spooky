using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.Shipyard.Ambient;

namespace Spooky.Content.Tiles.Shipyard
{
	public class BlackSand : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(63, 63, 63));
            DustType = DustID.Ash;
			MineResist = 0.5f;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Above.HasTile && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock)
            {
                //grow small weeds
                if (Main.rand.NextBool(10) && Above.LiquidAmount <= 0)
                {
                    TileGlobal.PlaceObject(i, j - 1, (ushort)ModContent.TileType<PaleSeaOats>(), true, Main.rand.Next(0, 14));
				}

				//grow bleached corals
                if (Main.rand.NextBool(10) && Above.LiquidAmount > 0 && Above.LiquidType == LiquidID.Water)
                {
                    TileGlobal.PlaceObject(i, j - 1, ModContent.TileType<BleachedCoral>(), true, Main.rand.Next(0, 8));
				}

                //giant bleached coral 
                if (Main.rand.NextBool(15) && Above.LiquidAmount > 0 && Above.LiquidType == LiquidID.Water)
                {
                    ushort[] GiantCorals = new ushort[] { (ushort)ModContent.TileType<BleachedCoralGiant1>(), (ushort)ModContent.TileType<BleachedCoralGiant2>(), (ushort)ModContent.TileType<BleachedCoralGiant3>(),
                    (ushort)ModContent.TileType<BleachedCoralGiant4>(), (ushort)ModContent.TileType<BleachedCoralGiant5>(), (ushort)ModContent.TileType<BleachedCoralGiant6>() };
                    TileGlobal.PlaceObject(i, j - 1, Main.rand.Next(GiantCorals), true);
                }
			}
		}
	}
}
