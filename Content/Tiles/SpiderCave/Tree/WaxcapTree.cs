﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    public class WaxcapTree : ModTile
    {
        //reminder:
        //X frame 0 = normal segment
        //X frame 18 = bottom segment (this is where the mushroom bottoms are)
        //X frame 36 = left branch segment (tiny branch)
        //X frame 54 = right branch segment (tiny branch)
        //X frame 72 = left and right branch segment (tiny branch)
        //X frame 90 = left branch segment (giant branch)
        //X frame 108 = right branch segment (giant branch)
        //X frame 126 = left and right branch segment (giant branch)
        //X frame 144 = mushroom top cap segment
        //X frame 162 = stubby top segment

        private Asset<Texture2D> TileTexture;
        private Asset<Texture2D> TileGlowTexture;
        private Asset<Texture2D> RootTexture;
        private Asset<Texture2D> RootGlowTexture;
        private Asset<Texture2D> TopTexture;
		private Asset<Texture2D> BranchSmallLeftTexture;
        private Asset<Texture2D> BranchSmallRightTexture;
        private Asset<Texture2D> BranchLargeLeftTexture;
        private Asset<Texture2D> BranchLargeRightTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(186, 163, 50), name);
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
                if (SolidTile(i, j - k) || SolidTile(i, j - k - 1) || SolidTile(i, j - k - 2) || SolidTile(i, j - k - 3) || SolidTile(i, j - k - 4))
                {
                    return false;
                }
            }

            //if the trees height is too short, dont let it grow
            if (height < minSize) 
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<WaxcapTree>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

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

            for (int numSegments = 0; numSegments < height; numSegments++)
            {
                //place tree segments
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<WaxcapTree>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //place root segment at the bottom
                if (numSegments == 0)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                }

                //chance to place random branch segment, but dont allow branch segments to place directly on top of each other
                if (Main.rand.NextBool() && numSegments > 1 && numSegments < height - 1 && 
                (Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX < 36 || Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX > 126))
                {
                    int[] BranchFrames = new int[] { 36, 54, 72, 90, 108, 126 };

                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = (short)Main.rand.Next(BranchFrames);
                }

                //place the tree top at the top of the tree
                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 144;
                }

                //chance to place extra vanity tree segment variant
                if (Main.rand.NextBool() && Framing.GetTileSafely(i, j - numSegments).TileFrameX == 0)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3, 5) * 18);
                }

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 1, 1, TileChangeType.None);
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
            if (belowFrame < 162)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 162;
                if (Framing.GetTileSafely(i, j + 1).TileFrameY > 36)
                {
                    Framing.GetTileSafely(i, j + 1).TileFrameY = 36;
                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
            TileTexture ??= ModContent.Request<Texture2D>(Texture);
            TileGlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
            TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops");
			RootTexture ??= ModContent.Request<Texture2D>(Texture + "Bottom");
            RootGlowTexture ??= ModContent.Request<Texture2D>(Texture + "BottomGlow");
			BranchSmallLeftTexture ??= ModContent.Request<Texture2D>(Texture + "BranchSmallLeft");
            BranchSmallRightTexture ??= ModContent.Request<Texture2D>(Texture + "BranchSmallRight");
            BranchLargeLeftTexture ??= ModContent.Request<Texture2D>(Texture + "BranchLeft");
            BranchLargeRightTexture ??= ModContent.Request<Texture2D>(Texture + "BranchRight");

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
            Color glowCol = TileGlobal.GetTileColorWithPaint(i, j, Color.White * 0.25f);
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

			int TopsTexRealWidth = TopTexture.Width() / 3;
            int RootsTexRealWidth = RootTexture.Width() / 2;
            int BranchSmallLeftTexRealWidth = BranchSmallLeftTexture.Width() / 2;
            int BranchSmallRightTexRealWidth = BranchSmallRightTexture.Width() / 2;
            int BranchLargeLeftTexRealWidth = BranchLargeLeftTexture.Width() / 2;
            int BranchLargeRightTexRealWidth = BranchLargeRightTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            int TileFrameWidth = Framing.GetTileSafely(i, j).TileFrameY > 36 ? 20 : 16;

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, TileFrameWidth, 16), 
                new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
            }

			//draw the actual tree
			spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, TileFrameWidth, 16), 
            new Color(col.R, col.G, col.B, 255), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TileGlowTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, TileFrameWidth, 16), 
            glowCol, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//left small branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 36 || Framing.GetTileSafely(i, j).TileFrameX == 72)
			{
                spriteBatch.Draw(BranchSmallLeftTexture.Value, pos + new Vector2(BranchSmallLeftTexRealWidth / 2 - 9, BranchSmallLeftTexture.Height() / 3), new Rectangle(0, 18 * frame, 24, 18), col, 0f, 
                new Vector2(BranchSmallLeftTexRealWidth, BranchSmallLeftTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(BranchSmallLeftTexture.Value, pos + new Vector2(BranchSmallLeftTexRealWidth / 2 - 9, BranchSmallLeftTexture.Height() / 3), new Rectangle(0, 18 * frame, 24, 18), glowCol, 0f, 
                new Vector2(BranchSmallLeftTexRealWidth, BranchSmallLeftTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
            }
			//right small branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 54 || Framing.GetTileSafely(i, j).TileFrameX == 72)
			{
                spriteBatch.Draw(BranchSmallRightTexture.Value, pos + new Vector2(BranchSmallRightTexRealWidth / 2 + 15, BranchSmallRightTexture.Height() / 3), new Rectangle(0, 18 * frame, 20, 18), col, 0f, 
                new Vector2(BranchSmallRightTexRealWidth, BranchSmallRightTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(BranchSmallRightTexture.Value, pos + new Vector2(BranchSmallRightTexRealWidth / 2 + 15, BranchSmallRightTexture.Height() / 3), new Rectangle(0, 18 * frame, 20, 18), glowCol, 0f, 
                new Vector2(BranchSmallRightTexRealWidth, BranchSmallRightTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
			}

            //left big branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 90 || Framing.GetTileSafely(i, j).TileFrameX == 126)
			{
                spriteBatch.Draw(BranchLargeLeftTexture.Value, pos + new Vector2(BranchLargeLeftTexRealWidth / 2 - 19, BranchLargeLeftTexture.Height() / 3 - 10), new Rectangle(0, 38 * frame, 28, 38), col, 0f, 
                new Vector2(BranchLargeLeftTexRealWidth, BranchLargeLeftTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(BranchLargeLeftTexture.Value, pos + new Vector2(BranchLargeLeftTexRealWidth / 2 - 19, BranchLargeLeftTexture.Height() / 3 - 10), new Rectangle(0, 38 * frame, 28, 38), glowCol, 0f, 
                new Vector2(BranchLargeLeftTexRealWidth, BranchLargeLeftTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
			}
            //right big branches
            if (Framing.GetTileSafely(i, j).TileFrameX == 108 || Framing.GetTileSafely(i, j).TileFrameX == 126)
			{
                spriteBatch.Draw(BranchLargeRightTexture.Value, pos + new Vector2(BranchLargeRightTexRealWidth / 2 + 23, BranchLargeRightTexture.Height() / 3 - 6), new Rectangle(0, 34 * frame, 28, 34), col, 0f, 
                new Vector2(BranchLargeRightTexRealWidth, BranchLargeRightTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(BranchLargeRightTexture.Value, pos + new Vector2(BranchLargeRightTexRealWidth / 2 + 23, BranchLargeRightTexture.Height() / 3 - 6), new Rectangle(0, 34 * frame, 28, 34), glowCol, 0f, 
                new Vector2(BranchLargeRightTexRealWidth, BranchLargeRightTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
			}

			//draw roots at the bottom of the tree
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
			{
				spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 2 - 2), new Rectangle(0, 22 * frame, 44, 22), col, 0f, 
                new Vector2(RootsTexRealWidth, RootTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(RootGlowTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 2 - 2), new Rectangle(0, 22 * frame, 44, 22), glowCol, 0f, 
                new Vector2(RootsTexRealWidth, RootTexture.Height() / 2), 1f, SpriteEffects.None, 0f);

                //left root
                Tile tileLeft = Framing.GetTileSafely(i - 1, j + 1);
                if (SolidTile(i - 1, j + 1) && !tileLeft.LeftSlope && !tileLeft.RightSlope && !tileLeft.IsHalfBlock)
                {
                    spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 9 - 2), new Rectangle(0, 22 * (frame + 3), 44, 22), col, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 9), 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(RootGlowTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 9 - 2), new Rectangle(0, 22 * (frame + 3), 44, 22), glowCol, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 9), 1f, SpriteEffects.None, 0f);
                }
                //right root
                Tile tileRight = Framing.GetTileSafely(i + 1, j + 1);
                if (SolidTile(i + 1, j + 1) && !tileRight.LeftSlope && !tileRight.RightSlope && !tileRight.IsHalfBlock)
                {
                    spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 9 - 2), new Rectangle(0, 22 * (frame + 6), 44, 22), col, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 9), 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(RootGlowTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 1, RootTexture.Height() / 9 - 2), new Rectangle(0, 22 * (frame + 6), 44, 22), glowCol, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 9), 1f, SpriteEffects.None, 0f);
                }
			}

			//draw the tree tops
			if (Framing.GetTileSafely(i, j).TileFrameX == 144)
			{
                //actual mushroom cap with unique scaling for squish effect
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 15;
                Vector2 scale = new Vector2(-MathF.Sin(-i / 8f + sin), 1f);

                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 32, 4), new Rectangle(82 * frame, 0, 82, 60), col, 0f, 
                new Vector2(TopsTexRealWidth / 2, TopTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 32, 4), new Rectangle(82 * frame, 0, 82, 60), glowCol, 0f, 
                new Vector2(TopsTexRealWidth / 2, TopTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
			}
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}