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
    public class ElGourdoProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyBiome/ElGourdo";

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much it can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

            int height = ProjTexture.Height() / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, ProjTexture.Width(), height);

            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, 
            Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), scaleStretch, effects, 0);

			return false;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.Item56 with { Volume = 2.5f, Pitch = -1f, PitchVariance = 0.5f }, Projectile.Center);

            stretchRecoil = -0.85f;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.98f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.98f;
            }

			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            target.AddBuff(BuffID.OnFire3, 600);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.Y * (Projectile.direction == 1 ? -0.015f : 0.015f);

            Projectile.spriteDirection = Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

            //stretch stuff
			if (stretchRecoil < 0)
			{
				stretchRecoil += 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            //explosion
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ElGourdoExplosion>(), Projectile.damage, 0f, Projectile.owner);

            //fire bolts
            float maxAmount = 15;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(8f, 25f), Main.rand.NextFloat(8f, 25f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(8f, 25f), Main.rand.NextFloat(8f, 25f));
                float intensity = Main.rand.NextFloat(8f, 25f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity,
                ModContent.ProjectileType<ElGordoFire>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                int newDust = Dust.NewDust(Projectile.Center, 1, 1, DustID.InfernoFork, 0f, 0f, 100, default, 3f);
                Main.dust[newDust].noGravity = true;
                Main.dust[newDust].position = Projectile.Center + vector12;
                Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                currentAmount++;
            }

            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool())
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}