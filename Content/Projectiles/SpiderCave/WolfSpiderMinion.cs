using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class WolfSpiderMinion : ModProjectile
    {
        int playerStill = 0;

        float BabyScale1 = 0f;
        float BabyScale2 = 0f;
        float BabyScale3 = 0f;
        float BabyScale4 = 0f;

        bool playerFlying = false;
        bool isAttacking = false;
        bool canAttack = false;
        bool HasBabySpider1 = false;
        bool HasBabySpider2 = false;
        bool HasBabySpider3 = false;
        bool HasBabySpider4 = false;

        private static Asset<Texture2D> BabyTexture1;
        private static Asset<Texture2D> BabyTexture2;
        private static Asset<Texture2D> BabyTexture3;
        private static Asset<Texture2D> BabyTexture4;

        NPC CurrentTarget = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
			Projectile.height = 52;
            Projectile.DamageType = DamageClass.Summon;
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
            Player player = Main.player[Projectile.owner];

            fallThrough = CurrentTarget == null ? (Projectile.position.Y < player.Center.Y - (Projectile.height) && !isAttacking) : (Projectile.position.Y < CurrentTarget.Center.Y - (Projectile.height));

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
			BabyTexture1 ??= ModContent.Request<Texture2D>(Texture + "Baby1");
            BabyTexture2 ??= ModContent.Request<Texture2D>(Texture + "Baby2");
            BabyTexture3 ??= ModContent.Request<Texture2D>(Texture + "Baby3");
            BabyTexture4 ??= ModContent.Request<Texture2D>(Texture + "Baby4");

            Vector2 drawOrigin = new(BabyTexture1.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, BabyTexture1.Height() / Main.projFrames[Projectile.type] * Projectile.frame, BabyTexture1.Width(), BabyTexture1.Height() / Main.projFrames[Projectile.type]);
			
            var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (HasBabySpider1)
            {
			    Main.EntitySpriteDraw(BabyTexture1.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, BabyScale1, spriteEffects, 0);
            }
            if (HasBabySpider2)
            {
			    Main.EntitySpriteDraw(BabyTexture2.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, BabyScale2, spriteEffects, 0);
            }
            if (HasBabySpider3)
            {
			    Main.EntitySpriteDraw(BabyTexture3.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, BabyScale3, spriteEffects, 0);
            }
            if (HasBabySpider4)
            {
			    Main.EntitySpriteDraw(BabyTexture4.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, BabyScale4, spriteEffects, 0);
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<WolfSpiderMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<WolfSpiderMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //prevents this from getting stuck on sloped tiles
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Projectile.Distance(Target.Center) <= 800f)
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
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Projectile.Distance(NPC.Center) <= 800f)
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
                canAttack = false;
                CurrentTarget = null;
            }

            if (HasBabySpider1 && BabyScale1 < 1f)
            {
                BabyScale1 += 0.1f;
            }
            if (HasBabySpider2 && BabyScale2 < 1f)
            {
                BabyScale2 += 0.1f;
            }
            if (HasBabySpider3 && BabyScale3 < 1f)
            {
                BabyScale3 += 0.1f;
            }
            if (HasBabySpider4 && BabyScale4 < 1f)
            {
                BabyScale4 += 0.1f;
            }

            if (!canAttack)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 30)
                {
                    if (!HasBabySpider1)
                    {
                        HasBabySpider1 = true;
                    }
                    else if (!HasBabySpider2)
                    {
                        HasBabySpider2 = true;
                    }
                    else if (!HasBabySpider3)
                    {
                        HasBabySpider3 = true;
                    }
                    else if (!HasBabySpider4)
                    {
                        HasBabySpider4 = true;
                    }

                    Projectile.ai[1] = 0;
                }
            }
            else
            {
                Projectile.ai[1] = 0;
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

            Projectile.tileCollide = true;

            Projectile.rotation = 0;

            Vector2 vector48 = target.Center - Projectile.Center;
            float targetDistance = vector48.Length();

            //if theres a hole or tile in the way jump to the player
            if (Projectile.velocity.Y == 0 && ((HoleBelow() && targetDistance > 100f) || (targetDistance > 300f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -8f;
            }

            Projectile.velocity.Y += 0.35f;
            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            //if close enough, spawn babies
            if (Projectile.Distance(target.Center) <= 300f)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;

                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;

                if (Projectile.velocity.Y > 0 && Collision.SolidTilesVersatile((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.X) / 16, (int)Projectile.Bottom.Y / 16, (int)Projectile.Bottom.Y / 16 + 3))
                {
                    Projectile.velocity.X = 0;
                }

                if (HasBabySpider1 && HasBabySpider2 && HasBabySpider3 && HasBabySpider4)
                {
                    canAttack = true;
                }
                if (!HasBabySpider1 && !HasBabySpider2 && !HasBabySpider3 && !HasBabySpider4)
                {
                    canAttack = false;
                }

                Projectile.ai[0]++;
                if (Projectile.ai[0] % 30 == 0 && canAttack)
                {
                    Vector2 ShootSpeed = target.Center - Projectile.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 25f;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 10, 
                    ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<WolfSpiderMinionTiny>(), Projectile.damage, 2f, Projectile.owner);

                    if (HasBabySpider1)
                    {
                        HasBabySpider1 = false;
                        BabyScale1 = 0f;
                    }
                    else if (HasBabySpider2)
                    {
                        HasBabySpider2 = false;
                        BabyScale2 = 0f;
                    }
                    else if (HasBabySpider3)
                    {
                        HasBabySpider3 = false;
                        BabyScale3 = 0f;
                    }
                    else if (HasBabySpider4)
                    {
                        HasBabySpider4 = false;
                        BabyScale4 = 0f;
                    }

                    Projectile.ai[0] = 0;
                }
            }
            //if a bit too far, move at the enemy
            else
            {
                //idle frames
                if (Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 4;
                }
                //up frame
                else if (Projectile.velocity.Y < 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 5;
                }
                //moving animation
                else if (Projectile.velocity.X != 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 5)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 1;
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
            }
        }

        public void IdleAI(Player player)
        {
            if (!playerFlying)
            {
                Projectile.rotation = 0;

                Vector2 vector48 = player.Center - Projectile.Center;
                float playerDistance = vector48.Length();

                //if theres a hole or tile in the way jump to the player
                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 170f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -8f;
                }

                Projectile.velocity.Y += 0.35f;
                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (playerDistance > 500f)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }

                if (playerDistance > 120f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
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
                }

                if (playerDistance < 110f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.8f)
                        {
                            Projectile.velocity.X -= 0.25f;
                        }
                        else if (Projectile.velocity.X < -0.8f)
                        {
                            Projectile.velocity.X += 0.25f;
                        }
                        else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }

                //idle frames
                if (Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 4;
                }
                //up frame
                else if (Projectile.velocity.Y < 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 5;
                }
                //moving animation
                else if (Projectile.velocity.X != 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 5)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 1;
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
            else if (playerFlying)
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