using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
    public class TomatoRed : ModProjectile
    {
		public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 500;
        }

        public override void AI()       
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.022f * (float)Projectile.direction;

			Projectile.velocity.Y = Projectile.velocity.Y + 0.45f;
            Projectile.velocity.X = Projectile.velocity.X * 0.99f;
        }

		public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            //spawn blood splatter
			for (int i = 0; i < 5; i++)
			{
				Vector2 Position = new Vector2(0, -Main.rand.NextFloat(5f, 9f)).RotatedByRandom(360);
			}

			//spawn blood explosion clouds
			for (int numExplosion = 0; numExplosion < 6; numExplosion++)
			{
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red, 0.5f);
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].velocity *= 0.01f;
				Main.dust[DustGore].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[DustGore].scale = 0.5f;
					Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
        }
    }

    public class TomatoOrange : TomatoRed
    {
        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            //spawn blood splatter
			for (int i = 0; i < 5; i++)
			{
				Vector2 Position = new Vector2(0, -Main.rand.NextFloat(5f, 9f)).RotatedByRandom(360);
			}

			//spawn blood explosion clouds
			for (int numExplosion = 0; numExplosion < 6; numExplosion++)
			{
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Orange, 0.5f);
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].velocity *= 0.01f;
				Main.dust[DustGore].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[DustGore].scale = 0.5f;
					Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
        }
    }

    public class TomatoYellow : TomatoRed
    {
        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            //spawn blood splatter
			for (int i = 0; i < 5; i++)
			{
				Vector2 Position = new Vector2(0, -Main.rand.NextFloat(5f, 9f)).RotatedByRandom(360);
			}

			//spawn blood explosion clouds
			for (int numExplosion = 0; numExplosion < 6; numExplosion++)
			{
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gold, 0.5f);
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].velocity *= 0.01f;
				Main.dust[DustGore].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[DustGore].scale = 0.5f;
					Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
        }
    }
}