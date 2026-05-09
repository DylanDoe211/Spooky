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
			dust.frame = new Rectangle(0, 0, 64, 64);
		}
        
		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color;
		}

		public override bool PreDraw(Dust dust)
		{
			DustTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

			Main.spriteBatch.Draw(DustTexture.Value, currentCenter - Main.screenPosition, null, dust.color.MultiplyRGBA(new Color(255, 255, 255, 0)), 
			dust.rotation, DustTexture.Size() * 0.5f, dust.scale * 2f, SpriteEffects.None, 0);
			
			return false;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData is null)
			{
				dust.position -= Vector2.One * 32 * dust.scale;
				dust.customData = true;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

			dust.scale = dust.scale * 0.98f;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

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