using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientGoblinFlyMouth : ModProjectile
    {
        int saveDirection = 0;

        bool Charging = false;
        bool isAttacking = false;

        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 42;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
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
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            if (Projectile.ai[1] < 200)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;
            }
            else
            {
                Projectile.spriteDirection = saveDirection;
            }

            //flying animation
            if (!Charging)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            //charging animation
            else
            {
                Projectile.frame = 3;
            }

            Projectile.ai[1]++;

            //go to the side of the target to prepare for dashing
            if (Projectile.ai[1] < 30)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X) ? -135 : 135;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //charge at the target
            if (Projectile.ai[1] == 30)
            {
                Charging = true;

                saveDirection = Projectile.spriteDirection;

                //set frame to charging immediately 
                Projectile.frame = 5;
                
                Vector2 ChargeSpeed = target.Center - Projectile.Center;
                ChargeSpeed.Normalize();

                ChargeSpeed.X *= 25;
                ChargeSpeed.Y *= 12;
                Projectile.velocity.X = ChargeSpeed.X;
                Projectile.velocity.Y = ChargeSpeed.Y;
            }

            //slow down at the end of the charge
            if (Projectile.ai[1] >= 40)
            {
                Projectile.velocity *= 0.7f;
            }

            //loop ai
            if (Projectile.ai[1] >= 50)
            {
                Charging = false;
                Projectile.ai[1] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.spriteDirection = player.Center.X > Projectile.Center.X ? -1 : 1;

            //idle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            ///reset attacking ai stuff
            Charging = false;
            Projectile.ai[1] = 0;

            float FlySpeed = 0.5f;
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
}