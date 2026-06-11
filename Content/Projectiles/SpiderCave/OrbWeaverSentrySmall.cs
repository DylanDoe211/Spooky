using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class OrbWeaverSentrySmall : ModProjectile
    {
        public bool isAttacking = false;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool? CanDamage()
		{
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            if (!isAttacking || (isAttacking && Projectile.ai[0] < 30))
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            if (isAttacking && Projectile.ai[0] >= 30)
            {
                Projectile.frame = 4;
            }

            //fall down constantly
            Projectile.velocity.Y++;
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 500f)
                {
					AttackingAI(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 500f)
                {
					AttackingAI(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 30)
            {
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 17f;
                        
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 5), ShootSpeed, 
                ModContent.ProjectileType<OrbWeaverSentrySmallSpike>(), Projectile.damage, 2f, Projectile.owner);

                //shoot additional spreads of spikes
                for (int numProjectiles = -2; numProjectiles <= -1; numProjectiles++)
                {
                    float Velocity = Main.rand.NextFloat(12f, 16f);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X - 10, Projectile.Center.Y - 5), 
                    Velocity * Projectile.DirectionTo(new Vector2(Projectile.Center.X, Projectile.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                    ModContent.ProjectileType<OrbWeaverSentrySmallSpike>(), Projectile.damage, 3f, Projectile.owner, ai0: 1);
                }
                for (int numProjectiles = 1; numProjectiles <= 2; numProjectiles++)
                {
                    float Velocity = Main.rand.NextFloat(12f, 16f);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + 10, Projectile.Center.Y - 5), 
                    Velocity * Projectile.DirectionTo(new Vector2(Projectile.Center.X, Projectile.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                    ModContent.ProjectileType<OrbWeaverSentrySmallSpike>(), Projectile.damage, 3f, Projectile.owner, ai0: 1);
                }
            }

            if (Projectile.ai[0] >= 60)
            {
                Projectile.ai[0] = 0;
            }
		}
    }
}