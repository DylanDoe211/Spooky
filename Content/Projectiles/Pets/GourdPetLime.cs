using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
    public class GourdPetLime : ModProjectile
    {
        private int playerStill = 0;
        private bool playerFlying = false;

        float stretch;

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
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            stretch = Math.Abs(stretch);

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle rectangle = new Rectangle(0, 0, ProjTexture.Width(), ProjTexture.Height());

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.height / 2 + Projectile.gfxOffY), rectangle, 
            Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height()), scaleStretch, effects, 0);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(254, 125, 13));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextFloat(0.2f, 0.8f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.height / 2 + Projectile.gfxOffY) + circular, rectangle,
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height()), scaleStretch, effects, 0);
            }

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector48 = player.Center - Projectile.Center;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 200f;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().GourdPetLime = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().GourdPetLime)
            {
				Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.3f);

            if (!playerFlying)
            {
                //limit how much it can stretch
                if (stretch > 0.5f)
                {
                    stretch = 0.5f;
                }

                //limit how much it can squish
                if (stretch < -0.5f)
                {
                    stretch = -0.5f;
                }

                if (Projectile.velocity.X != 0)
                {
                    Projectile.localAI[0] += 5;

                    if (Projectile.localAI[0] <= 10)
                    {
                        stretch -= 0.005f;
                    }
                    else if (Projectile.localAI[0] <= 30 && Projectile.localAI[0] > 10)
                    {
                        stretch -= 0.05f;
                    }
                    else if (Projectile.localAI[0] <= 40)
                    {
                        stretch += 0.005f;
                    }
                    else if (Projectile.localAI[0] <= 60 && Projectile.localAI[0] > 40)
                    {
                        stretch += 0.05f;
                    }

                    if (Projectile.localAI[0] >= 60)
                    {
                        Projectile.localAI[0] = 0;
                    }
                }
                else
                {
                    Projectile.localAI[0] = 0;

                    if (stretch > 0)
                    {
                        stretch -= 0.025f;
                    }
                    if (stretch < 0)
                    {
                        stretch += 0.025f;
                    }
                }

                //prevents the pet from getting stuck on sloped tiled
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

                Projectile.rotation = 0;
                Vector2 vector48 = player.Center - Projectile.Center;
                float playerDistance = vector48.Length();

                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -7f;
                }

                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (playerDistance > 75f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.12f;
                        if (Projectile.velocity.X > 7f)
                        {
                            Projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.12f;
                        if (Projectile.velocity.X < -7f)
                        {
                            Projectile.velocity.X = -7f;
                        }
                    }
                }

                if (playerDistance < 75f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.8f)
                        {
                            Projectile.velocity.X -= 0.25f;
                        }
                        else if (Projectile.velocity.X < -0.8f)
                        {
                            Projectile.velocity.X += 0.25f;
                        }
                        else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }

                if (playerDistance < 50f)
                {
                    Projectile.velocity.X *= 0.5f;
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }

                bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, Projectile.position, Projectile.width, Projectile.height);
                if (!HasLineOfSight)
                {
                    Projectile.ai[0]++;
                }

                if (playerDistance >= 450f || Projectile.ai[0] > 150)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
            else if (playerFlying)
            {
                Projectile.ai[0] = 0;

                float Speed = 0.5f;
                Projectile.tileCollide = false;
                Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
                float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                float num21 = 18f;
                float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }

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
                    if (playerDistance < 100f)
                    {
                        Speed = 0.35f;
                    }
                    if (playerDistance > 300f)
                    {
                        Speed = 1f;
                    }
                    
                    playerDistance = num21 / playerDistance;
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

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}