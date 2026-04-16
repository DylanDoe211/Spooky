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
    public class PossessedDagger : ModProjectile
    {
        public int SaveDirection;
        public float SaveRotation;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 14;
			Projectile.height = 14;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

            //draw afterimage trail
            Vector2 drawOriginTrail = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
            Vector2 previousPosition = Projectile.Center;

            for (int k = 0; k < trailLength.Length; k++)
            {
                float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
                scale *= 1f;

                if (trailLength[k] == Vector2.Zero)
                {
                    break;
                }

                Vector2 drawPos = trailLength[k] - Main.screenPosition;
                Vector2 currentPos = trailLength[k];
                Vector2 betweenPositions = previousPosition - currentPos;

                float max = betweenPositions.Length();

                for (int i = 0; i < max; i++)
                {
                    drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

                    Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOriginTrail, scale, SpriteEffects.None, 0f);
                }

                previousPosition = currentPos;
            }

            //draw aura
            for (int i = 0; i < 360; i += 90)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(2f, 2f), Main.rand.NextFloat(2f, 2f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {   
            NPC target = Main.npc[(int)Projectile.ai[1]];

            if (Projectile.timeLeft <= 30)
            {
                Projectile.alpha += 10;

                Projectile.velocity *= 0.5f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            if (Projectile.ai[0] == 0)
            {
                double Velocity = Math.Atan2(target.Center.Y - Projectile.Center.Y, target.Center.X - Projectile.Center.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 25;

                Projectile.ai[0]++;
            }

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
        }
    }
}