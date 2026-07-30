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
using Spooky.Content.Items.Minibiomes.Ocean;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Tree
{
    internal class BoneFishTreeHanging : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = tree bottom segment
        //X frame 36 = stubby bottom segment

        private static Asset<Texture2D> BottomTexture;
        private static Asset<Texture2D> StemTexture;
        private static Asset<Texture2D> BottomGlowTexture;
        private static Asset<Texture2D> StemGlowTexture;

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
            AddMapEntry(new Color(148, 151, 114), name);
            RegisterItemDrop(ModContent.ItemType<FishboneChunk>());
            DustType = DustID.Bone;
			HitSound = SoundID.DD2_SkeletonHurt;
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
                if (SolidTile(i, j + k + 1))
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
                WorldGen.PlaceTile(i, j, ModContent.TileType<BoneFishTreeHanging>(), true);
                Framing.GetTileSafely(i, j).TileFrameX = 0;
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(2) * 18);

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
                WorldGen.PlaceTile(i, j + numSegments, ModContent.TileType<BoneFishTreeHanging>(), true);
                Framing.GetTileSafely(i, j + numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j + numSegments).TileFrameY = (short)(WorldGen.genRand.Next(2) * 18);

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
            if (aboveFrame < 36)
            {
                Framing.GetTileSafely(i, j - 1).TileFrameX = 36;
            }

            if (tile.TileFrameX == 18)
            {
                if (tile.TileFrameY == 0)
                {
                    //spawn gores out of the tree
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j + 2) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeHangingBottomGore1").Type);
                    }
                }
                else
                {
                    //spawn gores out of the tree
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j + 2) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeHangingBottomGore2").Type);
                    }
                }
            }
            
            if (tile.TileFrameX == 0)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeGore").Type);
                }
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
            BottomTexture ??= ModContent.Request<Texture2D>(Texture + "Bottom");
            BottomGlowTexture ??= ModContent.Request<Texture2D>(Texture + "BottomGlow");
            StemTexture ??= ModContent.Request<Texture2D>(Texture);
			StemGlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;
            Vector2 WavyOffset = new Vector2((xOff * 2), 0);

            Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            float glowspeed = Main.GameUpdateCount * 0.01f;
			float glowbrightness = (float)MathF.Sin(j / 5f - glowspeed);

			if (glowbrightness > 0)
			{
				Lighting.AddLight(new Vector2(i * 16, j * 16), Color.Lime.ToVector3() * 0.35f * glowbrightness);
			}

            //divide texture width by 4 since there are 4 horizontal frames, then divide it by 2 to get half the width for the individual frame
			int BottomTexRealWidth = (BottomTexture.Width() / 2) / 2;

            int frame = tile.TileFrameY / 18;

            //draw tree glow stuff
            if (tile.TileFrameX == 18)
            {
                for (int circle = 0; circle < 360; circle += 90)
                {
                    Vector2 circular = new Vector2(2f, 0).RotatedBy(MathHelper.ToRadians(circle));

				    spriteBatch.Draw(BottomGlowTexture.Value, pos + new Vector2(BottomTexRealWidth / 2 - 2, 16) + WavyOffset + circular, new Rectangle(46 * frame, 0, 46, 34), Color.Lime * 0.85f * glowbrightness, 0f, 
				    new Vector2(BottomTexRealWidth, 0), 1f, SpriteEffects.None, 0f);
                }
            }

			spriteBatch.Draw(StemGlowTexture.Value, pos + WavyOffset + new Vector2(2, 0), new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
			Color.Lime * 0.85f * glowbrightness, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(StemGlowTexture.Value, pos + WavyOffset + new Vector2(-2, 0), new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
			Color.Lime * 0.85f * glowbrightness, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //draw the actual tree itself
            if (tile.TileFrameX == 18)
            {
                spriteBatch.Draw(BottomTexture.Value, pos + new Vector2(BottomTexRealWidth / 2 - 2, 16) + WavyOffset, new Rectangle(46 * frame, 0, 46, 34), col, 0f, 
                new Vector2(BottomTexRealWidth, 0), 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(StemTexture.Value, pos + WavyOffset, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
    }
}