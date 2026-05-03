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
    public class GlaggleOrbProj : ModProjectile
    {
		float OrbiterDistance = 1.2f;
		float OrbiterRotation = 0f;
		float ExtraRotation = 0f;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> OrbTexture;

		public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
        }

		//shout-out Flye for this orbiter draw code: https://github.com/flye-name/ebonian-mod/blob/1e460b0b4b889f2554e9d3441242edd5257a9eb6/Content/NPCs/Overworld/Asteroid/AsteroidHerder.cs
		public float CircleDividedEqually(float i, float max)
		{
			return 2f * MathF.PI / max * i;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            void DrawGlaggleOrbiters(bool after, float rotOff, Color color)
            {
				OrbTexture ??= ModContent.Request<Texture2D>(Texture + "Small");
                int TotalGlaggles = 5;
                for (int i = after ? TotalGlaggles : 0; after ? i > 0 : i < TotalGlaggles; i += (after ? -1 : 1))
                {
                    float angle = CircleDividedEqually(i, TotalGlaggles) + rotOff;
                    float progress = (MathF.Sin(rotOff * 0.1f) + 1) * 0.5f;
                    float distscale = MathHelper.Lerp(0.5f, 0.65f, progress) * 1.25f * OrbiterDistance;
                    Vector2 angleVec = angle.ToRotationVector2() * 50;
                    float perspectiveScale = MathHelper.Lerp(MathF.Sin(rotOff * 0.4f), 0.75f, 0f);
                    Vector2 offset = new Vector2(angleVec.X, angleVec.Y * (0.25f + perspectiveScale)) * distscale;
					float scale = MathHelper.Lerp(MathHelper.Lerp(0.5f + MathF.Abs(perspectiveScale * 0.25f), 1, MathHelper.Clamp(MathF.Sin(angle), 0, 1f)), 1, 0f);
                    bool shouldDraw = after ? scale > 0.73f : scale <= 0.73f;

                    if (shouldDraw)
                    {
                        Main.EntitySpriteDraw(OrbTexture.Value, Projectile.Center + offset.RotatedBy(OrbiterRotation + MathF.Sin(rotOff * 0.05f) * 3) - Main.screenPosition, null,
						color, angle, OrbTexture.Size() / 2, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            float rotation = Main.GlobalTimeWrappedHourly + Projectile.whoAmI * 5 + MathHelper.ToRadians(ExtraRotation);

			OrbiterRotation = Utils.AngleLerp(OrbiterRotation, -MathF.Sin(rotation * 0.05f) * 3, 0.2f);

			DrawGlaggleOrbiters(false, rotation, lightColor);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            DrawGlaggleOrbiters(true, rotation, lightColor);

            return false;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

        public override void AI()
		{
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

            Projectile.velocity *= 0.97f;

			if (Projectile.timeLeft <= 180)
			{
				if (OrbiterDistance > 0.3f)
				{
					OrbiterDistance -= 0.005f;
				}

				Projectile.ai[0]++;
				Projectile.ai[1] += 0.1f;
				ExtraRotation += MathHelper.Lerp(0, Projectile.ai[1], MathHelper.Clamp(Projectile.ai[0] / 60f, 0, 1));
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.5f, Pitch = 1f }, Projectile.Center);
			SoundEngine.PlaySound(SoundID.NPCDeath31 with { Volume = 0.5f, Pitch = 1f }, Projectile.Center);

			for (int i = 0; i < 5; i++)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 velocity = new Vector2(0, Main.rand.Next(2, 10)).RotatedByRandom(MathHelper.ToRadians(360));

					int newProj = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity,
					ModContent.ProjectileType<GlaggleOrbProjSmall>(), Projectile.damage / 2, Projectile.knockBack, ai0: Projectile.ai[0] == 2 ? 0 : 1);
				}
			}
		}
	}
}
     
          






