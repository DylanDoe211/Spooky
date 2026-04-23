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
    public class AntSpiderBoomerangProj : ModProjectile
    {
		float ActualSpeed = -15;

		bool IsStickingToTarget = false;

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			Projectile.ai[0] = 5;

			if (Projectile.ai[1] < 5)
			{
				Projectile.velocity = Vector2.Zero;

				int newProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<AntSpiderBoomerangSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				Main.projectile[newProj].rotation = Main.rand.Next(0, 361);
				Main.projectile[newProj].scale = Main.rand.NextFloat(1f, 2f);

				Projectile.ai[1]++;
			}

			if (!IsStickingToTarget)
            {
				ActualSpeed = 2f;
                Projectile.ai[2] = target.whoAmI;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			Projectile.ai[0] = 5;
			ActualSpeed = -ActualSpeed;

			if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;

			if (IsStickingToTarget && Projectile.ai[1] < 5) 
            {
				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

				Projectile.rotation += 0.75f * (float)Projectile.direction;

                int npcTarget = (int)Projectile.ai[2];
                if (npcTarget < 0 || npcTarget >= 200) 
                {
					Projectile.ai[0] = 5;
					ActualSpeed = 2f;
                    IsStickingToTarget = false;
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
					Projectile.ai[0] = 5;
					ActualSpeed = 2f;
                    IsStickingToTarget = false;
                }
			}
			else
			{
				Projectile.ignoreWater = false;

				Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.05f * (float)Projectile.direction;

				if (Projectile.soundDelay == 0 && ActualSpeed > 2)
				{
					Projectile.soundDelay = 15;
					SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
				}

				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 5 && ActualSpeed < 20)
				{
					ActualSpeed += 0.5f;

					if (ActualSpeed < 0)
					{
						Projectile.velocity *= 0.95f;
					}
				}

				if (ActualSpeed > 0)
				{
					Projectile.tileCollide = false;

					Vector2 Velocity = player.Center - Projectile.Center;
					Velocity.Normalize();
					Velocity *= ActualSpeed;

					Projectile.velocity = Velocity;

					if (Projectile.Hitbox.Intersects(player.Hitbox))
					{
						Projectile.Kill();
					}
				}
			}
        }
    }
}