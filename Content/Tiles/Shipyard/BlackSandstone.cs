using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.Shipyard.Ambient;

namespace Spooky.Content.Tiles.Shipyard
{
	public class BlackSandstone : ModTile
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
            AddMapEntry(new Color(43, 45, 52));
            DustType = DustID.Asphalt;
			HitSound = SoundID.Tink;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Above.HasTile && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.NextBool() && Above.LiquidAmount <= 0)
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<PaleSeaOats>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(14) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
				}

				//grow bleached corals
                int InWaterChance1 = Above.LiquidAmount <= 0 ? 25 : 15;
                if (Main.rand.NextBool(InWaterChance1))
                {
                    WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BleachedCoral>(), true, Main.rand.Next(0, 8));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<BleachedCoral>(), 0, 0, -1, -1);
				}
			}
		}
	}
}
