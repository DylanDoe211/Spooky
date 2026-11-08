using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class TatantulaTomeFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true; 
			Projectile.tileCollide = false;
            Projectile.netImportant = true;
			Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
		}

        public override bool? CanDamage()
		{
            return Projectile.ai[2] == 0;
        }

        public override bool? CanCutTiles()
        {
            return Projectile.ai[0] > 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 180;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[2] == 0)
            {
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == 1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (player.controlUseItem && Projectile.ai[0] == 0 && Projectile.ai[2] == 0)
            {
                Projectile.penetrate = -1;
                Projectile.timeLeft = 180;

                Projectile.tileCollide = false;

                if (Projectile.owner == Main.myPlayer)
                {
                    float goToX = Main.MouseWorld.X + Main.rand.Next(-200, 200);
                    float goToY = Main.MouseWorld.Y + Main.rand.Next(-200, 200);

                    Vector2 GoTo = new Vector2(goToX, goToY);

                    float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 0, 12);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
                }

                //prevent projectiles clumping together
                for (int num = 0; num < Main.projectile.Length; num++)
                {
                    Projectile other = Main.projectile[num];
                    if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                    {
                        const float pushAway = 0.08f;
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

            Projectile.ai[1]++;

            if (Main.mouseLeftRelease && Projectile.ai[0] == 0 && Projectile.ai[1] >= 5)
            {
                if (player.ownedProjectileCounts[Type] >= 10)
                {
                    Projectile.penetrate = 1;

                    Projectile.tileCollide = true;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ChargeDirection = Main.MouseWorld - Projectile.Center;
                        ChargeDirection.Normalize();

                        ChargeDirection *= -Main.rand.Next(15, 25);
                        Projectile.velocity = ChargeDirection;
                    }

                    Projectile.ai[0] = 1;
                }
                else
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[2] = 1;
                }
            }

            if (Projectile.ai[0] == 1 && Projectile.ai[2] == 0)
            {
                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
                }
            }

            if (Projectile.ai[2] > 0)
            {
                Projectile.rotation += 0.25f * (float)Projectile.direction;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

                Projectile.alpha += 5;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 350;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }
    }
}