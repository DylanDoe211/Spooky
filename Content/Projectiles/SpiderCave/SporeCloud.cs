using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SporeCloud : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}
		
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.localNPCHitCooldown = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor) * Projectile.ai[1], Projectile.rotation, drawOrigin, Projectile.scale * Projectile.ai[2], SpriteEffects.None, 0);

            return true;
        }

        public override void AI()
		{
            Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.005f * (float)Projectile.direction;

            Projectile.velocity *= 0.95f;

            if (Projectile.timeLeft < 120)
            {
                Projectile.ai[2] -= 0.01f;

                if (Projectile.timeLeft < 75)
                {
                    if (Projectile.ai[1] > 0f)
                    {
                        Projectile.ai[1] -= 0.025f;
                    }
                }

                if (Projectile.timeLeft < 60)
                {
                    if (Projectile.alpha < 255)
                    {
                        Projectile.alpha += 5;
                    }
                }
            }
            else
            {
                Projectile.ai[2] = 2f;

                if (Projectile.ai[1] <= 0.25f)
                {
                    Projectile.ai[1] += 0.005f;
                }

                if (Projectile.alpha > 125)
                {
                    Projectile.alpha -= 5;
                }
            }
        }   
    }
}
     
          






