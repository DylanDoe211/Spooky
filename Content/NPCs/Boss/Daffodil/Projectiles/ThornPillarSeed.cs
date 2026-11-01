using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class ThornPillarSeed : ModProjectile
    {
        Vector2 GoToPosition = Vector2.Zero;

        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle ThornSpawnSound = new("Spooky/Content/Sounds/Daffodil/SeedThorn", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			//draw aura
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = i switch
                {
                    1 => new(0, -2),
                    2 => new(2, 0),
                    3 => new(0, 2),
                    _ => new(-2, 0)
                };

                Main.EntitySpriteDraw(DrawUtils.ColorSolid(ProjTexture.Value, Color.White), vector + offset, rectangle, 
                Projectile.GetAlpha(Color.Lime * 0.35f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            NPC Parent = Main.npc[(int)Projectile.ai[2]];

            //add light for visibility
            Lighting.AddLight(Projectile.Center, 0.2f, 0.35f, 0f);

            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 30)
            {
                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			    Projectile.rotation += 0f * (float)Projectile.direction;

                if (GoToPosition == Vector2.Zero)
                {
                    GoToPosition = new Vector2(Parent.Center.X + Main.rand.Next(-700, 700), Parent.Center.Y + 400);
                }
                else
                {
                    if (Projectile.Center.Y > GoToPosition.Y && IsColliding())
                    {
                        Projectile.velocity *= 0.35f;

                        Projectile.ai[1]++;
                        if (Projectile.ai[1] >= 50)
                        {
                            SoundEngine.PlaySound(ThornSpawnSound, Projectile.Center);

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -12),
                            ModContent.ProjectileType<ThornPillar>(), Projectile.damage, 0, Main.myPlayer);

                            Projectile.Kill();
                        }
                    }
                    else
                    {
                        Vector2 desiredVelocity = Projectile.DirectionTo(GoToPosition) * 12;
				        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    }
                }
            }
        }

        public bool IsColliding()
        {
            int minTilePosX = (int)(Projectile.position.X / 16) - 1;
            int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16) + 2;
            int minTilePosY = (int)(Projectile.position.Y / 16) - 1;
            int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    if (Main.tile[i, j] != null && (Main.tile[i, j].HasTile && (Main.tileSolid[(int)Main.tile[i, j].TileType])))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}