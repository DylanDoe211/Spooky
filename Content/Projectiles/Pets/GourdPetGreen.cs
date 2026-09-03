using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Pets
{
    public class GourdPetGreen : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;

        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            float stretch = playerFlying ? 0f : Projectile.velocity.Y * 0.045f;

			stretch = Math.Abs(stretch);

			//limit how much it can stretch
			if (stretch > 0.65f)
			{
				stretch = 0.65f;
			}

			//limit how much it can squish
			if (stretch < -0.65f)
			{
				stretch = -0.65f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			
			if (Projectile.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}
			if (Projectile.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}

            Rectangle rectangle = new Rectangle(0, 0, ProjTexture.Width(), ProjTexture.Height());

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, 
            Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), scaleStretch, SpriteEffects.None, 0);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(254, 125, 13));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextFloat(0.2f, 0.8f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + circular, rectangle,
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), scaleStretch, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];

            fallThrough = Projectile.position.Y < player.Center.Y - Projectile.height;

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().GourdPetGreen = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().GourdPetGreen)
            {
				Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.3f);

            if (!playerFlying)
            {
                Projectile.rotation = 0;

                Projectile.velocity.Y += 0.35f;

                Projectile.tileCollide = true;

                //slow down a bit while falling after jumping
                if (Projectile.velocity.Y >= 0)
                {
                    Projectile.velocity.X *= 0.98f;
                }
                
                //slow down quickly while on the ground
                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) < 75f)
                {
                    Projectile.velocity.X *= 0.8f;
                }

                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) >= 75f)
                {
                    JumpTo(player);
                }

                bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, Projectile.position, Projectile.width, Projectile.height);
                if (!HasLineOfSight)
                {
                    Projectile.ai[0]++;
                }

                if (Projectile.Distance(player.Center) >= 450f || Projectile.ai[0] > 150)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
            else
            {
                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                Projectile.tileCollide = false;

                Projectile.ai[0] = 0;

                float Speed = 0.5f;
                float horiPos = player.Center.X - Projectile.Center.X;
                float vertiPos = player.Center.Y - Projectile.Center.Y;
                vertiPos += (float)Main.rand.Next(-10, 15);
                horiPos += (float)Main.rand.Next(-10, 15);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;

                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

                if (playerDistance < 100f)
                {
                    Speed = 0.5f;
                    if (player.velocity.Y == 0f)
                    {
                        playerStill++;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        playerFlying = false;
                        Projectile.velocity *= 0.2f;
                        Projectile.tileCollide = true;
                    }
                }

                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }

                if (playerDistance < 50f)
                {
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    {
                        Projectile.velocity *= 0.90f;
                    }

                    Speed = 0.02f;
                }
                else
                {
                    if (playerDistance < 150f)
                    {
                        Speed = 0.1f;
                    }
                    if (playerDistance > 400f)
                    {
                        Speed = 0.25f;
                    }
                    
                    playerDistance = 18f / playerDistance;
                    horiPos *= playerDistance;
                    vertiPos *= playerDistance;
                }

                if (Projectile.velocity.X <= horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X + Speed;
                    if (Speed > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + Speed;
                    }
                }

                if (Projectile.velocity.X > horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X - Speed;
                    if (Speed > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - Speed;
                    }
                }

                if (Projectile.velocity.Y <= vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + Speed;
                    if (Speed > 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + Speed * 2f;
                    }
                }

                if (Projectile.velocity.Y > vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - Speed;
                    if (Speed > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - Speed * 2f;
                    }
                }
            }
		}

        public void JumpTo(Player player)
        {
            Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 80);

            Vector2 velocity = JumpTo - Projectile.Center;

            bool Faster = Projectile.Distance(player.Center) >= 70f;

            float speed = MathHelper.Clamp(velocity.Length() / 36, 5, 15);
            velocity.Normalize();
            velocity.Y -= 0.12f;
            velocity.X *= (Faster ? 1f : 0.5f);
            Projectile.velocity = velocity * speed * 1.1f;
        }
    }
}