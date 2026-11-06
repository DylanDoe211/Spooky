using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class TinyPumpPutty : ModProjectile
    {
        int playerStill = 0;
        int targetFlyTimer = 0;
        bool playerFlying = false;
        bool FlyToTarget = false;
        bool isAttacking = false;

        public static readonly SoundStyle BoingSound = new("Spooky/Content/Sounds/SlimeBoing", SoundType.Sound) { Volume = 0.25f, Pitch = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 28;
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
			Projectile.minionSlots = 1f;
            Projectile.aiStyle = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (isAttacking)
            {
                SoundEngine.PlaySound(BoingSound with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, Projectile.Center);

                Projectile.velocity.Y = -12f;
            }

			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];

            fallThrough = Projectile.position.Y < player.Center.Y - Projectile.height;

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<TinyPumpPuttyBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<TinyPumpPuttyBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }

            if (Collision.SolidCollision(new Vector2(Projectile.Center.X - 3, Projectile.Bottom.Y - 5), 6, 8))
			{
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 7)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 2)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                if (Projectile.velocity.Y < 0)
                {
                    Projectile.frame = 0;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame >= 3)
                        {
                            Projectile.frame = 2;
                        }
                    }
                }
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 800f)
                {
					AttackingAI(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 800f)
                {
					AttackingAI(NPC);

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
            }

            //prevent projectiles clumping together
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

            if (!FlyToTarget)
            {
                Projectile.rotation = 0;

                if (Projectile.Distance(target.Center) >= 400f)
                {
                    if (Projectile.Center.Y >= target.Center.Y)
                    {
                        FlyToTarget = true;
                        Projectile.velocity = Vector2.Zero;
                    }
                }
                else
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.55f;

                    if (Projectile.Center.Y >= target.Center.Y - 10)
                    {
                        Projectile.tileCollide = true;
                    }
                    else
                    {
                        Projectile.tileCollide = false;
                    }

                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 15;
                    Projectile.velocity.X = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20).X;
                }
            }
            else
            {
                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

                Projectile.frame = 2;
                Projectile.frameCounter = 0;

                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 35;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

                if (Projectile.Distance(target.Center) < 100f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    SoundEngine.PlaySound(BoingSound with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, Projectile.Center);

                    FlyToTarget = false;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = -14f;
                }
            }
        }

        public void IdleAI(Player player)
        {
            if (!playerFlying)
            {
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
                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frame = 2;
                Projectile.frameCounter = 0;

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
            }
        }

        public void JumpTo(NPC target, Player player)
        {
            SoundEngine.PlaySound(BoingSound with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew }, Projectile.Center);

            Vector2 JumpTo = target == null ? new Vector2(player.Center.X, player.Center.Y - 200) : new Vector2(target.Center.X, target.Center.Y - 200);

            Vector2 velocity = JumpTo - Projectile.Center;

            float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 15);
            velocity.Normalize();
            velocity.Y -= 0.12f;
            velocity.X *= target == null ? 0.5f : 0.75f;
            Projectile.velocity = velocity * speed * 1.1f;
        }
    }
}