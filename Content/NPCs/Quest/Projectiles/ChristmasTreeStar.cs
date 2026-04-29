using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class ChristmasTreeStar : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor)
		{
            return Color.White * (1f - (Projectile.alpha / 255f));
        }

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color TrailColorFade = Color.Lerp(Color.Gold, Color.Gray, scale);
				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(TrailColorFade);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 1.2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.035f * (float)Projectile.direction;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.3f, 0f);

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
            
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 50)
            {
                Projectile.velocity *= 0.975f;
            }
            else
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] <= 50)
                {
                    int foundTarget = HomeOnTarget();
                    if (foundTarget != -1)
                    {
                        Player target = Main.player[foundTarget];
                        Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * (Projectile.ai[1] / 3);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    }
                }
                else
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                }
            }
        }

        private int HomeOnTarget()
        {
            int target = -1;
            float distance = 2000f;
            for (int k = 0; k < 255; k++) 
            {
                if (Main.player[k].active && !Main.player[k].dead)
                {
                    Vector2 center = Main.player[k].Center;
                    float currentDistance = Vector2.Distance(center, Projectile.Center);
                    if (currentDistance < distance || target == -1) 
                    {
                        distance = currentDistance;
                        target = k;
                    }
                }
            }

            return target;
        }

        public override void OnKill(int timeLeft)
		{
        	float maxAmount = 35;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                float intensity = Main.rand.NextFloat(2f, 12f);

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int newDust = Dust.NewDust(Projectile.Center, 0, 0, 57, 0f, 0f, 100, default, 2f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position = Projectile.Center + vector12;
				Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }
}