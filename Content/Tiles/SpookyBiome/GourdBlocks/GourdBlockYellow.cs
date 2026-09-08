using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
	public class GourdBlockYellow : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(215, 176, 73));
			DustType = 288;
            HitSound = SoundID.Dig;
		}

		int Seed = 0;
		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
		{
			var tile = Main.tile[i, j];

			if (Seed == 0)
			{
				Seed = WorldGen.genRand.Next();
			}

			if (tile.TileFrameX is 18 or 36 or 54 && tile.TileFrameY is 18) //Plain center frames
			{
				float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 150f, j / 150f, 5, unchecked(Seed + 1)) * 0.01f;
				float Value1 = SpookyWorldMethods.PerlinNoise2D(i / 150f, j / 150f, 5, Seed) + 0.5f;
				float Value2 = SpookyWorldMethods.PerlinNoise2D(i / 150f, j / 150f, 5, unchecked(Seed - 1)) + 0.5f;
				float SpecialVariantMap = (Value1 + Value2) * 0.35f;
				float SpecialVariantThreshold = horizontalOffsetNoise * 2f + 0.1f;

				//kill or place tiles depending on the noise map
				if (SpecialVariantMap * SpecialVariantMap <= SpecialVariantThreshold)
				{
					Point16 result = new(18 * 7, 18 * 12);
					int random = Main.rand.Next(3);

					tile.TileFrameX = (short)(result.X + 18 * random);
					tile.TileFrameY = result.Y;
				}
			}
		}
	}
}
