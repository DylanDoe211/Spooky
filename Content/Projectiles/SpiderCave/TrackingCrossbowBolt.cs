using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class TrackingCrossbowBolt : ModProjectile
    {
        public bool IsStickingToTarget = false;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget)
            {
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()       
        {
            if (IsStickingToTarget) 
            {
				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int npcTarget = (int)Projectile.ai[1];
                if (npcTarget < 0 || npcTarget >= 200) 
                {
                    IsStickingToTarget = false;
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
                    IsStickingToTarget = false;
                }
			}
			else 
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;

                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
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