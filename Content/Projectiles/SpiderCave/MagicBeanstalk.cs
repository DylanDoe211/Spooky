using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class MagicBeanstalk : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[50];
		Rectangle[] trailHitboxes = new Rectangle[50];
		float[] rotations = new float[50];

		float SaveRotation;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public static readonly SoundStyle GrowSound = new("Spooky/Content/Sounds/BigBone/PlantGrow", SoundType.Sound) { Pitch = -0.35f };
		public static readonly SoundStyle KillSound = new("Spooky/Content/Sounds/BigBone/PlantDestroy", SoundType.Sound) { Pitch = -0.35f, Volume = 0.5f };

		public override void SendExtraAI(BinaryWriter writer)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                writer.WriteVector2(trailLength[i]);
				writer.Write(rotations[i]);
            }

            //bools
            writer.Write(runOnce);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                trailLength[i] = reader.ReadVector2();
				rotations[i] = reader.ReadSingle();
            }

            //bools
            runOnce = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 36;
			Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			DrawChain(false);

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16));

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(color), SaveRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public bool DrawChain(bool SpawnGore)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Stem");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				Color color = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16));

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				drawPos = previousPosition + -betweenPositions - Main.screenPosition;

				if (!SpawnGore)
				{
					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, rotations[k], drawOrigin, 1f, SpriteEffects.None, 0f);
				}
				else
				{
					if (Main.rand.NextBool())
					{
						if (Main.netMode != NetmodeID.Server) 
						{
							Gore.NewGore(Projectile.GetSource_Death(), previousPosition + -betweenPositions, Vector2.Zero, ModContent.Find<ModGore>("Spooky/MagicBeanstalkStemGore").Type);
						}
					}
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			bool CollidingWithStem = false;

			if (!runOnce)
			{
				for (int i = 0; i < trailHitboxes.Length; i++)
				{
					if (trailHitboxes[i] != Rectangle.Empty && targetHitbox.Intersects(trailHitboxes[i]))
					{
						CollidingWithStem = true;
						break;
					}
					else
					{
						CollidingWithStem = false;
					}
				}
			}

			return targetHitbox.Intersects(projHitbox) || CollidingWithStem;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					trailHitboxes[i] = Rectangle.Empty;
					rotations[i] = 0f;
				}

				runOnce = false;

				Projectile.netUpdate = true;
			}

			Projectile.ai[2]++;
			if (Projectile.ai[2] == 2)
			{
				SoundEngine.PlaySound(GrowSound, Projectile.Center);
			}

			bool StopMoving = true;
			foreach (Vector2 thing in trailLength)
			{
				if (thing == Vector2.Zero)
				{
					StopMoving = false;
				}
			}
			if (!StopMoving)
			{
				int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
				Main.dust[ProjDust].noGravity = true;
				Main.dust[ProjDust].scale = 1.8f;
				Main.dust[ProjDust].velocity /= 4f;
				Main.dust[ProjDust].velocity += Projectile.velocity / 2;

				//save previous positions, rotations, and direction
				if (Projectile.velocity != Vector2.Zero && Projectile.localAI[2] % 2 == 0)
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

						trailHitboxes[i] = new Rectangle((int)current.X - 5, (int)current.Y - 5, 10, 10);
					}
				}

				float WaveIntensity = 5f;
				float Wave = 8f;

				Projectile.ai[0]++;
				if (Projectile.ai[1] == 0)
				{
					if (Projectile.ai[0] > Wave * 0.5f)
					{
						Projectile.ai[0] = 0;
						Projectile.ai[1] = 1;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
				}
				else
				{
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

				SaveRotation = Projectile.rotation;
			}
			else
			{
				Projectile.rotation = SaveRotation;

				Projectile.velocity = Vector2.Zero;
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);

			if (Main.netMode != NetmodeID.Server) 
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/MagicBeanstalkTopGore").Type);
			}

			DrawChain(true);
		}
	}
}