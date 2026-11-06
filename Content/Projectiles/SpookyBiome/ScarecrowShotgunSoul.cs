using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class ScarecrowShotgunSoul : ModProjectile
    {
		Vector2 RotatePosition;

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
			Projectile.width = 10;
            Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.hostile = false;                              			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
		}

		public override bool PreDraw(ref Color lightColor)
        {
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Color RealColor = Color.White;

            switch ((int)Projectile.ai[2])
            {
                case 0:
                {
                    RealColor = Color.Gold;
                    break;
                }
                case 1:
                {
                    RealColor = Color.DarkViolet;
                    break;
                }
                case 2:
                {
                    RealColor = Color.Red;
                    break;
                }
                case 3:
                {
                    RealColor = Color.Lime;
                    break;
                }
            }

			//draw afterimage trail
			Vector2 drawOriginTrail = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.White, RealColor, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					break;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, drawOriginTrail, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			//draw aura
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = i switch
                {
                    1 => new(0, -2),
                    2 => new(2, 0),
                    3 => new(0, 2),
                    _ => new(-2, 0)
                };

                Main.EntitySpriteDraw(DrawUtils.ColorSolid(ProjTexture.Value, Color.White), vector + offset, rectangle, 
                Projectile.GetAlpha(RealColor * 0.75f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
		
		public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				Projectile.localAI[0] = Main.rand.Next(5, 61);
			}
			if (Projectile.ai[0] == Projectile.localAI[0])
			{
				RotatePosition = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Main.rand.NextBool() ? -25 : 25));
			}
			if (Projectile.ai[0] > Projectile.localAI[0])
			{
				double angle = Projectile.DirectionTo(RotatePosition).ToRotation() - Projectile.velocity.ToRotation();
				while (angle > Math.PI)
				{
					angle -= 2.0 * Math.PI;
				}
				while (angle < -Math.PI)
				{
					angle += 2.0 * Math.PI;
				}

				if (Math.Abs(angle) > Math.PI / 2)
				{
					Projectile.ai[1] = Math.Sign(angle);
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10;
				}

				Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(12f) * Projectile.ai[1]);
			}
			if (Projectile.ai[0] >= Projectile.localAI[0] + 60)
			{
				Projectile.ai[0] = 0;
			}

			if (Projectile.timeLeft <= 60)
            {
                Projectile.scale *= 0.97f;
            }
			if (Projectile.timeLeft <= 30)
            {
                Projectile.alpha += 13;
            }

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
		}
	}
}