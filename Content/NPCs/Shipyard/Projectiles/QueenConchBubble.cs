using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard.Projectiles
{
    public class QueenConchBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.scale = 0.1f;
        }

		public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f;

            if (Projectile.scale < 1f)
            {
                Projectile.scale += 0.01f;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 30)
            {
                Projectile.velocity.Y -= 0.5f;
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, default, 1f);
                Main.dust[newDust].color = Color.Teal;
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