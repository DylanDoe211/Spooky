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
using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Tree
{
    public class GiantShroom : ModTile
    {
        //reminder:
        //X frame 0 = root segment
        //X frame 18 = normal tree segment
        //X frame 36 = top segment
        //X frame 54 = branches segment
        //X frame 72 = stubby top segment

        private Asset<Texture2D> TopTexture;
        private Asset<Texture2D> CapTexture;
		private Asset<Texture2D> BranchTexture;
        private Asset<Texture2D> SideFungusTexture;
        private Asset<Texture2D> RootTexture;
        private Asset<Texture2D> StemTexture;

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
            AddMapEntry(new Color(196, 188, 217), name);
            RegisterItemDrop(ModContent.ItemType<SpookyGlowshroom>());
            DustType = DustID.Slush;
			HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            //create light at the top of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
                float divide = 350f;

                r = 155f / divide;
                g = 83f / divide;
                b = 250f / divide;
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
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || 
            Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
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
                if (SolidTile(i, j - k))
                {
                    height = k - 2;
                    break;
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
                WorldGen.PlaceTile(i, j, ModContent.TileType<GiantShroom>(), true);
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

            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                //place tree segments
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<GiantShroom>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                //place root segment at the bottom
                if (numSegments == 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                }

                if (numSegments > 1 && numSegments < height - 1)
                {
                    if (Main.rand.NextBool(6))
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 54;
                    }
                    else
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                    }
                }

                //place the tree top at the top of the tree
                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
                }

                if (Framing.GetTileSafely(i, j - numSegments + 1).TileType != ModContent.TileType<MushroomMoss>() && Framing.GetTileSafely(i, j - numSegments).TileFrameX == 0)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
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
            if (belowFrame < 72)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 72;
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
            TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomTop");
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomTopCap");
			RootTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomRoots");
			SideFungusTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomSides");
			BranchTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomBranches");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

			int TopsTexRealWidth = TopTexture.Width() / 2;
            int BranchesTexRealWidth = BranchTexture.Width() / 2;
            int SideFungusTexRealWidth = SideFungusTexture.Width() / 2;
            int RootsTexRealWidth = RootTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

			//X frame 0 = root segment
			//X frame 18 = normal tree segment
			//X frame 36 = top segment
			//X frame 54 = branches segment
			//X frame 72 = stubby top segment

			//draw the actual tree

			spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
			new Color(col.R, col.G, col.B, 255), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//draw branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 54)
			{
				//left branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18)
				{
                    spriteBatch.Draw(BranchTexture.Value, pos + new Vector2(BranchesTexRealWidth / 2 - 25, BranchTexture.Height() / 4 - 14), new Rectangle(0, 32 * frame, 34, 30), col, 0f, 
                    new Vector2(BranchesTexRealWidth, BranchTexture.Height() / 4), 1f, SpriteEffects.None, 0f);
				}

				//right branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 36 || Framing.GetTileSafely(i, j).TileFrameY == 54)
				{
                    spriteBatch.Draw(BranchTexture.Value, pos + new Vector2(BranchesTexRealWidth / 2 + 23, BranchTexture.Height() / 4 - 14), new Rectangle(0, 32 * frame, 34, 30), col, 0f, 
                    new Vector2(BranchesTexRealWidth, BranchTexture.Height() / 4), 1f, SpriteEffects.None, 0f);
				}
			}

			//left side fungus
			if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 18)
			{
                spriteBatch.Draw(SideFungusTexture.Value, pos + new Vector2(SideFungusTexRealWidth / 2 - 2, SideFungusTexture.Height() / 2), new Rectangle(0, 0, 14, 10), col, 0f, 
                new Vector2(SideFungusTexRealWidth, SideFungusTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
			}

			//right side fungus
			if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 36)
			{
                spriteBatch.Draw(SideFungusTexture.Value, pos + new Vector2(SideFungusTexRealWidth / 2 + 12, SideFungusTexture.Height() / 2), new Rectangle(0, 12, 14, 10), col, 0f, 
                new Vector2(SideFungusTexRealWidth, SideFungusTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
			}

			//draw roots at the bottom of the tree
			if (Framing.GetTileSafely(i, j).TileFrameX == 0)
			{
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18)
				{
                    spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2, RootTexture.Height() / 2 + 6), new Rectangle(0, 0, 38, 14), col, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
				}

				if (Framing.GetTileSafely(i, j).TileFrameY == 36 || Framing.GetTileSafely(i, j).TileFrameY == 54)
				{
                    spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2, RootTexture.Height() / 2 + 6), new Rectangle(0, 16, 38, 14), col, 0f, 
                    new Vector2(RootsTexRealWidth, RootTexture.Height() / 2), 1f, SpriteEffects.None, 0f);
				}
			}

			//draw the tree tops
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
                //bottom stem part of the top
                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 19, 8), new Rectangle(0, 0, 112, 74), col, 0f, 
                new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);

                //actual mushroom cap with unique scaling for squish effect
                float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 15;
                Vector2 scale = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

                spriteBatch.Draw(CapTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 18, -16), new Rectangle(0, 0, 112, 74), col, 0f, 
                new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f * (Vector2.One + (0.1f * scale)), SpriteEffects.None, 0f);
			}
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}