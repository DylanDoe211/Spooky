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
		Vector2[] trailLength = new Vector2[12];

		float auraRotation = 0f;
		float auraScale = 0f;

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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }

        public override void PostDraw(Color lightColor)
        {
			AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/SpookyTornadoSpawner");

			Vector2 auraOrigin = new(AuraTexture.Width() * 0.5f, AuraTexture.Height() * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + auraOrigin + new Vector2(-31, Projectile.gfxOffY - 31);

			for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)).RotatedBy(MathHelper.ToRadians(i));

            	Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, null, color * 0.55f, auraRotation, auraOrigin, Projectile.scale * auraScale, SpriteEffects.None, 0);
				Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, null, color * 0.25f, -auraRotation, auraOrigin, (Projectile.scale * 2f) * auraScale, SpriteEffects.FlipHorizontally, 0);
			}

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
			Rectangle bigHitbox = new Rectangle((int)Projectile.Center.X - 40, (int)Projectile.Center.Y - 40, 80, 80);
			return targetHitbox.Intersects(bigHitbox);
		}

        public override void AI()
        {
			auraRotation += 0.12f;

			if (auraScale < 1f)
			{
				auraScale += 0.025f;
			}

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 60)
            {
				bool HasFoundTarget = false;
                foreach (var NPC in Main.ActiveNPCs)
				{
					if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 650f)
					{
						HasFoundTarget = true;
						break;
					}
				}

				if (HasFoundTarget)
				{
					SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

					int randDist = Main.rand.Next(1, 360);
					
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 12).RotatedByRandom(360), 
					ModContent.ProjectileType<SpookFishronYoyoShark>(), Projectile.damage, 0f, Projectile.owner, 
					ai0: randDist, ai1: Main.rand.NextBool() ? -1 : 1, ai2: Projectile.whoAmI);
					Projectile.localAI[0] = 0;
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
