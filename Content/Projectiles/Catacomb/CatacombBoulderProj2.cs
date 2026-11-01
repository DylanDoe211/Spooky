using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class CatacombBoulderProj2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
        {
            Projectile.width = 32;
			Projectile.height = 32;
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
			Projectile.frame = (int)Projectile.ai[2];

			//first launch at the player
			if (Projectile.localAI[0] == 0)
			{
				Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

				Projectile.ai[1] = target.Center.X < Projectile.Center.X ? -1 : 1;
				Projectile.velocity.X = 12 * Projectile.ai[1];
				Projectile.localAI[0]++;

				Projectile.netUpdate = true;
			}
			//fall down and then check if it hits the ground and then roll
			if (Projectile.localAI[0] == 1)
			{
				Projectile.rotation -= Math.Abs(Projectile.velocity.X) * 0.065f * (float)Projectile.direction;

				Projectile.velocity.Y += 0.35f;

				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 10)
				{
					if (Collision.SolidCollision(new Vector2(Projectile.Center.X - 3, Projectile.Bottom.Y - 5), 6, 6))
					{
						Projectile.localAI[0]++;

						Projectile.netUpdate = true;
					}
				}
			}
			//rolling behavior
			if (Projectile.localAI[0] == 2)
			{
				//-1 = going to the right
				//1 = going to the left

				Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

				if (Projectile.ai[1] == -1)
				{
					if (Projectile.velocity.X < 0)
					{
						Projectile.rotation -= Math.Abs(Projectile.velocity.X) * 0.05f * (float)Projectile.direction;
					}
					else
					{
						Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.05f * (float)Projectile.direction;
					}
				}

				if (Projectile.ai[1] == 1)
				{
					if (Projectile.velocity.X > 0)
					{
						Projectile.rotation -= Math.Abs(Projectile.velocity.X) * 0.05f * (float)Projectile.direction;
					}
					else
					{
						Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.05f * (float)Projectile.direction;
					}
				}

				if ((Projectile.velocity.X < 10 && Projectile.ai[1] == -1) || (Projectile.velocity.X > -10 && Projectile.ai[1] == 1))
				{
					Projectile.velocity.X += 0.25f * -Projectile.ai[1];
				}

				Projectile.velocity.Y += 0.2f;

				if (Projectile.velocity.X != 0)
				{
					if (Collision.SolidTilesVersatile((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.X + 22 * -Projectile.ai[1]) / 16, (int)Projectile.Top.Y / 16, (int)Projectile.Bottom.Y / 16 - 3))
					{
						Projectile.Kill();
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);

            for (int numDust = 0; numDust < 25; numDust++)
            {
				int DustType = Projectile.ai[2] == 3 ? DustID.Gold : DustID.t_Lihzahrd;

                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 100, default, 1f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-1, 2);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-1, 2);
            }
		}
	}

	public class CatacombBoulderProj2Gold : CatacombBoulderProj2
	{
		public override string Texture => "Spooky/Content/Projectiles/Catacomb/CatacombBoulderProj2";
	}
}