using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Shipyard.Tree
{
    public class CoralTree : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = branch draw segment
        //X frame 36 = top draw segment
        //X frame 54 = stubby top segment

        static int ColorVariant = 0;

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> StemTexture;
        private static Asset<Texture2D> BranchTexture;

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
            AddMapEntry(new Color(220, 220, 220), name);
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

        public static bool Grow(int i, int j, int minSize, int maxSize, int ColorType, bool saplingExists = false)
        {
            ColorVariant = ColorType;

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

            if (height < minSize)
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<CoralTree>(), true);
				Framing.GetTileSafely(i, j).TileFrameX = 0;
				Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(6) * 18);

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
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<CoralTree>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(6) * 18);

                //randomly place branch segment
                if (numSegments < height - 2)
                {
                    if (numSegments % 2 == 0)
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                    }
                }

                //top segment
                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
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
            TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops");
			BranchTexture ??= ModContent.Request<Texture2D>(Texture + "Branches");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);
            
            float xOff = (float)Math.Sin((j * 25) * 0.04f) * 1.2f;
			Vector2 WavyOffset = new Vector2((xOff), -2);

            Color realColor = Color.White;
            switch (ColorVariant)
            {
                case 0:
                {
                    realColor = Color.Blue;
                    break;
                }
                case 1:
                {
                    realColor = Color.Green;
                    break;
                }
                case 2:
                {
                    realColor = Color.Pink;
                    break;
                }
                case 3:
                {
                    realColor = Color.Purple;
                    break;
                }
                case 4:
                {
                    realColor = Color.Teal;
                    break;
                }
            }

			Tile tile = Framing.GetTileSafely(i, j);
            Color tilePaintCol = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Color col = tilePaintCol; //realColor.MultiplyRGBA(tilePaintCol);
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            //divide top and branch texture width by 6 since there are 6 horizontal frames, then divide it by 2 to get half the width for the individual frame
			int TopsTexRealWidth = (TopTexture.Width() / 6) / 2;
            int BranchesTexRealWidth = (BranchTexture.Width() / 6) / 2;
            
            int frame = tile.TileFrameY / 18;

			//draw extra tile below so it looks attached to the ground
			if (Main.tile[i, j + 1].TileType != Type)
			{
				spriteBatch.Draw(StemTexture.Value, pos + WavyOffset, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
			}

			//draw the actual tree
			spriteBatch.Draw(StemTexture.Value, pos + WavyOffset, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//draw tops
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
                int TextureWidth = (TopTexture.Width() / 6);
                int TextureHeight = TopTexture.Height();

                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 6, 0) + WavyOffset, new Rectangle(TextureWidth * frame, 0, TextureWidth, TextureHeight), col, 0f, 
				new Vector2(TopsTexRealWidth, TextureHeight), 1f, SpriteEffects.None, 0f);
			}

            //draw branches infront of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int TextureWidth = (BranchTexture.Width() / 6);
                int TextureHeight = BranchTexture.Height();

                //left branches
                if (Framing.GetTileSafely(i, j).TileFrameY <= 36)
                {
                    spriteBatch.Draw(BranchTexture.Value, pos + new Vector2(BranchesTexRealWidth / 2 - 5, 14) + WavyOffset, new Rectangle(TextureWidth * frame, 0, TextureWidth, TextureHeight), col, 0f, 
                    new Vector2(BranchesTexRealWidth, TextureHeight), 1f, SpriteEffects.None, 0f);
                }
                //right branches
                else
                {
                    spriteBatch.Draw(BranchTexture.Value, pos + new Vector2(BranchesTexRealWidth / 2 + 2, 14) + WavyOffset, new Rectangle(TextureWidth * frame, 0, TextureWidth, TextureHeight), col, 0f, 
                    new Vector2(BranchesTexRealWidth, TextureHeight), 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
	}
}