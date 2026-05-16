using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class OpalSlingshotBone : ModProjectile
    {
		int Bounces = 0;

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 3;
			ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 500;
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

                Color color = Color.White;

                switch ((int)Projectile.ai[1])
                {
                    case 0:
                    {
                        color = Color.Cyan;
                        break;
                    }
                    case 1:
                    {
                        color = Color.HotPink;
                        break;
                    }
                    case 2:
                    {
                        color = Color.Gold;
                        break;
                    }
                }

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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
        }

        public override void AI()       
        {
			Projectile.frame = (int)Projectile.ai[1];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.45f;
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
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

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 5)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 30;
				
				SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);

				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
					Projectile.velocity.X = -oldVelocity.X * 0.85f;
				}
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                }
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] = 30;

			SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);

			Projectile.velocity.Y = -Projectile.velocity.Y * 1.1f;
		}

		public override void OnKill(int timeLeft)
		{	
			SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);
		}
    }
}