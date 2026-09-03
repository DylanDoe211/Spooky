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
    public class GourdPetOrange1 : ModProjectile
    {
        private int playerStill = 0;
        private bool playerFlying = false;

        float saveRotation;

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
            Projectile.width = 18;
            Projectile.height = 20;
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

            Rectangle rectangle = new Rectangle(0, 0, GlowTexture.Width(), GlowTexture.Height());

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 4), rectangle,
            Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(254, 125, 13));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextFloat(0.2f, 0.8f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 4) + circular, rectangle,
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), Projectile.scale, SpriteEffects.None, 0);
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
				player.GetModPlayer<SpookyPlayer>().GourdPetOrange = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().GourdPetOrange)
            {
				Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.3f);

            bool IsSecondVariant = Projectile.type == ModContent.ProjectileType<GourdPetOrange2>();

            if (!playerFlying)
            {
                Vector2 vector48 = player.Center - Projectile.Center;
                float playerDistance = vector48.Length();

                //prevents the pet from getting stuck on sloped tiled
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                
                float MaxDist = IsSecondVariant ? 90f : 70f;

                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > (MaxDist + 30)) || (playerDistance > (MaxDist + 30) && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -6f;
                }

                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (playerDistance > MaxDist)
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

                if (playerDistance < MaxDist)
                {
                    if (Projectile.velocity.X < 0.1f && Projectile.velocity.X > -0.1f)
                    {
                        Projectile.velocity.X = 0f;
                    }
                    else
                    {
                        Projectile.velocity.X *= 0.8f;
                    }
                }

                //rotate and store rotation constantly if moving, and if not moving then use the last stored rotation
                if (Projectile.velocity.X != 0)
                {
                    saveRotation = Projectile.rotation;
                    Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.035f * (float)Projectile.direction;
                }
                else
                {
                    Projectile.rotation = saveRotation;
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
                float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
                float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;
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

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;
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

    public class GourdPetOrange2 : GourdPetOrange1
    {
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18;
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

            Rectangle rectangle = new Rectangle(0, 0, GlowTexture.Width(), GlowTexture.Height());

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 4), rectangle,
            Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(254, 125, 13));

				Vector2 circular = new Vector2(Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextFloat(0.2f, 0.8f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 5) + circular, rectangle,
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, ProjTexture.Height() / 2f), Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}