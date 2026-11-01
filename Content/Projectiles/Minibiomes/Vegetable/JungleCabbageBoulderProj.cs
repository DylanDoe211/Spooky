using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
	public class JungleCabbageBoulderProj : ModProjectile
	{
		public override void SetDefaults()
        {
            Projectile.width = 48;
			Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
			Projectile.aiStyle = -1;
		}

		public override bool? CanCutTiles()
        {
            return false;
        }
		
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = true;
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.05f * (float)Projectile.direction;

			if (Projectile.ai[2] == 0)
			{
				Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

				Projectile.ai[1] = target.Center.X < Projectile.Center.X ? -1 : 1;
				Projectile.ai[2]++;

				Projectile.netUpdate = true;
			}
			else
			{
				Projectile.ai[0] += 1f;
				if (Projectile.ai[0] > 5f)
				{
					if (Projectile.velocity.Y <= 0f)
					{
						Projectile.velocity.X += 0.05f * Projectile.ai[1];
					}

					Projectile.velocity.Y += 0.2f;
				}
				
				if (Projectile.velocity.Y > 16f)
				{
					Projectile.velocity.Y = 16f;
				}

				if (Projectile.velocity.X != 0)
				{
					if (Collision.SolidTilesVersatile((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.X + 40 * Projectile.ai[1]) / 16, (int)Projectile.Top.Y / 16, (int)Projectile.Bottom.Y / 16 - 3))
					{
						Projectile.Kill();
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);

            for (int numDusts = 0; numDusts < 35; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 2.5f);
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
				Main.dust[dust].velocity = Projectile.velocity;
                Main.dust[dust].noGravity = true;
            }
		}
	}
}