using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
    public class OozeBig : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 40;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            //make the booger scale up and down rapidly before exploding
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 2)
            {
                Projectile.scale -= 0.2f;
            }
            if (Projectile.ai[0] >= 2)
            {
                Projectile.scale += 0.2f;
            }
            
            if (Projectile.ai[0] > 4)
            {
                Projectile.ai[0] = 0;
                Projectile.scale = 1f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 3f, 500f);

            float maxAmount = 50;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(8f, 15f), Main.rand.NextFloat(8f, 15f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(8f, 15f), Main.rand.NextFloat(8f, 15f));
                float intensity = Main.rand.NextFloat(8f, 15f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity,
                ModContent.ProjectileType<OozeSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                int num104 = Dust.NewDust(Projectile.Center, 1, 1, DustID.KryptonMoss, 0f, 0f, 100, default, 3f);
                Main.dust[num104].noGravity = true;
                Main.dust[num104].position = Projectile.Center + vector12;
                Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                currentAmount++;
            }
		}
    }
}