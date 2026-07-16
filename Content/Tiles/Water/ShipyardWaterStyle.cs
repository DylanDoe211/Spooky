using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class ShipyardWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/ShipyardWaterfallStyle").Slot;

		public override int GetSplashDust() => 99;

		public override int GetDropletGore() => ModContent.Find<ModGore>("Spooky/ShipyardWaterDroplet").Type;

		public override Asset<Texture2D> GetRainTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Tiles/Water/ShipyardRain");
		}

		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0.96f;
			g = 0.99f;
			b = 0.99f;
		}

		public override Color BiomeHairColor() => Color.SkyBlue;
	}
}