using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable
{
	public class JungleSoil : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(50, 54, 58));
            DustType = 109;
			MineResist = 0.5f;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
		{
			var tile = Main.tile[i, j];

			if (Main.rand.NextBool(10) && tile.TileFrameX is 18 or 36 or 54 && tile.TileFrameY is 18) //Plain center frames
			{
				Point16 result = new(18 * 7, 18 * 12);
				int random = Main.rand.Next(3);

				tile.TileFrameX = (short)(result.X + 18 * random);
				tile.TileFrameY = result.Y;
			}
		}
	}
}
