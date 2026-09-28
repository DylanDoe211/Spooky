using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Dusts
{
    public class GlowyDust : ModDust
	{
		private static Asset<Texture2D> DustTexture;

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 10 * Main.rand.Next(0, 2), 10, 10);
			dust.rotation = Main.rand.NextFloat(0.001f, 0.01f);
		}
        
		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool PreDraw(Dust dust)
		{
			DustTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 5 * dust.scale;

			for (int repeats = 0; repeats < 4; repeats++)
            {
				Color color = new Color(125 - dust.alpha, 125 - dust.alpha, 125 - dust.alpha, 0).MultiplyRGBA(dust.color);

                Vector2 DrawPosition = currentCenter - dust.velocity * repeats;

				Main.spriteBatch.Draw(DustTexture.Value, DrawPosition - Main.screenPosition, dust.frame, color, 
				dust.rotation, dust.frame.Size() / 2, 0.8f + (dust.scale), SpriteEffects.None, 0);
			}
			
			return false;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData is null)
			{
				dust.position -= Vector2.One * 5 * dust.scale;
				dust.customData = true;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 5 * dust.scale;

			dust.scale = dust.scale * 0.98f;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 5 * dust.scale;

			dust.rotation += 0.06f;
			dust.position += currentCenter - nextCenter;
			dust.position += dust.velocity;

			dust.velocity *= 0.97f;
			dust.color *= 0.95f;

			if (!dust.noLight)
			{
				Lighting.AddLight(dust.position, dust.color.ToVector3());
			}

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			return false;
		}
	}
}