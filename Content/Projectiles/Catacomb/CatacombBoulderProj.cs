using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class CatacombBoulderProj : ModProjectile
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
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X * 0.25f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y * 0.25f;
			}

			return false;
		}

		public override void AI()
		{
			Projectile.frame = (int)Projectile.ai[2];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y * 0.5f)) * 0.05f * (float)Projectile.direction;

			Projectile.velocity.Y += 0.35f;

			if (Projectile.localAI[0] == 0)
			{
				if (Collision.SolidCollision(new Vector2(Projectile.Center.X - 3, Projectile.Bottom.Y - 5), 6, 6))
				{
					Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

					Projectile.ai[1] = target.Center.X < Projectile.Center.X ? -1 : 1;
					Projectile.localAI[0]++;

					Projectile.netUpdate = true;
				}
			}
			else
			{
				Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

				if (Projectile.velocity.X < 10 && Projectile.velocity.X > -10)
				{
					Projectile.velocity.X += 0.12f * Projectile.ai[1];
				}

				if (Projectile.velocity.X != 0)
				{
					if (Collision.SolidTilesVersatile((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.X + 22 * Projectile.ai[1]) / 16, (int)Projectile.Top.Y / 16, (int)Projectile.Bottom.Y / 16 - 3))
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
				int DustType = Projectile.ai[2] == 3 ? DustID.Gold : DustID.Bone;

                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 100, default, 1f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-1, 2);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-1, 2);
            }
		}
	}

	public class CatacombBoulderProjGold : CatacombBoulderProj
	{
		public override string Texture => "Spooky/Content/Projectiles/Catacomb/CatacombBoulderProj";
	}
}