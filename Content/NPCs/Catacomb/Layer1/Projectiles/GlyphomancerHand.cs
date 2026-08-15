using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Catacomb.Layer1.Projectiles
{
    public class GlyphomancerHand : ModProjectile
    {
		float Distance = 0f;
		float RotationSpeed = 15f;

		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			for (int i = 0; i < 360; i += 90)
			{
				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(98, 147, 64));

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(1f, 2f)).RotatedBy(MathHelper.ToRadians(i));

				Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			}

			return false;
		}

		public override void AI()
        {
			Projectile Parent = Main.projectile[(int)Projectile.ai[1]];

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = Parent.Center.X - vector.X;
			float RotateY = Parent.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			if (Projectile.timeLeft <= 30)
			{
				Projectile.alpha += 10;
			}

			if (RotationSpeed > 2f)
			{
				RotationSpeed -= 0.15f;
			}

			if (Distance < 140)
			{
				Distance += 1.5f;
			}

			Projectile.ai[0] += RotationSpeed * Projectile.ai[2];
			double rad = Projectile.ai[0] * (Math.PI / 180);
			Projectile.position.X = Parent.Center.X - (int)(Math.Cos(rad) * Distance) - Projectile.width / 2;
			Projectile.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * Distance) - Projectile.height / 2;
		}
	}
}