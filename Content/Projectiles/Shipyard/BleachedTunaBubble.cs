using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class BleachedTunaBubble : ModProjectile
    {
		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
		}

        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 20;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 30;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, default, 1f);
				if (Projectile.ai[0] == 0)
				{
					Main.dust[newDust].color = Color.Lavender;
				}
				if (Projectile.ai[0] == 1)
				{
					Main.dust[newDust].color = Color.Pink;
				}
				if (Projectile.ai[0] == 2)
				{
					Main.dust[newDust].color = Color.White;
				}
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}
		}
    }
}
     
          






