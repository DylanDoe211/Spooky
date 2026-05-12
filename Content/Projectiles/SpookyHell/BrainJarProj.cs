using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class BrainJarProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyHell/BrainJar";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
        }

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            player.AddBuff(ModContent.BuffType<BrainyBuff>(), 2);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Brainy>(), Projectile.damage, 0f, Projectile.owner);

            for (int numGores = 1; numGores <= 5; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/BrainJarGore" + numGores).Type);
				}
			}
		}
    }
}