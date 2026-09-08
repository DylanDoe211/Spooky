using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Pets;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdPetSeedTile : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(170, 132, 101));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.15f);
        }

        public override void NearbyEffects(int i, int j, bool closer)
		{
			if (Main.rand.NextBool(60))
			{
				int NewDust = Dust.NewDust(new Vector2((i * 16) + 6, (j * 16) + 6), 1, 1, DustID.CopperCoin);
                Main.dust[NewDust].noLight = true;
                Main.dust[NewDust].velocity.X = 0;
                Main.dust[NewDust].velocity.Y = Main.rand.NextFloat(-0.5f, 0);
			}
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 1000f;

            r = 255f / divide;
            g = 128f / divide;
            b = 0f / divide;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
            int variant = Framing.GetTileSafely(i, j).TileFrameX / 36;
			switch (variant)
			{
				case 0:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedGreen>());
					break;
				}
                case 1:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedLime>());
					break;
				}
                case 2:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedOrange>());
					break;
				}
                case 3:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedRed>());
					break;
				}
                case 4:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedWhite>());
					break;
				}
                case 5:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedYellow>());
					break;
				}
            }
        }
    }
}