using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class InkFishHead : ModProjectile
    {
        bool segmentsSpawned = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 1f;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<InkFishBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<InkFishBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            if (!segmentsSpawned)
            {
                int latestSegment = Projectile.identity;

                Vector2 Center = new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2));

                latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                ModContent.ProjectileType<InkFishBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                ai0: Projectile.identity, ai1: latestSegment);

                latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                ModContent.ProjectileType<InkFishTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                ai0: Projectile.identity, ai1: latestSegment);

                segmentsSpawned = true;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.Distance(player.Center) >= 2000f)
            {
                Projectile.Center = player.Center;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 600f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 600f)
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
                IdleMovement(player);
            }
        }

        public void IdleMovement(Player player)
        {
            Projectile.ai[2] = 0;

            Vector2 PlayerGoTo = new Vector2(player.Center.X, player.Center.Y - 100);

            if (Projectile.Distance(PlayerGoTo) > 50f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(PlayerGoTo) * 12;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }

        public void AttackingAI(NPC target)
        {
            isAttacking = true;

            Vector2 destination = target.Center;

            Projectile.ai[2]++;

            //charge at the target
            if (Projectile.ai[2] == 1)
            {
                Projectile.velocity = Projectile.DirectionTo(destination) * 20;
            }

            //once the target is passed, loop back around
            if (Projectile.ai[2] >= 20)
            {
                double angle = Projectile.DirectionTo(target.Center).ToRotation() - Projectile.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                if (Math.Abs(angle) > Math.PI / 2)
                {
                    Projectile.ai[1] = Math.Sign(angle);
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20;
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(6f) * Projectile.ai[1]);
            }

            //loop attack
            if (Projectile.ai[2] >= 60)
            {
                Projectile.ai[2] = 0;
                Projectile.netUpdate = true;
            }
        }
    }

    public class InkFishBody : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Parent.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }
        
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.HasBuff(ModContent.BuffType<InkFishBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //head parent
            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.ProjectileType<InkFishHead>())
            {
                Projectile.Kill();
            }

            //segment parent
			Projectile SegmentParent = Main.projectile[(int)Projectile.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - Projectile.Center;

			if (SegmentParent.rotation != Projectile.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - Projectile.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.35f);
			}

			Projectile.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				Projectile.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 24f;
			}
        }
    }

    public class InkFishTail : InkFishBody
    {
        private static Asset<Texture2D> ProjTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Parent.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }
    }
}