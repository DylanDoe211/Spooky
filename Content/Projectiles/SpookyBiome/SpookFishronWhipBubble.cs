using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class SpookFishronWhipBubble : ModProjectile
    {
        float Scale = 0f;
        float GlowRotation = 0f;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Scale, SpriteEffects.None, 0);

            //draw spinning fireballs inside the bubble
            var spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, vector + circular, rectangle, color, GlowRotation, drawOrigin, Scale, spriteEffects, 0);
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y;
            }

			return false;
		}

        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            SoundEngine.PlaySound(SoundID.Item54 with { Volume = 2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpookFishronWhipBubbleExplosion>(), Projectile.damage, 0f, Projectile.owner);

            for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, default, 1f);
                Main.dust[newDust].color = Color.OrangeRed;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}

            Projectile.Kill();
        }

        public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            GlowRotation += 0.1f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity *= 0.935f;
            }

            if (Scale < 1f)
            {
                Scale += 0.1f;
            }

            if (Projectile.timeLeft > 60)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
            }
            else
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 5;
                }
            }
        }
    }
}