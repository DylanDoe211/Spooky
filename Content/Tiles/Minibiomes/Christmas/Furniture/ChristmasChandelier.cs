using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasChandelier : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
			TileID.Sets.MultiTileSway[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = -10;
			TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
			AddMapEntry(new Color(72, 88, 88), Language.GetText("MapObject.Chandelier"));
            DustType = DustID.Stone;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Chandeliers };
        }

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY += 2;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<ChristmasChandelierItem>());
        }

        public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int width = 3;
			int height = 3;
			int x = i - tile.TileFrameX / 18 % width;
			int y = j - tile.TileFrameY / 18 % height;

			for (int l = x; l < x + width; l++)
			{
				for (int m = y; m < y + height; m++)
				{
					Tile checkTile = Framing.GetTileSafely(l, m);
					if (checkTile.HasTile && checkTile.TileType == Type)
					{
						if (checkTile.TileFrameX != 108)
						{
							checkTile.TileFrameX += 54;
						}
						if (checkTile.TileFrameX >= 108)
						{
							checkTile.TileFrameX -= 108;
						}
					}

					if (Wiring.running)
					{
						Wiring.SkipWire(l, m);
					}
				}
			}

			int w2 = width / 2;
			int h2 = height / 2;
			NetMessage.SendTileSquare(-1, x + w2 - 1, y + h2, 1 + w2 + h2);
		}

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 250f;

                r = 112f / divide;
                g = 147f / divide;
                b = 158f / divide;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			if (TileObjectData.IsTopLeft(tile))
			{
				Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);
			}

			return false;
		}

		public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor)
		{
			overrideWindCycle = 1f;
			windPushPowerY = 0;
		}

		public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
		{
			if (!Main.dedServ)
			{
				GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
			}

			tileFlameData.flameCount = 5;
			tileFlameData.flameColor = Color.White * 0.5f;
			tileFlameData.flameRangeXMin = -10;
			tileFlameData.flameRangeXMax = 11;
			tileFlameData.flameRangeYMin = -10;
			tileFlameData.flameRangeYMax = 1;
			tileFlameData.flameRangeMultX = 0.15f;
			tileFlameData.flameRangeMultY = 0.35f;
			ulong flameSeed = Main.TileFrameSeed ^ (ulong)((long)i << 32 | (uint)j);
			tileFlameData.flameTexture = GlowTexture.Value;
			tileFlameData.flameSeed = flameSeed;
		}
    }
}