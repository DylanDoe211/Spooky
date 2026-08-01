using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Shipyard.Tree
{
    public class MangroveSapling : ModTile
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
			TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<BlackSand>() };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(101, 98, 94), name);
            DustType = DustID.Ash;
			AdjTiles = new int[] { TileID.Saplings };
		}

        public override bool CanDrop(int i, int j)
        {
			return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 0;
            height = 36;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

		public override void RandomUpdate(int i, int j) 
		{
            if (Main.tile[i, j + 1].TileType != ModContent.TileType<BlackSand>())
            {
				if (WorldGen.genRand.NextBool(20))
				{
					MangroveTree.Grow(i, j + 1, 5, 13, true, Main.tile[i, j].TileFrameX);
				}
            }
		}
	}
}