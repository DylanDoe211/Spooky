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
    public class MangroveTree : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = root draw segment
        //X frame 36 = tree top draw segment
        //X frame 54 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> TopGlowTexture;
        private static Asset<Texture2D> TopEyeGlowTexture;
        private static Asset<Texture2D> RootTexture;
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
            AddMapEntry(new Color(101, 98, 94), name);
            RegisterItemDrop(ModContent.ItemType<RotWoodItem>());
            DustType = DustID.Ash;
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
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || 
            Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Grow(int i, int j, int minSize, int maxSize)
        {
            int height = WorldGen.genRand.Next(minSize, maxSize);
            for (int k = 1; k < height; ++k)
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

            //do not allow the tree to grow if theres not enough room
            for (int yCheck = j; yCheck <= j - 7; yCheck--)
            {
                if (Main.tile[i, yCheck].TileType > 0)
                {
                    return false;
                }
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<MangroveTree>(), true);
				Framing.GetTileSafely(i, j).TileFrameX = 72;
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
                //place tree segments
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<MangroveTree>(), true);
                if (numSegments == 2)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                }
				else if (numSegments < 2)
				{
					Framing.GetTileSafely(i, j - numSegments).TileFrameX = 72; //invisible frames
				}
                else
                {   
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                }
                
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //place the tree top at the top of the tree
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

			if (!Framing.GetTileSafely(i, j - 1).HasTile && (Framing.GetTileSafely(i, j).TileFrameX > 54 || Framing.GetTileSafely(i, j + 1).TileFrameX > 54))
			{
				(int x, int y) = (i, j);
				KillEntireTreeDown(ref x, ref y);
			}
		}

		private void KillEntireTreeDown(ref int x, ref int y)
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

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
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
            TopGlowTexture ??= ModContent.Request<Texture2D>(Texture + "TopsGlow");
            TopEyeGlowTexture ??= ModContent.Request<Texture2D>(Texture + "TopsEyeGlow");
			RootTexture ??= ModContent.Request<Texture2D>(Texture + "Roots");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			Vector2 pos = TileGlobal.TileCustomPosition(i, j, TileGlobal.TileOffset);

            //divide tops texture width by 3 since there are 3 horizontal frames, then divide it by 2 to get half the width for the individual frame
			int TopsTexRealWidth = (TopTexture.Width() / 3) / 2;
            int RootsTexRealWidth = RootTexture.Width() / 2;

            int frame = tile.TileFrameY / 18;

			//draw the actual tree
			spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//draw the tree roots
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
			{
                spriteBatch.Draw(RootTexture.Value, pos + new Vector2(RootsTexRealWidth / 2 - 24, 90), new Rectangle(0, 0, 150, 80), col, 0f, 
				new Vector2(RootsTexRealWidth, RootTexture.Height()), 1f, SpriteEffects.None, 0f);
			}

			//draw the tree tops
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
                Rectangle TopFrame = new Rectangle(164 * frame, 0, 162, 136);

                float WindRotation = ModContent.GetInstance<SpookyWorld>().GetTreeSway(i, j, ref pos) * 0.08f;

                spriteBatch.Draw(TopTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 31, 4), TopFrame, col, WindRotation, 
				new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TopGlowTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 31, 4), TopFrame, TileGlobal.GetTileColorWithPaint(i, j, Color.White * 0.05f), WindRotation, 
				new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TopEyeGlowTexture.Value, pos + new Vector2(TopsTexRealWidth / 2 - 31, 4), TopFrame, TileGlobal.GetTileColorWithPaint(i, j, Color.White * 0.5f), WindRotation,
				new Vector2(TopsTexRealWidth, TopTexture.Height()), 1f, SpriteEffects.None, 0f);
			}
		}
        
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);

			return false;
        }
	}
}