using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class SpookFishronYoyoProj : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

		float auraRotation = 0f;
		float auraScale = 0f;

		int SharkronShootTimer = 0;

		private static Asset<Texture2D> AuraTexture;
		private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 320f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
			Projectile.penetrate = -1;
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientWeatherPainTornado");

			Vector2 auraOrigin = new(AuraTexture.Width() * 0.5f, AuraTexture.Height() * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + auraOrigin + new Vector2(-94, Projectile.gfxOffY - 94);

			for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(254, 125, 13));

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)).RotatedBy(MathHelper.ToRadians(i));

            	Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, null, color * 0.45f, auraRotation, auraOrigin, (Projectile.scale * 0.5f) * auraScale, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, null, color * 0.3f, -auraRotation, auraOrigin, (Projectile.scale * 0.8f) * auraScale, SpriteEffects.FlipHorizontally, 0);
			}

			return true;
		}

		public override void PostDraw(Color lightColor)
        {
			if (Projectile.ai[0] != -1f)
			{
				if (runOnce)
				{
					return;
				}

				TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

				Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
				Vector2 previousPosition = Projectile.Center;

				for (int k = 0; k < trailLength.Length; k++)
				{
					float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
					scale *= 1f;

					Color color = Color.Lerp(new Color(251, 92, 37), new Color(249, 255, 74), scale);

					if (trailLength[k] == Vector2.Zero)
					{
						return;
					}

					Vector2 drawPos = trailLength[k] - Main.screenPosition;
					Vector2 currentPos = trailLength[k];
					Vector2 betweenPositions = previousPosition - currentPos;

					float max = betweenPositions.Length();

					for (int i = 0; i < max; i++)
					{
						drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

						Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
					}

					previousPosition = currentPos;
				}
			}
		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			if (player.channel)
			{
				if (auraScale < 1f)
				{
					auraScale += 0.025f;
				}
			}
			else
			{
				if (auraScale > 0f)
				{
					auraScale -= 0.1f;
				}
			}

			if (auraScale > 0f)
			{
				auraRotation += 0.12f;
			}

			//shoot out sharks
			if (player.channel)
			{
				bool HasFoundTarget = false;
				if (!HasFoundTarget)
				{
					foreach (NPC NPC in Main.ActiveNPCs)
					{
						if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 650f)
						{
							HasFoundTarget = true;
							break;
						}
					}
				}

				if (HasFoundTarget)
				{
					SharkronShootTimer++;
					if (SharkronShootTimer >= 45)
					{
						SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

						int randDist = Main.rand.Next(1, 360);
						
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 12).RotatedByRandom(360), 
						ModContent.ProjectileType<SpookFishronYoyoShark>(), Projectile.damage, 0f, Projectile.owner, 
						ai0: randDist, ai1: Main.rand.NextBool() ? -1 : 1, ai2: Projectile.whoAmI);
						SharkronShootTimer = 0;
					}
				}

				foreach (NPC NPC in Main.ActiveNPCs)
            	{
					if (NPC.active && NPC.Distance(Projectile.Center) <= 90f && !NPC.friendly && !NPC.dontTakeDamage && NPC.immune[Projectile.owner] == 0)
					{
						int direction = NPC.Center.X < player.Center.X ? -1 : 1;
						player.ApplyDamageToNPC(NPC, Projectile.damage, Projectile.knockBack, direction, false, null, true);
						NPC.immune[Projectile.owner] = 15;
					}
				}
			}

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
        }
    }
}
