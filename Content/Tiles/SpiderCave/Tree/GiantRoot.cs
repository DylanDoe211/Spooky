using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    internal class GiantRoot : ModTile
    {
        //reminder:
        //X frame 0 = normal segment
        //X frame 18 = bottom segment
        //X frame 36 = root base
        //X frame 54 = root sides
        //X frame 72 = root stubby bottom

        private Asset<Texture2D> BottomTexture;
        private Asset<Texture2D> RootTexture;

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
            AddMapEntry(new Color(201, 175, 139), name);
            RegisterItemDrop(ModContent.ItemType<RootWoodItem>());
            DustType = DustID.Web;
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

        public static bool NoSlopedTile(int i, int j)
        {
            return !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock;
        }

        public static bool Grow(int i, int j, int minSize, int maxSize)
        {
            int height = WorldGen.genRand.Next(minSize, maxSize);

            //if the root grows too low, set its height to be higher above the ground
            for (int k = 1; k < height; k++)
            {
                if (SolidTile(i, j + k + 3))
                {
                    height = k - 1;
                    break;
                }
            }

            //dont allow the root to grow if its height ends up being too small
            if (height < minSize)
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if (SolidTile(i, j - 1) && NoSlopedTile(i, j - 1) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<GiantRoot>(), true);
                Framing.GetTileSafely(i, j).TileFrameX = 0;
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //both side roots
                if (SolidTile(i - 1, j - 1) && SolidTile(i + 1, j - 1) && NoSlopedTile(i + 1, j - 1) && NoSlopedTile(i - 1, j - 1) && !Framing.GetTileSafely(i - 1, j).HasTile && !Framing.GetTileSafely(i + 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 0;

                    //left
                    WorldGen.PlaceTile(i - 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i - 1, j).TileFrameY = 18;

                    //right
                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i + 1, j).TileFrameY = 0;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i - 1, j, 1, 1, TileChangeType.None);
                        NetMessage.SendTileSquare(-1, i + 1, j, 1, 1, TileChangeType.None);
                    }
                }
                //left root only
                else if (SolidTile(i - 1, j - 1) && NoSlopedTile(i - 1, j - 1) && !Framing.GetTileSafely(i - 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 18;

                    //left
                    WorldGen.PlaceTile(i - 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i - 1, j).TileFrameY = 18;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i - 1, j, 1, 1, TileChangeType.None);
                    }
                }
                //right root only
                else if (SolidTile(i + 1, j - 1) && NoSlopedTile(i + 1, j - 1) && !Framing.GetTileSafely(i + 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 36;

                    //right
                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i + 1, j).TileFrameY = 0;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i + 1, j, 1, 1, TileChangeType.None);
                    }
                }

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

            //grow the tree itself
            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                WorldGen.PlaceTile(i, j + numSegments, ModContent.TileType<GiantRoot>(), true);
                Framing.GetTileSafely(i, j + numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j + numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j + numSegments).TileFrameX = 18;
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
            //kill the tree if there are no tiles above it
            if (!Framing.GetTileSafely(i, j - 1).HasTile)
            {
                //spawn root from the trees when broken
                int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<RootWoodItem>());

                if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
                }

                WorldGen.KillTile(i, j, false, false, false);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                }
            }

            //kill the side roots of the root if they arent attached to the main root itself
            if (Framing.GetTileSafely(i + 1, j).TileType != ModContent.TileType<GiantRoot>() && Framing.GetTileSafely(i - 1, j).TileType != ModContent.TileType<GiantRoot>() && Framing.GetTileSafely(i, j).TileFrameX == 54)
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
                y++;
			}

            y--;
        }

        private void CheckEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
			{
                y++;
			}

            y--;
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

            int aboveFrame = Framing.GetTileSafely(i, j - 1).TileFrameX;

            //if theres any remaining segments below, turn it into a stub bottom segment
            if (aboveFrame < 72)
            {
                Framing.GetTileSafely(i, j - 1).TileFrameX = 72;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
            BottomTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/GiantRootBottom");
			RootTexture ??= ModContent.Request<Texture2D>(Texture);

            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;
            Vector2 WavyOffset = new Vector2((xOff * 2), -2);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            //divide texture width by 3 since there are 3 horizontal frames, then divide it by 2 to get half the width for the individual frame
			int BottomTexRealWidth = (BottomTexture.Width() / 3) / 2;

            int frame = tile.TileFrameY / 18;

            if (tile.TileFrameX == 18)
            {
                spriteBatch.Draw(BottomTexture.Value, pos + new Vector2(BottomTexRealWidth / 2 - 12, 14) + WavyOffset, new Rectangle(88 * frame, 0, 86, 82), col, 0f, 
                new Vector2(BottomTexRealWidth, 0), 1f, SpriteEffects.None, 0f);
            }

            //draw the actual tree
            spriteBatch.Draw(RootTexture.Value, pos + WavyOffset, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}