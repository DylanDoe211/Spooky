using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class SabertoothScytheProj : ModProjectile
    {
		int SwingDirection;

		float SwingRadians = MathHelper.Pi * 1.35f;
		float rotation;
		
		bool initialized = false;
		bool flip = false;
		bool LaunchedBlade = false;

		Vector2 direction = Vector2.Zero;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> BladeTexture;

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
			BladeTexture ??= ModContent.Request<Texture2D>(Texture + "Blade");

			//fade out stuff
			int SwingTime = ItemGlobal.ActiveItem(player).useTime;
			float progress = Projectile.localAI[0] / (float)SwingTime;
			progress = EaseFunction.EaseQuadOut.Ease(progress);
			float SlashAlpha = 1f - Math.Abs(progress);

			var Effects1 = Projectile.ai[0] == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			var Effects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			if (flip)
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects1, 0f);
				
				if (!LaunchedBlade)
				{
					Main.spriteBatch.Draw(BladeTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(BladeTexture.Width() / 2, BladeTexture.Height()), Projectile.scale, Effects1, 0f);
				}
			}
			else
			{
				Main.spriteBatch.Draw(ProjTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(ProjTexture.Width() / 2, ProjTexture.Height()), Projectile.scale, Effects2, 0f);
				
				if (!LaunchedBlade)
				{
					Main.spriteBatch.Draw(BladeTexture.Value, player.MountedCenter - Main.screenPosition, null, lightColor, rotation + 1.57f, new Vector2(BladeTexture.Width() / 2, BladeTexture.Height()), Projectile.scale, Effects2, 0f);
				}
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

			if (!LaunchedBlade && Projectile.ai[0] == 1 && Projectile.localAI[0] > (SwingTime / 3))
			{
				if (Projectile.owner == Main.myPlayer)
                {
					Vector2 ShootSpeed = Main.MouseWorld - player.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 15f;

					Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + Offset, ShootSpeed, 
					ModContent.ProjectileType<SabertoothScytheFlung>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
				}

				LaunchedBlade = true;
			}

			Projectile.rotation = direction.ToRotation();
			rotation = Projectile.rotation + MathHelper.Lerp(SwingRadians / 2 * SwingDirection, -SwingRadians / 2 * SwingDirection, GetProgress());

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
     
          






