using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{
	public class MoldSpore : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 22;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 500;
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
            Player player = Main.LocalPlayer;

            if (Projectile.alpha > 0 && Projectile.timeLeft > 60)
            {
                Projectile.alpha -= 5;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

			Projectile.rotation = Projectile.velocity.X * 0.1f;

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

			Projectile.ai[0]++;
            if (Projectile.ai[0] < 85)
            {
                float goToX = (player.Center.X + Main.rand.Next(-15, 15)) - Projectile.Center.X;
                float goToY = (player.Center.Y + Main.rand.Next(-15, 15)) - Projectile.Center.Y;
                float speed = 0.025f;

                if (Projectile.velocity.X < goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                    if (Projectile.velocity.X < 0f && goToX > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                    }
                }
                else if (Projectile.velocity.X > goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                    if (Projectile.velocity.X > 0f && goToX < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                    }
                }
                if (Projectile.velocity.Y < goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    if (Projectile.velocity.Y < 0f && goToY > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        return;
                    }
                }
                else if (Projectile.velocity.Y > goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    if (Projectile.velocity.Y > 0f && goToY < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        return;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }
		}
	}
}