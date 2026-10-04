using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Shipyard
{
	public class PirateShipWheelVortex : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            int height = GlowTexture.Height() / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, GlowTexture.Width(), height);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle,
            lightColor * 0.5f, Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0; i < 360; i += 90)
            {
                Color color1 = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Teal);
                Color color2 = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.MediumSpringGreen);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + circular, rectangle,
                color1 * 0.45f, Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), Projectile.scale * 0.5f, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + circular, rectangle,
                color2 * 0.45f, -Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), Projectile.scale * 1.2f, SpriteEffects.FlipHorizontally, 0);
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}
            
            Projectile.rotation -= 0.15f;

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                Projectile.ai[1]++;

                if (Projectile.ai[1] >= 60)
                {
                    Projectile.velocity *= 0.985f;
                }

                if (Projectile.ai[1] <= 60)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 GoTo = Main.MouseWorld;

                        Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * 2;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    }
                }

                if (Projectile.ai[1] >= 120)
                {
                    Projectile.ai[1] = 0;
                }

                //create circle of dusts
                Projectile.ai[2]++;
                if (Projectile.ai[2] % 10 == 0)
                {
                    float Distance = 25 * (Projectile.frame + 1);
                    int Amount = Main.rand.Next(1, 3) * (Projectile.frame + 1);

                    for (int numDusts = 0; numDusts < Amount; numDusts++)
                    {
                        Vector2 offset = new Vector2();
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        offset.X += (float)(Math.Sin(angle) * Distance);
                        offset.Y += (float)(Math.Cos(angle) * Distance);
                        Vector2 DustPos = Projectile.Center + offset - new Vector2(4, 4);
                        Dust dust = Main.dust[Dust.NewDust(DustPos, 0, 0, 102, 0, 0, 100, default, 1.5f)];
                        dust.velocity = -((DustPos - Projectile.Center) * Main.rand.NextFloat(0.01f, 0.12f));
                        dust.velocity += Projectile.velocity;
                        dust.noGravity = true;
                    }
                }

                //make it grow every 15 seconds
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 420 && Projectile.frame < 3)
                {
                    SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item167 with { Pitch = 0.5f }, Projectile.Center);

                    Projectile.frame++;

                    Projectile.ai[0] = 0;
                }

                float MaxDistance = 50f * (Projectile.frame + 1);
                float Strength = 2f * (Projectile.frame + 1); 

                foreach (var NPC in Main.ActiveNPCs)
				{
                    if (!NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && NPC.CanBeChasedBy(this) && !NPC.IsChild(out _) && !NPC.IsTechnicallyBoss() && !NPCID.Sets.CountsAsCritter[NPC.type] && 
                    NPC.Distance(Projectile.Center) <= MaxDistance && NPC.Distance(Projectile.Center) >= 30f)
                    {
						Vector2 vector = NPC.Center;
						float num4 = Projectile.Center.X - vector.X;
						float num5 = Projectile.Center.Y - vector.Y;
						float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
						if (num6 > 0)
						{
							num6 = Strength / num6;
						}
						num4 *= num6;
						num5 *= num6;
						int num7 = 5;
						NPC.velocity.X = (NPC.velocity.X * (float)(num7 - 1) + num4) / (float)num7;
						NPC.velocity.Y = (NPC.velocity.Y * (float)(num7 - 1) + num5) / (float)num7;
					}
				}

                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
            }
        }
	}
}