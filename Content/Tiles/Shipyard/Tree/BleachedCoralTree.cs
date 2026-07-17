using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Shipyard.Tree
{
    public class BleachedCoralTree : ModTile
    {
        //reminder:
        //X frame 0 = normal segment
        //X frame 18 = branch segment
        //X frame 36 = top segment
        //X frame 54 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> BranchTexture;
        private static Asset<Texture2D> StemTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(170, 162, 174), name);
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
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

        public static bool Grow(int i, int j, int minSize, int maxSize, int color, bool saplingExists = false)
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
                if (SolidTile(i, j - k - 6))
                {
                    height = k - 2;
                    break;
                }
            }

            if (height < minSize) 
            {
                return false;
            }

			//preform a loop where the tree will grow and check to make sure no tiles are above it
			//if there are tiles blocking the way, dont allow the tree to grow
			for (int numSegments = 0; numSegments < height; numSegments++)
			{
				Tile above = Main.tile[i, j - numSegments - 1];

				if (above.HasTile)
				{
					return false;
				}
			}

			for (int numSegments = 0; numSegments < height; numSegments++)
			{
				WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<BleachedCoralTree>(), true);
				Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
				Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

				if (WorldGen.genRand.NextBool(4))
				{
					Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
				}

				if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
                }

                //this randomizes the Y-frame, so that the coral trees color is always random
                Framing.GetTileSafely(i, j - numSegments).TileFrameY += (short)(54 * color);

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
                /*
                int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<BleachedCoralBlockItem>());

                if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
                }
                */

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

            if (tile.TileFrameX == 36)
            {
                //spawn a seed from the tree
                if (Main.rand.NextBool())
                {
                    /*
                    int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-22, 22), Main.rand.Next(-22, 22)), 
                    ModContent.ItemType<BroccoliSaplingItem>(), Main.rand.Next(1, 3));

                    if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
					}
                    */
                }
            }
        }

        public static void DrawTreePiece(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
			Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

			Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //WITH PADDING
            //tree top width = 130 
            //tree top height = 88

			TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Shipyard/Tree/BleachedCoralTreeTops");
			BranchTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Shipyard/Tree/BleachedCoralTreeBranches");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            //all of this frame and drawing code sucks but i did not feel like resheeting the entire tree tops, my apologies to anyone seeing this
            bool GreenFrames = Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 2;
            bool OrangeFrames = Framing.GetTileSafely(i, j).TileFrameY == 18 * 3 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 4 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 5;
            bool PurpleFrames = Framing.GetTileSafely(i, j).TileFrameY == 18 * 6 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 7 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 8;
            bool RedFrames = Framing.GetTileSafely(i, j).TileFrameY == 18 * 9 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 10 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 11;
            bool YellowFrames = Framing.GetTileSafely(i, j).TileFrameY == 18 * 12 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 13 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 14;
            bool BlueFrames = Framing.GetTileSafely(i, j).TileFrameY == 18 * 15 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 16 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 17;

			//draw tree branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
				//reminder: offset negative numbers are right and down, while positive is left and up
                Vector2 LeftOffset = new Vector2((BranchTexture.Width() / 2) + 2, -(BranchTexture.Height() / 12) + 12);
                Vector2 RightOffset = new Vector2(-(BranchTexture.Width() / 2) - 12, -(BranchTexture.Height() / 12) + 12);

                if (GreenFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 2)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 0, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 2)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
                if (OrangeFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 3 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 2, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 3, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
                if (PurpleFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 6 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 8)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 4, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 7 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 8)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 5, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
                if (RedFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 9 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 11)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 6, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 10 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 11)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 7, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
                if (YellowFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 12 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 14)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 8, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 13 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 14)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 9, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
                if (BlueFrames)
                {
                    //left branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 15 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 17)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 10, 24, 18), TileGlobal.TileOffset, LeftOffset);
                    }
                    //right branch
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 16 || Framing.GetTileSafely(i, j).TileFrameY == 18 * 17)
                    {
                        DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18 * 11, 24, 18), TileGlobal.TileOffset, RightOffset);
                    }
                }
            }

            //draw tree tops
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
            {
                Vector2 offset = new Vector2(((TopTexture.Width() / 6) / 2) - 17, (TopTexture.Height() / 3) - 16);

                //three first, top row
                if (GreenFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 0, 0, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18)
                    {
                        FrameToUse = new Rectangle(130 * 1, 0, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 2)
                    {
                        FrameToUse = new Rectangle(130 * 2, 0, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
                //three last, top row
                if (OrangeFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 3, 0, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4)
                    {
                        FrameToUse = new Rectangle(130 * 4, 0, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        FrameToUse = new Rectangle(130 * 5, 0, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
                //three first, middle row
                if (PurpleFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 0, 88, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4)
                    {
                        FrameToUse = new Rectangle(130 * 1, 88, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        FrameToUse = new Rectangle(130 * 2, 88, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
                //three last, middle row
                if (RedFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 3, 88, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4)
                    {
                        FrameToUse = new Rectangle(130 * 4, 88, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        FrameToUse = new Rectangle(130 * 5, 88, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
                //three first, bottom row
                if (YellowFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 0, 88 * 2, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4)
                    {
                        FrameToUse = new Rectangle(130 * 1, 88 * 2, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        FrameToUse = new Rectangle(130 * 2, 88 * 2, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
                //three last, bottom row
                if (BlueFrames)
                {
                    Rectangle FrameToUse = new Rectangle(130 * 3, 88 * 2, 130, 88);
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 4)
                    {
                        FrameToUse = new Rectangle(130 * 4, 88 * 2, 130, 88);
                    }
                    if (Framing.GetTileSafely(i, j).TileFrameY == 18 * 5)
                    {
                        FrameToUse = new Rectangle(130 * 5, 88 * 2, 130, 88);
                    }

                    DrawTreePiece(i - 1, j - 1, TopTexture.Value, FrameToUse, TileGlobal.TileOffset, offset);
                }
            }

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
            }

            //draw the actual tree
            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}