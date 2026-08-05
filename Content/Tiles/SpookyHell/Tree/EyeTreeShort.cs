using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Items.Food;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
    public class EyeTreeShort : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = tree top draw segment
        //X frame 36 = left branch segment
        //X frame 54 = right branch segment
        //X frame 72 = both branch segment
        //X frame 90 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> StemTexture;
        private static Asset<Texture2D> BranchLeftTexture;
        private static Asset<Texture2D> BranchRightTexture;
        private static Asset<Texture2D> TopGlowTexture;
        private static Asset<Texture2D> StemGlowTexture;
        private static Asset<Texture2D> BranchLeftGlowTexture;
        private static Asset<Texture2D> BranchRightGlowTexture;

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
            AddMapEntry(new Color(168, 58, 96), name);
            RegisterItemDrop(ModContent.ItemType<LivingFleshItem>());
            DustType = DustID.Blood;
			HitSound = SoundID.NPCHit13;
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

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<EyeTreeShort>(), true);
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
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<EyeTreeShort>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //randomly place branch segment
                if (numSegments > 1 && numSegments < height - 1 && 
                Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX != 36 && 
                Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX != 54 &&
                Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX != 72)
                {
                    if (Main.rand.NextBool())
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                            {
                                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
                                break;
                            }
                            case 1:
                            {
                                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 54;
                                break;
                            }
                            case 2:
                            {
                                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 72;
                                break;
                            }
                        }
                    }
                }

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
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

            if (Main.tile[x, y].TileFrameX == 18)
            {
                //spawn a fruit from the tree
                if (Main.rand.NextBool(30))
                {
                    int NewItem = Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<EyeFruit>(), Main.rand.Next(1, 4));

                    if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
					}
                }
            }
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
            if (belowFrame < 90)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 90;
            }

            if (tile.TileFrameX == 18)
            {
                //play squishy sound
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                //spawn a seed from the tree
                if (Main.rand.NextBool())
                {
                    int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<EyeSeed>(), Main.rand.Next(1, 4));

                    if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
					}
                }

                int EyeBlock = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<EyeballBlockItem>(), Main.rand.Next(5, 11));

                if (Main.netMode == NetmodeID.MultiplayerClient && EyeBlock >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, EyeBlock, 1f);
                }

                //spawn gores out of the tree
                for (int numGores = 0; numGores <= Main.rand.Next(1, 3); numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16),
                        new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                    }
                }
            }

            //left branches
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 || Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
                for (int numGores = 0; numGores <= Main.rand.Next(1, 3); numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i - 2, j) * 16),
                        new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                    }
                }
            }

            //right branches
            if (Framing.GetTileSafely(i, j).TileFrameX == 54 || Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
                for (int numGores = 0; numGores <= Main.rand.Next(1, 3); numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i + 2, j) * 16),
                        new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                    }
                }
            }

            if (tile.TileFrameX == 90)
            {
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16),
                    new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore3").Type);
                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops");
            TopGlowTexture ??= ModContent.Request<Texture2D>(Texture + "TopsGlow");
            BranchLeftTexture ??= ModContent.Request<Texture2D>(Texture + "BranchLeft");
            BranchLeftGlowTexture ??= ModContent.Request<Texture2D>(Texture + "BranchLeftGlow");
            BranchRightTexture ??= ModContent.Request<Texture2D>(Texture + "BranchRight");
            BranchRightGlowTexture ??= ModContent.Request<Texture2D>(Texture + "BranchRightGlow");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);
            StemGlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            //divide tops texture width by 3 since there are 3 horizontal frames, then divide it by 2 to get half the width for the individual frame
            int TopsTexRealWidth = (TopTexture.Width() / 3) / 2;

            //divide branches texture width by 3 since there are 3 vertical frames, then divide it by 2 to get half the height for the individual frame
            int BranchLeftTexRealWidth = BranchLeftTexture.Width() / 2;
            int BranchRightTexRealWidth = BranchRightTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
            }

            //draw the actual tree
            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(StemGlowTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), TileGlobal.GetTileColorWithPaint(i, j, Color.White), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //draw tree tops
            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 7, 4), new Rectangle(60 * frame, 0, 58, 44), col, 0f, 
				new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TopGlowTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 7, 4), new Rectangle(60 * frame, 0, 58, 44), TileGlobal.GetTileColorWithPaint(i, j, Color.White), 0f, 
				new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);
            }

            //left branches
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 || Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
                spriteBatch.Draw(BranchLeftTexture.Value, pos + new Vector2(BranchLeftTexRealWidth / 2 - 43, -3), new Rectangle(0, 46 * frame, 58, 44), col, 0f, 
				new Vector2(BranchLeftTexRealWidth, TopTexture.Height() / 3), 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(BranchLeftGlowTexture.Value, pos + new Vector2(BranchLeftTexRealWidth / 2 - 43, -3), new Rectangle(0, 46 * frame, 58, 44), TileGlobal.GetTileColorWithPaint(i, j, Color.White), 0f, 
				new Vector2(BranchLeftTexRealWidth, TopTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
            }

            //right branches
            if (Framing.GetTileSafely(i, j).TileFrameX == 54 || Framing.GetTileSafely(i, j).TileFrameX == 72)
            {
                spriteBatch.Draw(BranchRightTexture.Value, pos + new Vector2(BranchRightTexRealWidth / 2 + 31, -3), new Rectangle(0, 46 * frame, 58, 44), col, 0f, 
				new Vector2(BranchRightTexRealWidth, TopTexture.Height() / 3), 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(BranchRightGlowTexture.Value, pos + new Vector2(BranchRightTexRealWidth / 2 + 31, -3), new Rectangle(0, 46 * frame, 58, 44), TileGlobal.GetTileColorWithPaint(i, j, Color.White), 0f, 
				new Vector2(BranchRightTexRealWidth, TopTexture.Height() / 3), 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}