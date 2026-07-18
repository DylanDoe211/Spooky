using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Shipyard.Ambient
{
	public class MossyAnchor1 : ModTile
	{
		private Asset<Texture2D> GlowTexture1;
		private Asset<Texture2D> GlowTexture2;

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.Origin = new Point16(3, 3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
			Main.tileLighted[Type] = true;
			AddMapEntry(new Color(50, 46, 43));
			DustType = DustID.Ash;
			HitSound = SoundID.Dig;
		}
		
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 1300f;

			r = 54f / divide;
			g = 199f / divide;
			b = 191f / divide;
		}
	}

	public class MossyAnchor2 : MossyAnchor1{}
	public class MossyAnchor3 : MossyAnchor1{}
}