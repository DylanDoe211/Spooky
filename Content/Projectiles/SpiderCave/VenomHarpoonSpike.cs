using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class VenomHarpoonSpike : ModProjectile
	{
		int ProjectileCenterOffsetY = 13;
		bool HasHitBoss = false;

		NPC GrappledNPC = null;

		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults() 
        {
			Projectile.width = 40;
			Projectile.height = 40;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			Projectile ParentProjectile = Main.projectile[(int)Projectile.ai[1]];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/BallSpiderWeb");

			int XAdd = Projectile.Center.Y > ParentProjectile.Center.Y ? (ParentProjectile.spriteDirection == 1 ? 10 : -10) : (ParentProjectile.spriteDirection == 1 ? -10 : 10);

            Vector2 ParentCenter = new Vector2(ParentProjectile.Center.X + XAdd, ParentProjectile.Center.Y - ProjectileCenterOffsetY);

			Rectangle? chainSourceRectangle = null;
			float chainHeightAdjustment = 0f;

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10;
			}

			float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

			while (chainLengthRemainingToDraw > 0f)
			{
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorToParent * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Venom, 300);

			if (GrappledNPC == null && target.active && target.CanBeChasedBy(this) && !target.IsTechnicallyBoss() && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
			{	
				GrappledNPC = target;

				Projectile.ai[0] = 25;
			}
			if (Projectile.ai[0] >= 12 && !HasHitBoss && target.active && target.CanBeChasedBy(this) && target.IsTechnicallyBoss() && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
			{	
				SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

				Projectile ParentProjectile = Main.projectile[(int)Projectile.ai[1]];
				Vector2 ParentCenter = new Vector2(ParentProjectile.Center.X, ParentProjectile.Center.Y - ProjectileCenterOffsetY);

				for (int numProjectiles = 0; numProjectiles < 6; numProjectiles++)
				{
					Vector2 ShootSpeed = target.Center - ParentCenter;
					ShootSpeed.Normalize();
					ShootSpeed.X *= Main.rand.Next(15, 25);
					ShootSpeed.Y *= Main.rand.Next(15, 25);

					Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;

					Projectile.NewProjectile(Projectile.GetSource_FromAI(), ParentCenter + muzzleOffset, ShootSpeed, 
					ModContent.ProjectileType<VenomHarpoonSpit>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
				}
		
				Projectile.ai[0] = 25;
				HasHitBoss = true;
			}
		}

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];

			Projectile ParentProjectile = Main.projectile[(int)Projectile.ai[1]];

			Vector2 ParentCenter = new Vector2(ParentProjectile.Center.X, ParentProjectile.Center.Y - ProjectileCenterOffsetY);

			Vector2 ParentTowardsChild = Projectile.DirectionTo(ParentCenter).SafeNormalize(Vector2.Zero);
			ParentProjectile.rotation = ParentTowardsChild.ToRotation() + MathHelper.PiOver2;
			ParentProjectile.timeLeft = 2;

			Vector2 vectorTowardsPlayer = Projectile.DirectionTo(ParentCenter).SafeNormalize(Vector2.Zero);
			Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 15)
			{
				int XAdd = Projectile.Center.Y > ParentProjectile.Center.Y ? (ParentProjectile.spriteDirection == 1 ? 10 : -10) : (ParentProjectile.spriteDirection == 1 ? -10 : 10);

				Vector2 ParentCenter2 = new Vector2(ParentProjectile.Center.X + XAdd, ParentProjectile.Center.Y - ProjectileCenterOffsetY);

				Vector2 RetractSpeed = Projectile.Center - ParentCenter2;
				RetractSpeed.Normalize();

				if (GrappledNPC != null)
				{
					if (GrappledNPC.Distance(player.Center) >= 230f)
					{
						GrappledNPC.position = Projectile.Center - GrappledNPC.Size / 2;
					}

					RetractSpeed *= 32;
					Projectile.velocity = -RetractSpeed;
				}
				else
				{
					RetractSpeed *= 65;
					Projectile.velocity = -RetractSpeed;
				}

				if (Projectile.Distance(ParentCenter2) <= 150f)
				{
					ParentProjectile.ai[1] = 1;

					if (GrappledNPC != null)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

						GrappledNPC.velocity = Vector2.Zero;

						for (int numProjectiles = 0; numProjectiles < 6; numProjectiles++)
						{
							Vector2 ShootSpeed = GrappledNPC.Center - ParentCenter;
							ShootSpeed.Normalize();
							ShootSpeed.X *= Main.rand.Next(15, 25);
							ShootSpeed.Y *= Main.rand.Next(15, 25);

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;

							Projectile.NewProjectile(Projectile.GetSource_FromAI(), ParentCenter + muzzleOffset, ShootSpeed, ModContent.ProjectileType<VenomHarpoonSpit>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
						}

						ParentProjectile.timeLeft = 10;
					}

					Projectile.Kill();
				}
			}
		}
	}
}