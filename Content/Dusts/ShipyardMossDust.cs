using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Dusts
{
    public class ShipyardMossDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0f;
			dust.scale *= Main.rand.NextFloat(1f, 1.5f);
			dust.noGravity = true;
			dust.noLight = true;
			dust.alpha = 0;
			dust.fadeIn = 12f;
			dust.frame = new Rectangle(0, 0, 8, 8);
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return lightColor;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.X += (float)Main.rand.Next(-10, 11) * 0.01f;
			dust.velocity.Y += (float)Main.rand.Next(-10, -2) * 0.01f;
			dust.position += dust.velocity;

			dust.rotation += (Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y)) * 0.01f;

			if ((double)dust.velocity.X > 0.75)
			{
				dust.velocity.X = 0.75f;
			}
			if ((double)dust.velocity.X < -0.75)
			{
				dust.velocity.X = -0.75f;
			}
			if ((double)dust.velocity.Y > 0.75)
			{
				dust.velocity.Y = 0.75f;
			}
			if ((double)dust.velocity.Y < -0.75)
			{
				dust.velocity.Y = -0.75f;
			}
			dust.scale -= 0.005f;
			if (dust.scale < 0f || WorldGen.SolidTile(Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16]))
			{
				dust.active = false;
			}

			float divide = 1000f;

			float r = 39f / divide;
			float g = 199f / divide;
			float b = 135f / divide;

			Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), r * dust.scale, g * dust.scale, b * dust.scale);

			return false;
		}
	}
}