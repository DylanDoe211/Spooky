using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class BloodBallTooth : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[1] == 0)
            {
                Projectile.velocity.X = 0;

                Projectile.ai[1] = 1;
            }

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
            }
        }
    }
}