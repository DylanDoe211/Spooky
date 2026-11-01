using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class LingeringCandle : ModProjectile
    {
        bool Shake = false;

		private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void PostDraw(Color lightColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Rectangle rectangle = new Rectangle(0, 0, GlowTexture.Width(), GlowTexture.Height());

            if (Projectile.ai[1] > 80)
            {
                for (int i = 0; i < 360; i += 60)
			    {
                    Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.White);

                    Vector2 circular = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), Main.rand.NextFloat(0.5f, 2.5f)).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + circular, rectangle,
                    color, Projectile.rotation, GlowTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }
        }

        public override void AI()
        {
            if (Shake)
            {
                Projectile.rotation += 0.01f;
                if (Projectile.rotation > 0.2f)
                {
                    Shake = false;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.rotation -= 0.01f;
                if (Projectile.rotation < -0.2f)
                {
                    Shake = true;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.velocity *= 0.95f;

            Projectile.ai[1]++;
            if (Projectile.ai[1] == 80)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Pitch = -0.5f, Volume = 0.5f }, Projectile.Center);

                Projectile.velocity = Vector2.Zero;
                Projectile.ai[0] = Projectile.position.Y;
                Projectile.localAI[0]++;

                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] > 80)
            {
                Projectile.ai[2]++;
                Projectile.position.Y = Projectile.ai[0] + Projectile.velocity.Y + (float)Math.Sin(Projectile.ai[2] / 30) * 30;

                if (Projectile.timeLeft > 60)
                {
                    Lighting.AddLight(Projectile.Center, 1f, 0.75f, 0.5f);

                    foreach (var NPC in Main.ActiveNPCs)
                    {
                        if (NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && NPC.Distance(Projectile.Center) <= 100f)
                        {
                            NPC.AddBuff(BuffID.OnFire3, 600);
                        }
                    }

                    if (Projectile.ai[1] % 10 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * Main.rand.NextFloat(100f, 121f));
                            offset.Y += (float)(Math.Cos(angle) * Main.rand.NextFloat(100f, 121f));
                            Vector2 DustPos = Projectile.Center + offset - new Vector2(4, 4);
                            Dust dust = Main.dust[Dust.NewDust(DustPos, 0, 0, DustID.Torch, 0, 0, 100, Color.White, 1f)];
                            dust.velocity = -((DustPos - Projectile.Center) * 0.01f);
                            dust.noGravity = true;
                            dust.noLight = true;
                            dust.scale = 2f;
                        }
                    }
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}