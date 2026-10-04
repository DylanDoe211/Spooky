using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Shipyard
{
    public class BlackSandstoneSlabWall : ModWall 
    {
		private static Asset<Texture2D> WallTexture;

		public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(23, 25, 32));
            DustType = DustID.Ash;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			WallTexture ??= ModContent.Request<Texture2D>(Texture);

			//frame using the composite sheet, where 468 is the width and 180 is the height of each wall sheet
			//with 2 and 4 being the number of separatewall sheets horizontally and vertically respectively
			Rectangle frame = new Rectangle(tile.WallFrameX + (i % 2 * 468), tile.WallFrameY + (j % 4 * 180), 32, 32);

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}

			Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

			Main.spriteBatch.Draw(WallTexture.Value, pos + new Vector2(-8, -8), frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
    }
}