using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class PirateHookProj : ModProjectile
    {
		int SwingDirection;

		float SwingRadians = MathHelper.Pi * 1.35f;
		float rotation;

		public static List<float> oldRotations = new List<float>();

		bool initialized = false;
		bool flip = false;
		bool ShotProjectile = false;

		Vector2 direction = Vector2.Zero;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(70, 70);
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.ownerHitCheck = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 16;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			//fade out stuff
			int SwingTime = ItemGlobal.ActiveItem(player).useTime;
			float progress = Projectile.localAI[0] / (float)SwingTime;
			progress = EaseFunction.EaseQuadOut.Ease(progress);
			float SlashAlpha = 1f - Math.Abs(progress);

			var Effects1 = Projectile.ai[0] == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			var Effects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw slash afterimages
			for (int oldRot = 0; oldRot < oldRotations.Count; oldRot++)
			{
				for (int i = 0; i < 360; i += 90)
				{
					Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

					Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

					float TrailAlpha = 0.2f + (oldRot / (float)oldRotations.Count);

					if (flip)
					{
						Main.spriteBatch.Draw(TrailTexture.Value, player.MountedCenter - Main.screenPosition + circular, null, color * 0.5f * TrailAlpha, oldRotations[oldRot] + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects1, 0f);
					}
					else
					{
						Main.spriteBatch.Draw(TrailTexture.Value, player.MountedCenter - Main.screenPosition + circular, null, color * 0.5f * TrailAlpha, oldRotations[oldRot] + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects2, 0f);
					}
				}
			}

			//draw hook itself
			if (flip)
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects1, 0f);
			}
			else
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects2, 0f);
			}

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Player player = Main.player[Projectile.owner];

			Vector2 lineDirection = rotation.ToRotationVector2();
			float collisionPoint = 0;
			
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + (lineDirection * Projectile.width), Projectile.height, ref collisionPoint))
			{
				return true;
			}

			return false;
		}

		public override void CutTiles()
		{
			Player player = Main.player[Projectile.owner];
			
			Vector2 lineDirection = rotation.ToRotationVector2();
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(player.Center, player.Center + (lineDirection * Projectile.width), Projectile.height, DelegateMethods.CutTiles);
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (GetProgress() > 0.2f && GetProgress() < 0.8f)
			{
				return base.CanHitNPC(target);
			}

			return false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.HitDirectionOverride = Math.Sign(direction.X);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			int SwingTime = ItemGlobal.ActiveItem(player).useTime;

			Projectile.velocity = Vector2.Zero;
			player.itemTime = player.itemAnimation = 5;
			player.heldProj = Projectile.whoAmI;

			if (!initialized)
			{
				if (Projectile.owner == Main.myPlayer)
				{
					Vector2 ProjDirection = Main.MouseWorld - new Vector2(player.MountedCenter.X, player.MountedCenter.Y);
					ProjDirection.Normalize();
					Projectile.ai[1] = ProjDirection.X;
					Projectile.ai[2] = ProjDirection.Y;
					Projectile.netUpdate = true;
				}

				direction = new Vector2(Projectile.ai[1], Projectile.ai[2]);

				if (direction.X < 0) flip = !flip;

				SwingDirection = -1 * Math.Sign(direction.X);

				initialized = true;
				Projectile.netUpdate = true;
			}

			direction = new Vector2(Projectile.ai[1], Projectile.ai[2]);

			Projectile.Center = player.MountedCenter + (direction.RotatedBy(-1.57f) * 20);

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] > SwingTime)
			{
				Projectile.Kill();
			}

			if (!ShotProjectile && Projectile.localAI[0] > (SwingTime / 3))
			{
				SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaivePierce, player.Center);

				if (Projectile.owner == Main.myPlayer)
                {
					Vector2 ShootSpeed = Main.MouseWorld - player.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 2f;

					Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + Offset, ShootSpeed, 
					ModContent.ProjectileType<PirateHookScythe>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				}

				ShotProjectile = true;
			}

			Projectile.rotation = direction.ToRotation();
			rotation = Projectile.rotation + MathHelper.Lerp(SwingRadians / 2 * SwingDirection, -SwingRadians / 2 * SwingDirection, GetProgress());

			oldRotations.Add(rotation);
			if (oldRotations.Count >= 6)
			{
				oldRotations.RemoveAt(0);
			}

			Projectile.alpha += 2;

			player.direction = Math.Sign(direction.X);

			player.itemRotation = rotation;

			if (player.direction != 1)
			{
				player.itemRotation -= 3.14f;
			}

			Projectile.netUpdate = true;

			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - 1.57f);
		}

		private float GetProgress()
		{
			Player player = Main.player[Projectile.owner];

			int SwingTime = ItemGlobal.ActiveItem(player).useTime;

			float progress = Projectile.localAI[0] / (float)SwingTime;
			progress = EaseFunction.EaseQuadOut.Ease(progress);

			return Projectile.ai[0] == 1 ? -progress + 0.98f : progress;
		}
	}
}
     
          






