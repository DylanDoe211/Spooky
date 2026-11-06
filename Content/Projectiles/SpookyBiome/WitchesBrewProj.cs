using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class WitchesBrewProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyBiome/WitchesBrew";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.65f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            //spawn splatter
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, -1)), ModContent.ProjectileType<GreenSplatter>(), 0, 0f);
            }

            for (int numProjectile = 0; numProjectile <= 5; numProjectile++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-3, 4)), 
                    ModContent.ProjectileType<WitchesBrewMist>(), Projectile.damage / 2, 0f, Projectile.owner, ai0: Main.rand.Next(0, 2));
                }
            }

            for (int numDust = 0; numDust < 20; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
			}
		}
    }
}