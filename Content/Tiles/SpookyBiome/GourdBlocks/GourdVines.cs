using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
	public class GourdVinesGreen : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(0, 151, 64));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0)
			{
				Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);
			}

			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			int[] ValidTiles = { Type, ModContent.TileType<GourdBlockGreen>(), ModContent.TileType<GourdBlockLime>(), ModContent.TileType<GourdBlockLimeOrange>(),
			ModContent.TileType<GourdBlockOrange>(), ModContent.TileType<GourdBlockRed>(), ModContent.TileType<GourdBlockWhite>(), ModContent.TileType<GourdBlockYellow>(), ModContent.TileType<GourdBlockYellowGreen>() };

			if (!ValidTiles.Contains(Main.tile[i, j - 1].TileType))
			{
				WorldGen.KillTile(i, j, false, false, false);
			}
			
            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

		public override void RandomUpdate(int i, int j)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (Main.rand.NextBool(12) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
				int[] ValidTiles = { ModContent.TileType<GourdBlockGreen>(), ModContent.TileType<GourdBlockLime>(), ModContent.TileType<GourdBlockLimeOrange>(), ModContent.TileType<GourdBlockOrange>(), 
				ModContent.TileType<GourdBlockRed>(), ModContent.TileType<GourdBlockWhite>(), ModContent.TileType<GourdBlockYellow>(), ModContent.TileType<GourdBlockYellowGreen>() };

				bool PlaceVine = false;
				int Test = j;
				while (Test > j - 10) 
                {
					Tile testTile = Framing.GetTileSafely(i, Test);
					if (testTile.BottomSlope) 
                    {
						break;
					}
					else if (!testTile.HasTile || ValidTiles.Contains(testTile.TileType)) 
                    {
						Test--;
						continue;
					}
					PlaceVine = true;
					break;
				}
				
				if (PlaceVine) 
                {
					tileBelow.TileType = Type;
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server) 
                    {
						NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
					}
				}
			}
		}
	}

	public class GourdVinesLime : GourdVinesGreen
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(102, 172, 47));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}
	}

	public class GourdVinesOrange : GourdVinesGreen
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(192, 99, 40));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}
	}

	public class GourdVinesRed : GourdVinesGreen
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(171, 58, 55));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}
	}

	public class GourdVinesWhite : GourdVinesGreen
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(135, 143, 145));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}
	}

	public class GourdVinesYellow : GourdVinesGreen
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(170, 118, 47));
			DustType = 288;
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}
	}
}