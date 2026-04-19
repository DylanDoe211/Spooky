using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FlowerPotShoeFlower : ModProjectile
	{
        public override string Texture => "Spooky/Content/NPCs/Boss/BigBone/Projectiles/BouncingFlower";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
		{
			Projectile.width = 46;
            Projectile.height = 52;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

            Color RealColor = Color.White;

            switch ((int)Projectile.frame)
            {
                case 0:
                {
                    RealColor = Color.Lime;
                    break;
                }
                case 1:
                {
                    RealColor = Color.MediumPurple;
                    break;
                }
                case 2:
                {
                    RealColor = Color.Red;
                    break;
                }
                case 3:
                {
                    RealColor = Color.Gold;
                    break;
                }
            }

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(RealColor);

            Vector2 drawOriginTrail = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
            Vector2 previousPosition = Projectile.Center;

            for (int k = 0; k < trailLength.Length; k++)
            {
                float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
                scale *= 2f;

                if (trailLength[k] == Vector2.Zero)
                {
                    break;
                }

                Vector2 drawPos = trailLength[k] - Main.screenPosition;
                Vector2 currentPos = trailLength[k];
                Vector2 betweenPositions = previousPosition - currentPos;

                float max = betweenPositions.Length();

                for (int i = 0; i < max; i++)
                {
                    drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

                    Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOriginTrail, scale, SpriteEffects.None, 0f);
                }

                previousPosition = currentPos;
            }

            return true;
        }

		public override void AI()
		{
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 15)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }

            int foundTarget = HomeOnTarget();
            if (foundTarget != -1)
            {
                NPC target = Main.npc[foundTarget];
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 800;

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

        public override void OnKill(int timeLeft)
		{
        	for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
	}
}