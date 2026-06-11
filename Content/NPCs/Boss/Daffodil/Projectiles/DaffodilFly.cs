using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class DaffodilFly : ModProjectile
    {
        Vector2 SavePos = Vector2.Zero;

        private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 26;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16));

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}
		
		public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            if (Projectile.ai[0] > 0)
            {
                if (SavePos == Vector2.Zero)
                {
                    SavePos = Projectile.Center;
                }
                else
                {
                    GoToPosition(SavePos);
                }
            }

            foreach (var Proj in Main.ActiveProjectiles)
            {
                if (Proj.active && Proj.type == ModContent.ProjectileType<Corpsebloom>() && Projectile.Hitbox.Intersects(Proj.Hitbox))
                {
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
		}

        public void GoToPosition(Vector2 GoToPos)
        {
            float goToX = GoToPos.X - Projectile.Center.X + Main.rand.Next(-50, 50);
            float goToY = GoToPos.Y - Projectile.Center.Y + Main.rand.Next(-50, 50);

            float speedLimit = 5f;
            float speed = 0.08f;

            if (Vector2.Distance(Projectile.Center, GoToPos) >= 135)
            {
                speed = speedLimit;
            }
            else
            {
                speed = 2f;
            }

            if (speed >= speedLimit)
            {
                speed = speedLimit;
            }
            
            if (Projectile.velocity.X > speed)
            {
                Projectile.velocity.X *= 0.98f;
            }
            if (Projectile.velocity.Y > speed)
            {
                Projectile.velocity.Y *= 0.98f;
            }

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
    }
}