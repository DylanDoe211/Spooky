using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
	public class WaxcapTreeSapling : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.CommonSapling[Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.addTile(Type);
			AdjTiles = new int[]{ TileID.Saplings };
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(204, 223, 216), name);
            DustType = -1;
			HitSound = SoundID.Dig;
		}

		public override bool CanDrop(int i, int j)
		{
			return false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
		{
			if (i % 2 == 1)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
		}

		public override void RandomUpdate(int i, int j) 
		{
            if (Main.tile[i, j + 1].TileType != Type)
            {
				if (WorldGen.genRand.NextBool(12))
				{
					WaxcapTree.Grow(i, j - 1, 3, 15, true);
				}
            }
		}
	}
}