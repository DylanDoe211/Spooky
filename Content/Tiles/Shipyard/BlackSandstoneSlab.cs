using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.Shipyard.Ambient;

namespace Spooky.Content.Tiles.Shipyard
{
	public class BlackSandstoneSlab : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(43, 45, 52));
            DustType = DustID.Asphalt;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 4 * 234; //288 is the width of each individual sheet
			frameYOffset = j % 4 * 90; //270 is the height of each individual sheet
        }
	}
}
