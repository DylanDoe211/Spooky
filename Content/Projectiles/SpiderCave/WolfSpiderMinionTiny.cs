using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class WolfSpiderMinionTiny : ModProjectile
    {
        bool isAttacking = false;

        NPC CurrentTarget = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
			Projectile.height = 12;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
		{
            return isAttacking;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];

            fallThrough = CurrentTarget == null ? (Projectile.position.Y < player.Center.Y - (Projectile.height) && !isAttacking) : (Projectile.position.Y < CurrentTarget.Center.Y - (Projectile.height));

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            //prevents this from getting stuck on sloped tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 450f)
                {
					AttackingAI(Target);
                    CurrentTarget = Target;

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
                {
					AttackingAI(NPC);
                    CurrentTarget = NPC;

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking)
            {
                IdleAI();
                CurrentTarget = null;
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.tileCollide = true;

            Projectile.rotation = 0;

            Vector2 vector48 = target.Center - Projectile.Center;
            float targetDistance = vector48.Length();

            if (Projectile.velocity.Y == 0 && (HoleBelow() || (targetDistance > 50f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -10f;
            }

            //jump if the target is too high
            if (Projectile.velocity.Y == 0 && target.Center.Y < Projectile.Center.Y - 50)
            {
                Projectile.velocity.Y = Main.rand.Next(-12, -8);
            }

            Projectile.velocity.Y += 0.35f;
            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            if (target.position.X - Projectile.position.X > 0f)
            {
                Projectile.velocity.X += 0.12f;
                if (Projectile.velocity.X > 6f)
                {
                    Projectile.velocity.X = 6f;
                }
            }
            else
            {
                Projectile.velocity.X -= 0.12f;
                if (Projectile.velocity.X < -6f)
                {
                    Projectile.velocity.X = -6f;
                }
            }

            //set frames when idle
            if (Projectile.velocity.X == 0)
            {
                Projectile.frame = 4;
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public void IdleAI()
        {
            Projectile.velocity.X *= 0.85f;

            Projectile.velocity.Y += 0.35f;
            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            //set frames when idle
            if (Projectile.velocity.X == 0)
            {
                Projectile.frame = 4;
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}