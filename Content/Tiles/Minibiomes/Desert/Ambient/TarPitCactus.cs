using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Desert.Ambient
{
    public class TarPitCactus : ModTile
    {
        //reminder:
        //X frame 0 = segment
        //X frame 16 = left branch segment
        //X frame 36 = right branch segment
        //X frame 54 = top segment

        private static Asset<Texture2D> BranchLeftTexture;
		private static Asset<Texture2D> BranchRightTexture;
        private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> TileTexture;

		public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(59, 100, 52), name);
            RegisterItemDrop(ModContent.ItemType<TarPitCactusBlockItem>());
            DustType = DustID.Grass;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX >= 54)
            {
                float divide = 450f;

                r = 255f / divide;
                g = 134f / divide;
                b = 0f / divide;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            resetFrame = false;
            noBreak = true;
            return false;
        }

        public static bool SolidTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        }

        public static bool SolidTopTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Grow(int i, int j, int minSize, int maxSize, bool saplingExists = false)
        {
            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 2, 1, TileChangeType.None);
				}
			}

            int height = WorldGen.genRand.Next(minSize, maxSize);
            for (int k = 1; k < height; k++)
            {
                if (SolidTile(i, j - k - 1))
                {
                    height = k - 2;
                    break;
                }
            }

            if (height < minSize) 
            {
                return false;
            }

            for (int numSegments = 0; numSegments < height; numSegments++)
            {
				if (Main.tile[i - 1, j - numSegments].HasTile || Main.tile[i, j - numSegments].HasTile || Main.tile[i + 1, j - numSegments].HasTile || Main.tile[i, j - numSegments].LiquidAmount > 0)
				{
					return false;
				}
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<TarPitCactus>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 1, 1, TileChangeType.None);
				}
			}
            //otherwise dont allow the tree to grow
            else
            {
                return false;
            }

            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<TarPitCactus>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

				if (WorldGen.genRand.NextBool())
				{
					if (WorldGen.genRand.NextBool())
					{
						Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
					}
					else
					{
						Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
					}
				}
				else
				{
					Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
				}

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 54;
                }

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j - numSegments, 1, 1, TileChangeType.None);
				}
			}

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            //kill the tree if there are no tiles below it
            if (!Framing.GetTileSafely(i, j + 1).HasTile)
            {
                (int x, int y) = (i, j);
                KillEntireTree(ref x, ref y);
            }
        }

        private void KillEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
			{
                WorldGen.KillTile(x, y, false, false, false);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                }
                y--;
			}

            y++;
        }

        private void CheckEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
			{
                y--;
			}

            y++;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //X frame 0 = normal tree segment
            //X frame 16 = tree top draw segment
            //X frame 36 = stubby top segment

            Tile tile = Framing.GetTileSafely(i, j);

            if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
                CheckEntireTree(ref x, ref y);
            }

            if (fail)
            {
                return;
            }

            int belowFrame = Framing.GetTileSafely(i, j + 1).TileFrameX;

            //if theres any remaining segments below, turn it into a stub top segment
            if (belowFrame < 54)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 54;
            }
        }

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			BranchLeftTexture ??= ModContent.Request<Texture2D>(Texture + "BranchLeft");
			BranchRightTexture ??= ModContent.Request<Texture2D>(Texture + "BranchRight");
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
			TileTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            int BranchLeftTexRealWidth = BranchLeftTexture.Width() / 2;
            int BranchRightTexRealWidth = BranchRightTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
            }

			//draw the actual tree
			spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(GlowTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //draw branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
			{
                spriteBatch.Draw(BranchLeftTexture.Value, pos + new Vector2(BranchLeftTexRealWidth / 2 - 7, 14), new Rectangle(0, 18 * frame, 16, 16), col, 0f, 
				new Vector2(BranchLeftTexRealWidth, BranchLeftTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
			}
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
				spriteBatch.Draw(BranchRightTexture.Value, pos + new Vector2(BranchRightTexRealWidth / 2 + 17, 14), new Rectangle(0, 18 * frame, 16, 16), col, 0f, 
				new Vector2(BranchRightTexRealWidth, BranchRightTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
			}
		}

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}