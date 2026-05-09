using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MagicBean : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()       
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
            }
        }

		public override void OnKill(int timeLeft)
		{
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-3, 4), -8), 
            ModContent.ProjectileType<MagicBeanstalk>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

			for (int numDusts = 0; numDusts < 5; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
					
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}