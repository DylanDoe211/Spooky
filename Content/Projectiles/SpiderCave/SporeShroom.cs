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
	public class SporeShroom : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[15];
		float[] rotations = new float[15];

		float SaveRotation = 0f;
		float addedStretch = 0f;
		float stretchRecoil = 0f;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetDefaults()
		{
			Projectile.width = 70;
			Projectile.height = 38;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Stem");

			Vector2 drawOriginTrail = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					break;
				}

				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color lightColorAtPos = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16));
				Color color = Color.Lerp(lightColorAtPos * 0f, lightColorAtPos, scale);

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / 15;

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Color colorWithAlphaWhenDying = Projectile.timeLeft <= 60 ? Projectile.GetAlpha(color) : color;

					Main.EntitySpriteDraw(TrailTexture.Value, drawPos, null, colorWithAlphaWhenDying, rotations[k], drawOriginTrail, Projectile.scale, SpriteEffects.None, 0);
				}

				previousPosition = currentPos;
			}

			float stretch = 0f;
			stretch = Math.Abs(stretch) - addedStretch;

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

			Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, scaleStretch, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			addedStretch = -stretchRecoil;

			if (stretchRecoil > 0f)
			{
				stretchRecoil -= 0.02f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					rotations[i] = 0f;
				}

				runOnce = false;
			}

			if (Projectile.scale < 1f)
			{
				Projectile.scale += 0.075f;
			}

			if (Projectile.localAI[0] > 15)
			{
				Projectile.velocity = Vector2.Zero;

				Projectile.rotation = SaveRotation;
			}

			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}
			else
			{
				Projectile.localAI[0]++;
				if (Projectile.localAI[0] <= 15)
				{
					SaveRotation = Projectile.rotation;

					int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch);
					Main.dust[ProjDust].noGravity = true;
					Main.dust[ProjDust].scale = 1.2f;
					Main.dust[ProjDust].velocity /= 4f;
					Main.dust[ProjDust].velocity += Projectile.velocity / 2;

					//save previous positions, rotations, and direction
					if (Projectile.velocity != Vector2.Zero)
					{
						Vector2 current = Projectile.Center;
						float currentRot = Projectile.rotation;
						for (int i = 0; i < trailLength.Length; i++)
						{
							Vector2 previousPosition = trailLength[i];
							trailLength[i] = current;
							current = previousPosition;

							float previousRot = rotations[i];
							rotations[i] = currentRot;
							currentRot = previousRot;
						}
					}

					//move in a random wavy pattern
					float WaveIntensity = 5f;
					float Wave = 5f;

					Projectile.ai[0]++;
					if (Projectile.ai[0] <= Wave)
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					if (Projectile.ai[0] >= Wave * 2)
					{
						Projectile.ai[0] = 0;
					}
				}
				else
				{
					//spawn spores
					if (Projectile.localAI[0] % 20 == 0)
					{
						int MaxSpores = Main.rand.Next(1, 4);
						for (int numSpores = 0; numSpores < MaxSpores; numSpores++)
						{
							Vector2 ProjectilePosition = Projectile.Center + new Vector2(Main.rand.Next(0, 170), 0).RotatedByRandom(360);

							int Spore = Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjectilePosition, Vector2.Zero, ModContent.ProjectileType<SporeShroomCloud>(), 
							Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Main.rand.Next(0, 6));
							Main.projectile[Spore].rotation = Main.rand.NextFloat(0f, 360f);
						}
					}

					//allow players to bounce off of it
					if (stretchRecoil <= 0f)
					{
						foreach (var player in Main.ActivePlayers)
						{
							if (player.Hitbox.Intersects(Projectile.Hitbox))
							{
								SoundEngine.PlaySound(SoundID.Item56 with { Pitch = -0.7f }, Projectile.Center);

								stretchRecoil = 0.5f;

								player.velocity.Y = -15;

								//restore a bit of the players wing time (only if below half)
								if (player.wingTime < (player.wingTimeMax / 2))
								{
									player.wingTime += player.wingTimeMax / 6;
								}

								//spawn some more spores when jumped on 
								int MaxSpores = Main.rand.Next(4, 9);
								for (int numSpores = 0; numSpores < MaxSpores; numSpores++)
								{
									Vector2 ProjectilePosition = Projectile.Center + new Vector2(Main.rand.Next(0, 170), 0).RotatedByRandom(360);

									int Spore = Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjectilePosition, Vector2.Zero, ModContent.ProjectileType<SporeShroomCloud>(), 
									Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Main.rand.Next(0, 6));
									Main.projectile[Spore].rotation = Main.rand.NextFloat(0f, 360f);
								}
							}
						}
					}
				}
			}
		}
	}
}