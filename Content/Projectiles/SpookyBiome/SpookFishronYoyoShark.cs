using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SpookFishronYoyoShark : ModProjectile
	{
        float Distance = Main.rand.NextFloat(100f, 150f);
		float RotationSpeed = Main.rand.NextFloat(8f, 15f);

        private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true; 
			Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
			Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, 27);
            Vector2 drawOriginTrail = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Projectile.localAI[0] > 0)
            {
                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    Color glowColor = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.OrangeRed);

                    for (int circle = 0; circle < 360; circle += 90)
                    {
                        Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)).RotatedBy(MathHelper.ToRadians(circle));

                        float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                        Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + (drawOriginTrail * scale) + new Vector2(0f, Projectile.gfxOffY);
                        Main.EntitySpriteDraw(ProjTexture.Value, drawPos + circular, rectangle, color * 0.75f, Projectile.rotation, drawOriginTrail, scale, spriteEffects, 0);
                    }
                }
            }

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
            rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return Projectile.localAI[0] > 0;
        }

        public override void AI()
		{
            Player player = Main.player[Projectile.owner];
            Projectile Parent = Main.projectile[(int)Projectile.ai[2]];

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            
            if (Projectile.localAI[0] == 0)
            {
                if (!Parent.active || Parent.type != ModContent.ProjectileType<SpookFishronYoyoProj>() || !player.channel)
                {
                    Projectile.Kill();
                }

                Projectile.timeLeft = 180;

                if (RotationSpeed > 2f)
                {
                    RotationSpeed -= 0.15f;
                }

                if (Distance > 0)
                {
                    Distance -= 2f;
                }

                Projectile.ai[0] += RotationSpeed * Projectile.ai[1];
                double rad = Projectile.ai[0] * (Math.PI / 180);
                Projectile.position.X = Parent.Center.X - (int)(Math.Cos(rad) * Distance) - Projectile.width / 2;
                Projectile.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * Distance) - Projectile.height / 2;

                Vector2 FakeVelocity = new Vector2((int)(Math.Cos(rad) * Distance), (int)(Math.Sin(rad) * Distance));

                Projectile.rotation = FakeVelocity.ToRotation() + (Projectile.ai[1] == -1 ? MathHelper.Pi : MathHelper.TwoPi);
			    Projectile.rotation += 0f * (float)Projectile.direction;

                if (Projectile.Hitbox.Intersects(Parent.Hitbox))
                {
                    Projectile.localAI[0]++;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			    Projectile.rotation += 0f * (float)Projectile.direction;

                if (Projectile.localAI[1] == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

                    bool HasFoundTarget = false;
                    foreach (NPC NPC in Main.ActiveNPCs)
                    {
                        if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 650f)
                        {
                            Vector2 ChargeDirection = NPC.Center - Projectile.Center;
                            ChargeDirection.Normalize();                     
                            ChargeDirection *= 25;
                            Projectile.velocity = ChargeDirection;

                            HasFoundTarget = true;
                            break;
                        }
                    }

                    if (!HasFoundTarget)
                    {
                        Vector2 ChargeDirection = Projectile.Center - Parent.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 25;
                        Projectile.velocity = ChargeDirection;
                    }

                    Projectile.localAI[1]++;
                    Projectile.netUpdate = true;
                }
            }
		}

        public void IdleAI(Projectile Parent)
		{
            if (Projectile.Distance(Parent.Center) < 150f)
            {
                float goToX = Parent.Center.X - Projectile.Center.X;
                float goToY = Parent.Center.Y - Projectile.Center.Y;
                float speed = 0.5f;
                
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
                Vector2 desiredVelocity = Projectile.DirectionTo(Parent.Center) * 40;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }
	}
}