using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Dusts
{
    public class DaffodilPollenDust : ModDust
	{
		private static Asset<Texture2D> DustTexture;

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 20, 20);
		}

		public override bool PreDraw(Dust dust)
		{
			DustTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 10 * dust.scale;

			Color color = Lighting.GetColor((int)dust.position.X / 16, (int)(dust.position.Y / 16));

			Main.spriteBatch.Draw(DustTexture.Value, currentCenter - Main.screenPosition, null, color, 
			dust.rotation, DustTexture.Size() * 0.5f, dust.scale * 2f, SpriteEffects.None, 0);
			
			return false;
		}

		public override bool Update(Dust dust)
		{
			if (dust.customData is null)
			{
				dust.position -= Vector2.One * 10 * dust.scale;
				dust.customData = true;
			}

			Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 10 * dust.scale;

			dust.scale = dust.scale * 0.98f;
			Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 10 * dust.scale;

			dust.rotation += 0.06f;
			dust.position += currentCenter - nextCenter;
			dust.position += dust.velocity;

			dust.velocity *= 0.97f;
			dust.color *= 0.9f;

			if (dust.scale <= 0f)
			{
				dust.active = false;
			}

			return false;
		}
	}
}