using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class SpookFishronYoyoProj : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[15];
        Rectangle[] trailHitboxes = new Rectangle[15];

		private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 50000f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 420f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 18f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }

        public override void PostDraw(Color lightColor)
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			bool CollidingWithTrail = false;

			if (!runOnce)
			{
				for (int i = 0; i < trailHitboxes.Length; i++)
				{
					if (trailHitboxes[i] != Rectangle.Empty && targetHitbox.Intersects(trailHitboxes[i]))
					{
						CollidingWithTrail = true;
						break;
					}
					else
					{
						CollidingWithTrail = false;
					}
				}
			}

			return targetHitbox.Intersects(projHitbox) || CollidingWithTrail;
		}

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 90)
            {
				SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

				int randDist = Main.rand.Next(1, 360);
				
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 12).RotatedByRandom(360), 
				ModContent.ProjectileType<SpookFishronYoyoShark>(), Projectile.damage, 0f, Projectile.owner, 
				ai0: randDist, ai1: Main.rand.NextBool() ? -1 : 1, ai2: Projectile.whoAmI);
                Projectile.localAI[0] = 0;
            }

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
                    trailHitboxes[i] = Rectangle.Empty;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;

                trailHitboxes[i] = new Rectangle((int)current.X - 5, (int)current.Y - 5, 10, 10);
			}
        }
    }
}
