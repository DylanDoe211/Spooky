using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientGoblinTiny : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;
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
            Projectile.width = 28;
			Projectile.height = 22;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 40;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 0.25f;
            Projectile.aiStyle = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
        {
            Projectile.velocity.X = -Projectile.velocity.X / 4;
            Projectile.velocity.Y = -10;
            Projectile.ai[1]++;
        }

        public override bool? CanDamage()
		{
			return Projectile.ai[1] <= 0;
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

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<GoblinoidBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<GoblinoidBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 750f)
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
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 750f)
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
                IdleAI(player);
                CurrentTarget = null;
            }

            for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];

				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.5f;

					if (Projectile.position.X < other.position.X)
					{
                        if (Projectile.velocity.X > -2)
                        {
						    Projectile.velocity.X -= pushAway;
                        }
					}
					else
					{
                        if (Projectile.velocity.X < 2)
                        {
						    Projectile.velocity.X += pushAway;
                        }
					}
				}
			}

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.15f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            if (Projectile.ai[1] == 0)
            {
                Projectile.tileCollide = false;

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 35f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                Projectile.tileCollide = true;

                Projectile.rotation = 0;
                
                Projectile.velocity.Y += 0.35f;
                if (Projectile.velocity.Y > 2f)
                {
                    Projectile.velocity.Y += 0.35f;
                }

                Projectile.ai[0]++;
                if (Projectile.velocity.Y > 0 && Collision.SolidTilesVersatile((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.X) / 16, (int)Projectile.Bottom.Y / 16, (int)Projectile.Bottom.Y / 16 + 3))
                {
                    Projectile.velocity.X = 0;

                    if (Projectile.ai[0] >= 20)
                    {
                        Projectile.ai[0] = 0;
                        Projectile.ai[1] = 0;
                    }
                }
            }
        }

        public void IdleAI(Player player)
        {
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;

            if (!playerFlying)
            {
                //set frames when idle
                if (Projectile.velocity.Y == 0)
                {
                    Projectile.frame = 0;
                }
                //up frame
                else if (Projectile.velocity.Y < 0)
                {
                    Projectile.frame = 2;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0)
                {
                    Projectile.frame = 3;
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.rotation = 0;

                Projectile.velocity.Y += 0.35f;

                Projectile.tileCollide = true;

                //slow down a bit while falling after jumping
                if (Projectile.velocity.Y >= 0)
                {
                    Projectile.velocity.X *= 0.98f;
                }
                
                //slow down quickly while on the ground
                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) < 120f)
                {
                    Projectile.velocity.X *= 0.8f;
                }

                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) >= 120f)
                {
                    JumpTo(null, player);
                }

                if (Projectile.Distance(player.Center) >= 500f)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
            else
            {
                float FlySpeed = 0.5f;
                Projectile.tileCollide = false;
                float horiPos = player.Center.X - Projectile.Center.X;
                float vertiPos = player.Center.Y - Projectile.Center.Y;

                //offset values when flying
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;

                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                float num21 = 18f;

                //teleport to the player if its way too far away
                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }

                if (playerDistance < 100f)
                {
                    FlySpeed = 0.5f;
                    if (player.velocity.Y == 0f)
                    {
                        playerStill++;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        playerFlying = false;
                        Projectile.velocity *= 0.2f;
                        Projectile.tileCollide = true;
                    }
                }

                if (playerDistance < 50f)
                {
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    {
                        Projectile.velocity *= 0.90f;
                    }
                    FlySpeed = 0.02f;
                }
                else
                {
                    if (playerDistance < 100f)
                    {
                        FlySpeed = 0.35f;
                    }
                    if (playerDistance > 300f)
                    {
                        FlySpeed = 1f;
                    }
                    
                    playerDistance = num21 / playerDistance;
                    horiPos *= playerDistance;
                    vertiPos *= playerDistance;
                }

                if (Projectile.velocity.X <= horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X + FlySpeed;
                    if (FlySpeed > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + FlySpeed;
                    }
                }

                if (Projectile.velocity.X > horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X - FlySpeed;
                    if (FlySpeed > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - FlySpeed;
                    }
                }

                if (Projectile.velocity.Y <= vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + FlySpeed;
                    if (FlySpeed > 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + FlySpeed * 2f;
                    }
                }

                if (Projectile.velocity.Y > vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - FlySpeed;
                    if (FlySpeed > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - FlySpeed * 2f;
                    }
                }

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        public void JumpTo(NPC target, Player player)
        {
            Vector2 JumpTo = target == null ? new Vector2(player.Center.X, player.Center.Y - 100) : new Vector2(target.Center.X, target.Center.Y - 200);

            Vector2 velocity = JumpTo - Projectile.Center;

            float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 15);
            velocity.Normalize();
            velocity.Y -= 0.12f;
            velocity.X *= target == null ? 0.5f : 0.75f;
            Projectile.velocity = velocity * speed * 1.1f;
        }
    }
}