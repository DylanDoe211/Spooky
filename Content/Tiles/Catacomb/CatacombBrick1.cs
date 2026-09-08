using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
	[LegacyName("CatacombBrick")]
	public class CatacombBrick1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			RegisterItemDrop(ModContent.ItemType<CatacombBrick1Item>());
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
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

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return Flags.downedDaffodil && tileTypeBeingPlaced != ModContent.TileType<CatacombBrick1Safe>();
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			MinPick = Flags.downedDaffodil ? 0 : int.MaxValue;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}

	public class CatacombBrick1Arena : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
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

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return false;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return false;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}

	public class CatacombBrick1Safe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
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
